using System;
using System.Collections.Generic;
using System.Windows.Forms; // necesario para que funcione MessageBox y Form

namespace SistemaGestionHospital
{
    public partial class Form1 : Form
    {

        // 1 declaramos una variable para nuestro gestor del hospital
        //   esta variable contendra el cerebro de nuestra aplicacion
        private GestorHospital miHospital;

        public Form1()
        {
            InitializeComponent();
            // 2 creamos la instancia del gestor cuando el formulario se inicia
            //   esto enciende el motor de nuestro sistema
            miHospital = new GestorHospital();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // simularemos que un medico de Cardiología esta llamando
            string especialidadQueLlama = "Cardiología";

            // 1 llamamos al metodo principal, este nos devolvera un Paciente o null
            Paciente pacienteLlamado = miHospital.LlamarSiguientePaciente(especialidadQueLlama);

            // 2 verificamos el resultado
            if (pacienteLlamado != null)
            {
                // si NO es nulo, significa que nos devolvio un paciente
                MessageBox.Show("Llamando a paciente: " + pacienteLlamado.NombreCompleto +
                                " (DUI: " + pacienteLlamado.DUI + ")");
            }
            else
            {
                // si es nulo, no habia nadie en la cola de emergencias ni en la de cardiología
                MessageBox.Show("No hay pacientes en espera para " + especialidadQueLlama + ".");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // obtenemos la lista de nombres de especialidades desde nuestro gestor
            List<string> especialidades = miHospital.ObtenerEspecialidades();

            // limpiamos el ComboBox por si acaso y luego añadimos cada especialidad
            cmbEspecialidades.Items.Clear();
            foreach (string esp in especialidades)
            {
                cmbEspecialidades.Items.Add(esp);
            }

            // opcional, estamos haciendo que la primera especialidad aparezca seleccionada por defecto
            if (cmbEspecialidades.Items.Count > 0)
            {
                cmbEspecialidades.SelectedIndex = 0;
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            // 1 --- RECOLECCION DE DATOS ---
            // obtenemos el texto de cada TextBox. .Trim() quita espacios en blanco al inicio y al final
            string dui = txtDUI.Text.Trim();
            string nombre = txtNombre.Text.Trim();
            string motivo = txtMotivo.Text.Trim();

            // 2 --- VALIDACION DE ENTRADAS ---
            // verificamos que ningun campo de texto importante este vacio
            if (string.IsNullOrEmpty(dui) || string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Por favor, ingrese al menos el DUI y el Nombre del paciente.", "Datos Incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // detenemos la ejecucion del metodo si faltan datos
            }

            // intentamos convertir la edad a un numero entero
            if (!int.TryParse(txtEdad.Text.Trim(), out int edad))
            {
                MessageBox.Show("Por favor, ingrese un valor numérico válido para la Edad.", "Dato Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Detenemos si la edad no es un numero
            }

            // 3 --- CREACION DEL OBJETO ---
            // si todas las validaciones pasan, creamos el nuevo objeto Paciente
            Paciente nuevoPaciente = new Paciente(dui, nombre, edad, motivo);

            // 4 --- LLAMADA A LA LÓGICA DEL NEGOCIO ---
            // usamos el metodo que ya habiamos creado en nuestro gestor
            bool fueExitoso = miHospital.RegistrarNuevoPaciente(nuevoPaciente);

            // 5 --- RETROALIMENTACIÓN AL USUARIO Y ACTUALIZACIÓN DE UI ---
            if (fueExitoso)
            {
                MessageBox.Show("¡Paciente registrado con éxito!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LimpiarCamposRegistro();
                ActualizarListaPacientes();
            }
            else
            {
                MessageBox.Show("Ya existe un paciente con el DUI ingresado.", "Registro Fallido", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- METODOS AUXILIARES PARA LA UI ---

        private void LimpiarCamposRegistro()
        {
            txtDUI.Clear();
            txtNombre.Clear();
            txtEdad.Clear();
            txtMotivo.Clear();
        }

        // este metodo actualiza la lista visual de pacientes registrados
        private void ActualizarListaPacientes()
        {
            // Limpiamos la lista visual
            lstPacientesRegistrados.Items.Clear();

            // Obtenemos la lista actualizada de pacientes desde el gestor
            List<Paciente> pacientes = miHospital.ObtenerPacientesRegistrados();

            // Recorremos la lista de objetos Paciente y añadimos su información a la lista visual
            foreach (Paciente p in pacientes)
            {
                // Mostramos el DUI y el Nombre para una facil identificacion
                lstPacientesRegistrados.Items.Add($"{p.DUI} - {p.NombreCompleto}");
            }
        }

        // este metodo actualiza el visualizador de colas
        private void ActualizarVisualizadorColas()
        {
            lstVisualizadorColas.Items.Clear();
            Dictionary<string, int> estadoColas = miHospital.ObtenerEstadoDeColas();

            foreach (var item in estadoColas)
            {
                lstVisualizadorColas.Items.Add($"{item.Key}: {item.Value} paciente(s) en espera");
            }
        }

        // este metodo actualiza el visualizador de colas
        private void btnAsignar_Click(object sender, EventArgs e)
        {
            // 1 --- VALIDACIÓN DE SELECCION ---
            // Verificamos si el usuario ha seleccionado un paciente de la lista
            // SelectedIndex devuelve -1 si no hay nada seleccionado
            if (lstPacientesRegistrados.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, seleccione un paciente de la lista.", "Selección Requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2 --- OBTENCION DE DATOS ---
            // obtenemos el paciente seleccionado. Primero necesitamos la lista completa
            List<Paciente> pacientes = miHospital.ObtenerPacientesRegistrados();
            Paciente pacienteSeleccionado = pacientes[lstPacientesRegistrados.SelectedIndex];

            string especialidadSeleccionada = cmbEspecialidades.SelectedItem.ToString();
            bool esEmergencia = chkEmergencia.Checked;
            string resultado;

            // 3 --- LLAMADA A LA LOGICA DEL NEGOCIO ---
            // decidimos a que metodo llamar basado en si la casilla de emergencia esta marcada
            if (esEmergencia)
            {
                resultado = miHospital.AsignarPacienteAEmergencia(pacienteSeleccionado);
            }
            else
            {
                resultado = miHospital.AsignarPacienteACola(pacienteSeleccionado, especialidadSeleccionada);
            }

            // 4 --- RETROALIMENTACION Y ACTUALIZACION ---
            MessageBox.Show(resultado, "Asignación", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ActualizarVisualizadorColas();
        }
    }
}