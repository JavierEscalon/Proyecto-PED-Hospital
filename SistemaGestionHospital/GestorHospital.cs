using System;
using System.Collections.Generic; // para usar list, dictionary y queue

namespace SistemaGestionHospital
{
    public class GestorHospital
    {
        // --- ESTRUCTURAS DE DATOS PRINCIPALES ---
        // usamos private para proteger los datos, encapsulación

        // 1 registro maestro de todos los pacientes
        private List<Paciente> listaMaestraPacientes;

        // 2 diccionario para gestionar las colas por especialidad
        private Dictionary<string, Queue<Paciente>> colasPorEspecialidad;

        // 3 cola dedicada para emergencias
        private Queue<Paciente> colaEmergencias;

        // lectura segura de las claves (los nombres de las especialidades) desde fuera de la clase
        public List<string> ObtenerEspecialidades()
        {
            // Devolvemos una nueva lista con las claves del diccionario.
            return new List<string>(colasPorEspecialidad.Keys);
        }

        // lectura segura de la lista maestra de pacientes desde fuera de la clase
        public List<Paciente> ObtenerPacientesRegistrados()
        {
            return listaMaestraPacientes;
        }

        // --- CONSTRUCTOR ---
        // este metodo se ejecuta automaticamente al crear un objeto GestorHospital
        public GestorHospital()
        {
            // inicializamos las estructuras de datos para que no esten vacias
            listaMaestraPacientes = new List<Paciente>();
            colasPorEspecialidad = new Dictionary<string, Queue<Paciente>>();
            colaEmergencias = new Queue<Paciente>();

            // prepoblamos el hospital con algunas especialidades
            InicializarEspecialidades();
        }

        private void InicializarEspecialidades()
        {
            // agregamos las especialidades que manejara el hospital
            // cada especialidad tiene su propia cola de pacientes vacia
            colasPorEspecialidad.Add("Medicina General", new Queue<Paciente>());
            colasPorEspecialidad.Add("Cardiología", new Queue<Paciente>());
            colasPorEspecialidad.Add("Pediatría", new Queue<Paciente>());
            colasPorEspecialidad.Add("Dermatología", new Queue<Paciente>());
        }

        // --- METODOS PUBLICOS, las acciones que podra hacer nuestro sistema ---

        // añadir un paciente a la listaMaestraPacientes
        // el metodo devuelve true si el registro fue exitoso y false si el paciente ya existe
        public bool RegistrarNuevoPaciente(Paciente nuevoPaciente)
        {
            // 1 verificamos si ya existe un paciente con el mismo DUI
            // recorremos la listaMaestraPacientes para comparar cada paciente
            foreach (Paciente p in listaMaestraPacientes)
            {
                if (p.DUI == nuevoPaciente.DUI)
                {
                    // si encontramos un DUI que coincida, el paciente ya existe
                    // no lo agregamos y devolvemos false.
                    return false;
                }
            }

            // 2 si el bucle termina y no encontramos duplicados, agregamos el nuevo paciente
            listaMaestraPacientes.Add(nuevoPaciente);

            // 3 devolvemos true para indicar que la operacion fue un exito
            return true;
        }

        // metodo encontrar un paciente por DUI y añadirlo a la cola correcta
        public string AsignarPacienteACola(Paciente paciente, string especialidad)
        {
            // 1 validamos que el paciente no sea nulo
            if (paciente == null)
            {
                return "Error: No se ha seleccionado ningún paciente.";
            }

            // 2 validamos si la especialidad existe
            if (!colasPorEspecialidad.ContainsKey(especialidad))
            {
                return "Error: La especialidad '" + especialidad + "' no existe.";
            }

            // 3 di todo es correcto, añadimos el paciente a la cola
            colasPorEspecialidad[especialidad].Enqueue(paciente);

            // 4 devolvemos un mensaje de exito
            return "Paciente " + paciente.NombreCompleto + " asignado a la cola de " + especialidad + ".";
        }

        // metodo para manejar emergencias
        public string AsignarPacienteAEmergencia(Paciente paciente)
        {
            if (paciente == null)
            {
                return "Error: No se ha seleccionado ningún paciente.";
            }

            colaEmergencias.Enqueue(paciente);
            return "Paciente " + paciente.NombreCompleto + " asignado a la cola de EMERGENCIAS.";
        }

        // atender al proximo paciente, dando prioridad a emergencias
        public Paciente LlamarSiguientePaciente(string especialidad)
        {
            // 1 PRIMERA REGLA: hay alguien en la cola de emercias?
            // la propiedad .Count nos dice cuantos elementos hay en la cola
            if (colaEmergencias.Count > 0)
            {
                // si hay, sacamos al paciente de la cola de emergencias y lo retornamos
                return colaEmergencias.Dequeue();
            }

            // 2 SEGUNDA REGLA: si no hay emergencias, revisamos la cola de la especialidad
            // primero validamos que la especialidad exista y que su cola no este vacia
            if (colasPorEspecialidad.ContainsKey(especialidad) && colasPorEspecialidad[especialidad].Count > 0)
            {
                // si hay, sacamos al paciente de esa cola y lo retornamos
                return colasPorEspecialidad[especialidad].Dequeue();
            }

            // 3 si llegamos hasta aqui, significa que no habia nadie en ninguna cola relevante
            // retornamos null para indicar que no hay pacientes en espera
            return null;
        }

        // metodo para obtener el estado actual de todas las colas
        public Dictionary<string, int> ObtenerEstadoDeColas()
        {
            var estado = new Dictionary<string, int>();
            // agregamos el conteo de la cola de emergencias primero.
            estado.Add("EMERGENCIAS", colaEmergencias.Count);

            // agregamos el conteo para cada especialidad.
            foreach (var cola in colasPorEspecialidad)
            {
                estado.Add(cola.Key, cola.Value.Count);
            }
            return estado;
        }
    }
}