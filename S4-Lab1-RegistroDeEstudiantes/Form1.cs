using Microsoft.VisualBasic;
using System.Text.Json;

namespace S4_Lab1_RegistroDeEstudiantes
{
    public partial class Form1 : Form
    {
        //password
        string valor;
        string codigo = "admin123";

        //Inicialización del formulario
        public Form1()
        {
            InitializeComponent();
            cargarListas();
        }

        //Evento que se ejecuta al mostrar el formulario
        private void Form1_Shown(object sender, EventArgs e)
        {
            valor = Interaction.InputBox("Ingrese el codigo de administrador:", "Inicio de sesión");
            if (valor != codigo)
            {
                if (valor == "")
                {
                    MessageBox.Show("Sesión cancelada", "Cerrando programa",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    //Interaction.MsgBox("Acceso Denegado. Código incorrecto.");
                    MessageBox.Show("Advertencia: La clave es incorrecta", "Acceso denegado",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.Close();
            }
            else
            {
                Interaction.MsgBox("Acceso Permitido. Bienvenido al sistema de registro.",
                    MsgBoxStyle.OkOnly, "Registro correcto");
            }
        }

        private void cargarListas()
        {
            string json = File.ReadAllText("carrerasUniversitarias.json");

            Carreras data = JsonSerializer.Deserialize<Carreras>(json);
            foreach (var carrera in data.carreras_unificadas)
            {
                cbxCarrera.Items.Add(carrera);
            }

            for (int i = 0; i < 11; i++)
            {
                cbx_Semestre.Items.Add((i + 1).ToString());
            }
        }
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void verRegistroToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void SubMenu_nuevo_Click(object sender, EventArgs e)
        {
            tbxCedula.Clear();
            tbxNombre.Clear();
            tbxUsuario.Clear();
            tbxPassword.Clear();
            tbxConfirmacion.Clear();

            cbxCarrera.SelectedIndex = -1;
            cbx_Semestre.SelectedIndex = -1;

            rbMatutina.Checked = false;
            rbVespertina.Checked = false;

            chbx_Terminos.Checked = false;
            chbx_Notificaciones.Checked = false;
        }

        private void SubMenu_guardar_Click(object sender, EventArgs e)
        {
            ListaDeEstudiantes data;

            if (File.Exists("Lista de estudiantes.json"))
            {
                string json = File.ReadAllText("Lista de estudiantes.json");
                data = JsonSerializer.Deserialize<ListaDeEstudiantes>(json);
            }
            else
            {
                // Si el archivo no existe, se crea uno base
                data = new ListaDeEstudiantes()
                {
                    estudiantes = new List<Estudiante>()
                };
            }

            Estudiante nuevo = new Estudiante()
            {
                nombre = tbxNombre.Text,
                cedula = tbxCedula.Text,
                carrera = cbxCarrera.SelectedItem.ToString(),
                semestre = int.Parse(cbx_Semestre.SelectedItem.ToString()),
                jornada = rbMatutina.Checked ? "Matutina" : "Vespertina",
                usuario = tbxUsuario.Text,
                password = tbxPassword.Text 
            };

            data.estudiantes.Add(nuevo);

            var opciones = new JsonSerializerOptions
            {
                WriteIndented = true // para formatearlo bonito
            };

            string nuevoJson = JsonSerializer.Serialize(data, opciones);

            File.WriteAllText("Lista de estudiantes.json", nuevoJson);

        }

        private void SubMenu_salir_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
