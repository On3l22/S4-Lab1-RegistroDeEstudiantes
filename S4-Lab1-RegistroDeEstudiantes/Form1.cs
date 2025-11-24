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

        //################ CONSTRUCTOR ################
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
        }//################ FIN CONSTRUCTOR ################

        // -- EVENTO PARA ENTER --------------------------------
        // Permite saltar al siguiente control al presionar ENTER
        // ------------------------------------------------------
        private void SaltarConEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                this.SelectNextControl((Control)sender, true, true, true, true);
            }
        }

        // -- EVENTO AL MOSTRAR EL FORMULARIO -----------------------------------------
        // Hecho para solicitar un codigo de administrador antes de permitir el acceso
        //-----------------------------------------------------------------------------
        private void Form1_Shown(object sender, EventArgs e)
        {
            while (true)//Contrla el bucle hasta que se ingrese el codigo correcto o se cancele
            {
                valor = Interaction.InputBox("Ingrese el codigo de administrador:", "Inicio de sesión");

                if (valor != codigo)//si el codigo es incorrecto
                {
                    if (valor == "")//si selecciona "Cancelar"
                    {
                        MessageBox.Show("Sesión cancelada", "Cerrando programa",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();//sale del programa
                    }
                    else
                    {
                        MessageBox.Show("Advertencia: La clave es incorrecta", "Acceso denegado",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else// si el codigo es correcto
                {
                    Interaction.MsgBox("Acceso Permitido. Bienvenido al sistema de registro.",
                        MsgBoxStyle.OkOnly, "Registro correcto");

                    tbxNombre.Focus();
                    break;//sale del buble
                }
            }
        }

        //----------------------
        // METODO CARGAR LISTAS
        //----------------------
        private void cargarListas()
        {
            string json = File.ReadAllText("carrerasUniversitarias.json"); // Lee el archivo JSON
            Carreras data = JsonSerializer.Deserialize<Carreras>(json);// Deserializa el contenido JSON a un objeto Carreras

            foreach (var carrera in data.carreras_unificadas)// Agrega cada carrera al ComboBox
            {
                cbxCarrera.Items.Add(carrera);
            }

            for (int i = 0; i < 11; i++)// Agrega los semestres al ComboBox
            {
                cbx_Semestre.Items.Add((i + 1).ToString());
            }

            cargarRegistros();//Carga los registros guardados en el DataGridView
        }

        //-------------------------
        // METODO CARGAR REGISTROS
        //-------------------------
        private void cargarRegistros()
        {
            if (!File.Exists("Lista de estudiantes.json"))
                return; // Si el archivo no existe, sale del método
            string json = File.ReadAllText("Lista de estudiantes.json");// Lee el contenido del archivo JSON
            ListaDeEstudiantes datos = JsonConvert.DeserializeObject<ListaDeEstudiantes>(json);// Deserializa el contenido JSON a un objeto ListaDeEstudiantes

            dataGridView1.DataSource = datos.estudiantes;// Asigna la lista de estudiantes como fuente de datos del DataGridView

            // Ocultar contraseña
            if (dataGridView1.Columns["password"] != null)
            {
                dataGridView1.Columns["password"].Visible = false;
            }
        }

        //---------------------
        // VER ÚLTIMO REGISTRO 
        //---------------------
        private void verRegistroToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!File.Exists("Lista de estudiantes.json"))// Verifica si el archivo existe
            {
                MessageBox.Show("No hay registros guardados.", "Sin datos",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string json = File.ReadAllText("Lista de estudiantes.json");// Lee el contenido del archivo JSON
            if (string.IsNullOrWhiteSpace(json))// Verifica si el contenido está vacío
            {
                MessageBox.Show("El archivo de registros está vacío.", "Sin datos",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ListaDeEstudiantes data = JsonSerializer.Deserialize<ListaDeEstudiantes>(json);// Deserializa el contenido JSON a un objeto ListaDeEstudiantes
            if (data == null || data.estudiantes == null || data.estudiantes.Count == 0)
            {
                MessageBox.Show("No hay estudiantes guardados.", "Sin datos",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Estudiante ultimo = data.estudiantes.Last();// Obtiene el último estudiante registrado

            // Rellena los campos del formulario con los datos del último estudiante
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

        //-- EVENTO DE MENU [NUEVO REGISTRO] -------------------------
        // Limpia todos los campos para un nuevo registro
        //------------------------------------------------------------
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

        //-- EVENTO DE MENU [GUARDAR] --------------------------------
        // Valida y guarda el registro en un archivo JSON
        //------------------------------------------------------------
        private void SubMenu_guardar_Click(object sender, EventArgs e)
        {
            // Guarda el nombre de usuario generado automáticamente
            tbxUsuario.Text = tbxNombre.Text.Replace(" ","_") + tbxCedula.Text.Replace("-", "");

            // ----- Validaciones de las entradas -----------
            if (string.IsNullOrWhiteSpace(tbxNombre.Text) ||
                string.IsNullOrWhiteSpace(tbxCedula.Text) ||
                string.IsNullOrWhiteSpace(tbxUsuario.Text) ||
                string.IsNullOrWhiteSpace(tbxPassword.Text) ||
                string.IsNullOrWhiteSpace(tbxConfirmacion.Text) ||
                cbxCarrera.SelectedIndex == -1 ||
                cbx_Semestre.SelectedIndex == -1 ||
                (!rbMatutina.Checked && !rbVespertina.Checked))
            {
                MessageBox.Show("Uno de los campos está vacío.", "Error",//Muestra un mensaje de error
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ----- Validar que el nombre no contenga símbolos -----
            foreach (char c in tbxNombre.Text)
            {
                if (!char.IsLetter(c) && !char.IsWhiteSpace(c))// Verifica si el carácter no es una letra ni un espacio
                {
                    MessageBox.Show("El nombre no puede contener símbolos.", "Dato ilógico",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);//Muestra un mensaje de advertencia
                    return;
                }
            }
            // ----- Validar longitud del nombre --------------------
            if (tbxNombre.Text.Length > 30)
            {
                MessageBox.Show("El nombre excede el límite permitido (30 caracteres).",
                    "Dato ilógico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // ----- Validar formato de cédula ----------------------
            string patron = @"^\d{1,2}-\d{1,4}-\d{1,6}$";
            if (!Regex.IsMatch(tbxCedula.Text, patron))
            {
                MessageBox.Show("La cédula debe tener formato válido como 2-755-39 o 02-0755-000039",
                    "Dato ilógico", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // ----- Validar coincidencia de contraseñas -----------
            if (tbxPassword.Text != tbxConfirmacion.Text)
            {
                MessageBox.Show("La contraseña y la confirmación no coinciden.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // ### CARGAR LISTA DE ESTUDIANTES ###
            ListaDeEstudiantes data;// Declarar la variable data
            if (File.Exists("Lista de estudiantes.json"))// Verifica si el archivo existe
            {
                try
                {
                    string json = File.ReadAllText("Lista de estudiantes.json");//Leer el Json

                    if (string.IsNullOrWhiteSpace(json))// Verifica si el contenido está vacío
                    {
                        data = new ListaDeEstudiantes() { estudiantes = new List<Estudiante>() };//Si esta vacio crea una nueva
                    }
                    else
                    {
                        data = JsonSerializer.Deserialize<ListaDeEstudiantes>(json);//sino deserializa el json

                        if (data == null || data.estudiantes == null)// Verifica si la deserialización fue exitosa
                            data = new ListaDeEstudiantes() { estudiantes = new List<Estudiante>() };//Si no crea una nueva
                    }
                }
                catch// Captura errores de deserialización
                {
                    MessageBox.Show("Archivo dañado. Se creará uno nuevo.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    data = new ListaDeEstudiantes() { estudiantes = new List<Estudiante>() };//Crea una nueva
                }
            }
            else// Si el archivo no existe, crea una nueva lista
            {
                data = new ListaDeEstudiantes() { estudiantes = new List<Estudiante>() };
            }

            // ----- VERIFICA CEDULA EXISTENTE -----
            bool cedulaExiste = data.estudiantes.Any(e => e.cedula == tbxCedula.Text);
            if (cedulaExiste)
            {
                MessageBox.Show("La cédula ya está registrada.", "Dato repetido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);//Muestra un mensaje de advertencia
                return;
            }

            // ----- VERIFICA USUARIO EXISTENTE -----
            bool usuarioExiste = data.estudiantes.Any(e => e.usuario == tbxUsuario.Text);
            if (usuarioExiste)
            {
                MessageBox.Show("El usuario ya está registrado.", "Dato repetido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);//Muestra un mensaje de advertencia
                return;
            }

            //############# AGREGAR NUEVO ESTUDIANTE #############
            Estudiante nuevo = new Estudiante()//crea una nueva instancia de estudiante
            {
                //llena los campos con los datos del formulario
                nombre = tbxNombre.Text,
                cedula = tbxCedula.Text,
                carrera = cbxCarrera.SelectedItem.ToString(),
                semestre = int.Parse(cbx_Semestre.SelectedItem.ToString()),
                jornada = rbMatutina.Checked ? "Matutina" : "Vespertina",
                usuario = tbxUsuario.Text,
                password = tbxPassword.Text
            };

            if (!chbx_Terminos.Checked)//Verifica si se aceptaron los términos
            {
                MessageBox.Show("Debes aceptar los términos.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);//muestra un mensaje de advertencia
                return;
            }

            data.estudiantes.Add(nuevo);//agrega el nuevo estudiante a la lista
            var opciones = new JsonSerializerOptions// Configura las opciones de serialización JSON
            {
                WriteIndented = true
            };

            string nuevoJson = JsonSerializer.Serialize(data, opciones);// Serializa la lista actualizada a JSON
            File.WriteAllText("Lista de estudiantes.json", nuevoJson);// Escribe el JSON en el archivo

            MessageBox.Show("Registro guardado correctamente.", "Éxito",
                MessageBoxButtons.OK, MessageBoxIcon.Information);// Muestra un mensaje de éxito

            cargarRegistros();// Recarga los registros en el DataGridView
        }

        //-- EVENTO DE MENU [SALIR] ----------------------------------
        // Permite salir del programa con confirmacion
        //------------------------------------------------------------
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

        /////////////////////////// ATAJO DE TECLADO ///////////////////////////
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

            // ===== CTRL + N → Nuevo Registro =====
            if (e.Control && e.KeyCode == Keys.N)
            {
                e.SuppressKeyPress = true;
                SubMenu_nuevo_Click(sender, e);
            }

            // ===== CTRL + L → Ver Último Registro =====
            if (e.Control && e.KeyCode == Keys.L)
            {
                e.SuppressKeyPress = true;
                verRegistroToolStripMenuItem_Click(sender, e);
            }

            // ===== CTRL + Q → Salir =====
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

        ////////////////////////// EVENTO FORM CLOSING ////////////////////////
        // Permite confirmar el cierre del formulario al darle click en la X
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!esValidoCierre)//soluciona error al cerrar el programa 
            {
                var salida = Interaction.MsgBox("¿Esta segura que quiere salir del programa?",
                MsgBoxStyle.YesNo, "Cierre del programa");

                if (salida == MsgBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        //////////////////////// EVENTOS TBX CEDULA Y CONFIRMACION ////////////////////////
        /// Permite solo la entrada de números en el textbox de cédula
        private void tbxCedula_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Permitir solo números
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '-')
            {
                e.Handled = true; // Bloquea la tecla
            }
        }

        ////// Actualiza el nombre de usuario automáticamente al cambiar nombre o cédula
        /// dentro del campo de usuario //////
        private void tbxConfirmacion_TextChanged(object sender, EventArgs e)
        {
            ActualizarNombreCompleto();// Llama al método para actualizar el nombre de usuario
        }

        private void tbxCedula_TextChanged(object sender, EventArgs e)
        {
            ActualizarNombreCompleto();// Llama al método para actualizar el nombre de usuario
        }

        private void ActualizarNombreCompleto()
        {
            if (!string.IsNullOrWhiteSpace(tbxNombre.Text) &&
                !string.IsNullOrWhiteSpace(tbxCedula.Text))// Verifica que ambos campos no estén vacíos
            {
                tbxUsuario.Text = $"{tbxNombre.Text.Replace(" ", "_")}{tbxCedula.Text.Replace("-", "")}";// Genera el nombre de usuario
            }
            else
            {
                tbxUsuario.Text = "";  // Solo se llena si ambos están completos
            }
        }
    }
}
