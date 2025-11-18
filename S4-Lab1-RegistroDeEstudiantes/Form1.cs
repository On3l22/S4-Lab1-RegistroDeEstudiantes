using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

        // ///// EVENTO AL MOSTRAR EL FORMULARIO ////////////////////////////////////////////////////
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

        // ////// METODO PARA CARGAR LAS LISTAS DEL COMBOBOX /////////////////////////////////////
        private void cargarListas()
        {
            string json = File.ReadAllText("carrerasUniversitarias.json");

            Carreras data = JsonSerializer.Deserialize<Carreras>(json);
            // Llenar el ComboBox con las carreras
            foreach (var carrera in data.carreras_unificadas)
            {
                cbxCarrera.Items.Add(carrera);
            }

            // Llenar el ComboBox con los semestres (1-11)
            for (int i = 0; i < 11; i++)
            {
                cbx_Semestre.Items.Add((i + 1).ToString());
            }

            cargarRegistros();
        }

        private void cargarRegistros()
        {
            // Leer archivo
            string json = File.ReadAllText("Lista de estudiantes.json");

            // Convertir JSON a objeto C#
            ListaDeEstudiantes datos = JsonConvert.DeserializeObject<ListaDeEstudiantes>(json);

            // Asignar la lista directamente al DataGridView
            dataGridView1.DataSource = datos.estudiantes;
        }

        // ///// VER UTLIMO REGISTRO GUARDADO ////////////////////////////////////////////////////
        private void verRegistroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Verificar si el archivo existe
            if (!File.Exists("Lista de estudiantes.json"))
            {
                MessageBox.Show("No hay registros guardados.", "Sin datos",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Leer el archivo JSON
            string json = File.ReadAllText("Lista de estudiantes.json");

            if (string.IsNullOrWhiteSpace(json))
            {
                MessageBox.Show("El archivo de registros está vacío.", "Sin datos",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Convertir JSON a objeto
            ListaDeEstudiantes data = JsonSerializer.Deserialize<ListaDeEstudiantes>(json);

            if (data == null || data.estudiantes == null || data.estudiantes.Count == 0)
            {
                MessageBox.Show("No hay estudiantes guardados.", "Sin datos",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Obtener el último estudiante registrado
            Estudiante ultimo = data.estudiantes.Last();

            // Cargar datos en los controles del formulario
            tbxNombre.Text = ultimo.nombre;
            tbxCedula.Text = ultimo.cedula;
            tbxUsuario.Text = ultimo.usuario;
            tbxPassword.Text = ultimo.password;
            tbxConfirmacion.Text = ultimo.password;

            cbxCarrera.SelectedItem = ultimo.carrera;
            cbx_Semestre.SelectedItem = ultimo.semestre.ToString();

            if (ultimo.jornada == "Matutina")
                rbMatutina.Checked = true;
            else
                rbVespertina.Checked = true;

            // Opcionales (si deseas marcar algo por defecto)
            //chbx_Terminos.Checked = true;
            //chbx_Notificaciones.Checked = true;

            MessageBox.Show("Registro cargado correctamente.", "Información",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ///// EVENTO DEL SUBMENU NUEVO ///////////////////////////////////////////////////////
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

        // ///// EVENTO DEL SUBMENU GUARDAR //////////////////////////////////////////////////////
        private void SubMenu_guardar_Click(object sender, EventArgs e)
        {


            // VALIDACION DE CAMPOS VACIO
            if (string.IsNullOrWhiteSpace(tbxNombre.Text) ||
                string.IsNullOrWhiteSpace(tbxCedula.Text) ||
                string.IsNullOrWhiteSpace(tbxUsuario.Text) ||
                string.IsNullOrWhiteSpace(tbxPassword.Text) ||
                string.IsNullOrWhiteSpace(tbxConfirmacion.Text) ||
                cbxCarrera.SelectedIndex == -1 ||
                cbx_Semestre.SelectedIndex == -1 ||
                (rbMatutina.Checked == false && rbVespertina.Checked == false))
            {
                MessageBox.Show("Uno de los campos está vacío.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //  VALIDACIÓN NOMBRE SIN SIMBOLOS
            foreach (char c in tbxNombre.Text)
            {
                if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
                {
                    MessageBox.Show("El nombre no puede contener símbolos.", "Dato ilógico",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            //CANTIDAD MÁXIMA DE CARACTERES
            if (tbxNombre.Text.Length > 30)
            {
                MessageBox.Show("El nombre excede el límite permitido (30 caracteres).",
                    "Dato ilógico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // VALIDACION DEL FORMATO DE LA CEDULA
            string patron = @"^\d{1,2}-\d{1,4}-\d{1,6}$";


            if (!Regex.IsMatch(tbxCedula.Text, patron))
            {
                MessageBox.Show("La cédula debe contener números separados por guiones. Ejemplos válidos:\n2-755-39\n02-0755-000039",
                    "Dato ilógico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            //VALIDACIÓN PASSWORD 
            if (tbxPassword.Text != tbxConfirmacion.Text)
            {
                MessageBox.Show("La contraseña y la confirmación no coinciden.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ListaDeEstudiantes data;

            if (File.Exists("Lista de estudiantes.json"))
            {
                try
                {
                    string json = File.ReadAllText("Lista de estudiantes.json");

                    if (string.IsNullOrWhiteSpace(json))
                    {
                        // Archivo vacío → crear lista nueva
                        data = new ListaDeEstudiantes()
                        {
                            estudiantes = new List<Estudiante>()
                        };
                    }
                    else
                    {
                        data = JsonSerializer.Deserialize<ListaDeEstudiantes>(json);

                        if (data == null || data.estudiantes == null)
                        {
                            data = new ListaDeEstudiantes()
                            {
                                estudiantes = new List<Estudiante>()
                            };
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("El archivo JSON está dañado. Se creará uno nuevo.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    data = new ListaDeEstudiantes()
                    {
                        estudiantes = new List<Estudiante>()
                    };
                }
            }
            else
            {
                data = new ListaDeEstudiantes()
                {
                    estudiantes = new List<Estudiante>()
                };
            }


            // CREAR EL NUEVO OBJETO
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

            if (!chbx_Terminos.Checked)
            {
                //VALIDA QUE SE ACEPTEN TÉRMINOS Y CONDICIONES
                MessageBox.Show("Debes aceptar los términos y condiciones para continuar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //SE GUARDA EL ARCHIVO
            var opciones = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string nuevoJson = JsonSerializer.Serialize(data, opciones);
            File.WriteAllText("Lista de estudiantes.json", nuevoJson);

            MessageBox.Show("Registro guardado correctamente.", "Éxito",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        // ///// EVENTO DEL SUBMENU SALIR /////////////////////////////////////////////////////////
        private void SubMenu_salir_Click(object sender, EventArgs e)
        {
            Close();
        }

        
    }
}
