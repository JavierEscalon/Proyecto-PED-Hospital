using System;
using System.Windows.Forms; // necesario para que funcione MessageBox y Form

namespace SistemaGestionHospital
{
    public partial class Form1 : Form
    {

        // ====================================================================
        // INICIO: CÓDIGO QUE AGREGAMOS
        // ====================================================================

        // 1 declaramos una variable para nuestro gestor del hospital
        //   esta variable contendra el cerebro de nuestra aplicación
        private GestorHospital miHospital;

        public Form1()
        {
            InitializeComponent();
            // 2 creamos la instancia del gestor cuando el formulario se inicia
            //   esto enciende el motor de nuestro sistema
            miHospital = new GestorHospital();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 3 este codigo se ejecuta cada vez que se hace clic en el boton

            // a creamos un paciente de prueba
            Paciente pacienteDePrueba = new Paciente("12345678-9", "Juan Pérez", 30, "Consulta General");

            // b Intentamos registrarlo usando el metodo de nuestro gestor
            bool fueExitoso = miHospital.RegistrarNuevoPaciente(pacienteDePrueba);

            // c mostramos un mensaje para saber el resultado
            if (fueExitoso)
            {
                MessageBox.Show("¡Paciente 'Juan Pérez' registrado con éxito!");
            }
            else
            {
                MessageBox.Show("ERROR: El paciente 'Juan Pérez' ya existe en el registro.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // --- CASO DE PRUEBA 1: Asignación exitosa ---

            // El DUI debe ser el mismo que registramos con el primer botón.
            string duiDePrueba = "12345678-9";
            string especialidadDePrueba = "Cardiología";

            // Llamamos al nuevo método de nuestro gestor.
            string resultado = miHospital.AsignarPacienteACola(duiDePrueba, especialidadDePrueba);

            // Mostramos el mensaje devuelto por el método.
            MessageBox.Show(resultado);


            // --- CASO DE PRUEBA 2 (Opcional): Paciente no encontrado ---

            // string resultadoFallo = miHospital.AsignarPacienteACola("00000000-0", "Pediatría");
            // MessageBox.Show(resultadoFallo);
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

        // ====================================================================
        // FIN: CÓDIGO QUE AGREGAMOS
        // ====================================================================
    }
}