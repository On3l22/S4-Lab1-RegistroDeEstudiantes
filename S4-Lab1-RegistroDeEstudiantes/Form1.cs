using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace S4_Lab1_RegistroDeEstudiantes
{
    public partial class Form1 : Form
    {
        string valor;
        string codigo = "admin123";
        bool esValidoCierre = false;

        public Form1()
        {
            InitializeComponent();
            cargarListas();

            // Activar salto con ENTER
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;

            tbxNombre.KeyDown += SaltarConEnter;
            tbxCedula.KeyDown += SaltarConEnter;
            tbxUsuario.KeyDown += SaltarConEnter;
            tbxPassword.KeyDown += SaltarConEnter;
            tbxConfirmacion.KeyDown += SaltarConEnter;
            cbxCarrera.KeyDown += SaltarConEnter;
            cbx_Semestre.KeyDown += SaltarConEnter;
        }

        // ===== FUNCION PARA ENTER =====
        private void SaltarConEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.SelectNextControl((Control)sender, true, true, true, true);
            }
        }

        // ===== EVENTO AL MOSTRAR EL FORMULARIO =====
        private void Form1_Shown(object sender, EventArgs e)
        {
            while (true)
            {
                valor = Interaction.InputBox("Ingrese el codigo de administrador:", "Inicio de sesión");

                if (valor != codigo)
                {
                    if (valor == "")
                    {
                        MessageBox.Show("Sesión cancelada", "Cerrando programa",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Advertencia: La clave es incorrecta", "Acceso denegado",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    Interaction.MsgBox("Acceso Permitido. Bienvenido al sistema de registro.",
                        MsgBoxStyle.OkOnly, "Registro correcto");

                    tbxNombre.Focus();
                    break;
                }
            }


        }

        // ===== CARGAR LISTAS =====
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

            cargarRegistros();
        }

        private void cargarRegistros()
        {
            if (!File.Exists("Lista de estudiantes.json"))
                return;

            string json = File.ReadAllText("Lista de estudiantes.json");

            ListaDeEstudiantes datos = JsonConvert.DeserializeObject<ListaDeEstudiantes>(json);

            dataGridView1.DataSource = datos.estudiantes;

            // Ocultar contraseña
            if (dataGridView1.Columns["password"] != null)
            {
                dataGridView1.Columns["password"].Visible = false;
            }
        }

        // ===== VER ÚLTIMO REGISTRO =====
        private void verRegistroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists("Lista de estudiantes.json"))
            {
                MessageBox.Show("No hay registros guardados.", "Sin datos",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string json = File.ReadAllText("Lista de estudiantes.json");

            if (string.IsNullOrWhiteSpace(json))
            {
                MessageBox.Show("El archivo de registros está vacío.", "Sin datos",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ListaDeEstudiantes data = JsonSerializer.Deserialize<ListaDeEstudiantes>(json);

            if (data == null || data.estudiantes == null || data.estudiantes.Count == 0)
            {
                MessageBox.Show("No hay estudiantes guardados.", "Sin datos",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Estudiante ultimo = data.estudiantes.Last();

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

            MessageBox.Show("Registro cargado correctamente.", "Información",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ===== NUEVO =====
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

            tbxNombre.Focus();
        }

        // ===== GUARDAR =====
        private void SubMenu_guardar_Click(object sender, EventArgs e)
        {

            tbxUsuario.Text = tbxNombre.Text + tbxCedula.Text.Replace("-", "");

            // ----- VALIDACIONES -----
            if (string.IsNullOrWhiteSpace(tbxNombre.Text) ||
                string.IsNullOrWhiteSpace(tbxCedula.Text) ||
                string.IsNullOrWhiteSpace(tbxUsuario.Text) ||
                string.IsNullOrWhiteSpace(tbxPassword.Text) ||
                string.IsNullOrWhiteSpace(tbxConfirmacion.Text) ||
                cbxCarrera.SelectedIndex == -1 ||
                cbx_Semestre.SelectedIndex == -1 ||
                (!rbMatutina.Checked && !rbVespertina.Checked))
            {
                MessageBox.Show("Uno de los campos está vacío.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (char c in tbxNombre.Text)
            {
                if (!char.IsLetter(c) && !char.IsWhiteSpace(c))
                {
                    MessageBox.Show("El nombre no puede contener símbolos.", "Dato ilógico",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (tbxNombre.Text.Length > 30)
            {
                MessageBox.Show("El nombre excede el límite permitido (30 caracteres).",
                    "Dato ilógico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string patron = @"^\d{1,2}-\d{1,4}-\d{1,6}$";

            if (!Regex.IsMatch(tbxCedula.Text, patron))
            {
                MessageBox.Show("La cédula debe tener formato válido como 2-755-39 o 02-0755-000039",
                    "Dato ilógico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
                        data = new ListaDeEstudiantes() { estudiantes = new List<Estudiante>() };
                    }
                    else
                    {
                        data = JsonSerializer.Deserialize<ListaDeEstudiantes>(json);

                        if (data == null || data.estudiantes == null)
                            data = new ListaDeEstudiantes() { estudiantes = new List<Estudiante>() };
                    }
                }
                catch
                {
                    MessageBox.Show("Archivo dañado. Se creará uno nuevo.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    data = new ListaDeEstudiantes() { estudiantes = new List<Estudiante>() };
                }
            }
            else
            {
                data = new ListaDeEstudiantes() { estudiantes = new List<Estudiante>() };
            }

            // ----- VERIFICAR REPETIDOS -----
            bool cedulaExiste = data.estudiantes.Any(e => e.cedula == tbxCedula.Text);
            if (cedulaExiste)
            {
                MessageBox.Show("La cédula ya está registrada.", "Dato repetido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool usuarioExiste = data.estudiantes.Any(e => e.usuario == tbxUsuario.Text);
            if (usuarioExiste)
            {
                MessageBox.Show("El usuario ya está registrado.", "Dato repetido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ----- CREAR OBJETO -----
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

            if (!chbx_Terminos.Checked)
            {
                MessageBox.Show("Debes aceptar los términos.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            data.estudiantes.Add(nuevo);

            var opciones = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string nuevoJson = JsonSerializer.Serialize(data, opciones);
            File.WriteAllText("Lista de estudiantes.json", nuevoJson);

            MessageBox.Show("Registro guardado correctamente.", "Éxito",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            cargarRegistros();
        }

        // ===== SALIR =====
        private void SubMenu_salir_Click(object sender, EventArgs e)
        {
            var salida = Interaction.MsgBox("¿Esta segura que quiere salir del programa?",
                MsgBoxStyle.YesNo, "Cierre del programa");

            if (salida == MsgBoxResult.Yes)
            {
                esValidoCierre = true;
                this.Close();
            }
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // ===== CTRL + S → Guardar =====
            if (e.Control && e.KeyCode == Keys.S)
            {
                e.SuppressKeyPress = true;

                // Verificar campos obligatorios
                if (!string.IsNullOrWhiteSpace(tbxNombre.Text) &&
                    !string.IsNullOrWhiteSpace(tbxCedula.Text) &&
                    !string.IsNullOrWhiteSpace(tbxUsuario.Text) &&
                    !string.IsNullOrWhiteSpace(tbxPassword.Text) &&
                    !string.IsNullOrWhiteSpace(tbxConfirmacion.Text) &&
                    cbxCarrera.SelectedIndex != -1 &&
                    cbx_Semestre.SelectedIndex != -1 &&
                    (rbMatutina.Checked || rbVespertina.Checked))
                {
                    SubMenu_guardar_Click(sender, e);
                }
                else
                {
                    MessageBox.Show("Completa todos los campos obligatorios antes de guardar.",
                        "Campos incompletos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }


            if (e.Control && e.KeyCode == Keys.N)
            {
                e.SuppressKeyPress = true;
                SubMenu_nuevo_Click(sender, e);
            }


            if (e.Control && e.KeyCode == Keys.L)
            {
                e.SuppressKeyPress = true;
                verRegistroToolStripMenuItem_Click(sender, e);
            }


            if (e.Control && e.KeyCode == Keys.Q)
            {
                var salida = Interaction.MsgBox("¿Esta segura que quiere salir del programa?",
                    MsgBoxStyle.YesNo, "Cierre del programa");

                if (salida == MsgBoxResult.Yes)
                {
                    e.SuppressKeyPress = true;
                    esValidoCierre = true;
                    this.Close();
                }

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!esValidoCierre)
            {
                var salida = Interaction.MsgBox("¿Esta segura que quiere salir del programa?",
                MsgBoxStyle.YesNo, "Cierre del programa");

                if (salida == MsgBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void tbxCedula_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir solo números
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '-')
            {
                e.Handled = true; // Bloquea la tecla
            }
        }

        private void tbxConfirmacion_TextChanged(object sender, EventArgs e)
        {
            ActualizarNombreCompleto();
        }

        private void tbxCedula_TextChanged(object sender, EventArgs e)
        {
            ActualizarNombreCompleto();
        }

        private void ActualizarNombreCompleto()
        {
            if (!string.IsNullOrWhiteSpace(tbxNombre.Text) &&
                !string.IsNullOrWhiteSpace(tbxCedula.Text))
            {
                tbxUsuario.Text = $"{tbxNombre.Text}{tbxCedula.Text.Replace("-", "")}";
            }
            else
            {
                tbxUsuario.Text = "";  // Solo se llena si ambos están completos
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

}
