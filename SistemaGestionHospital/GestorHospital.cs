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

        // encontrar un paciente por DUI y añadirlo a la cola correcta
        public string AsignarPacienteACola(string duiPaciente, string especialidad)
        {
            // 1 buscamos al paciente en la lista maestra
            Paciente pacienteEncontrado = null; // variable para guardar el paciente si lo encontramos
            foreach (Paciente p in listaMaestraPacientes)
            {
                if (p.DUI == duiPaciente)
                {
                    pacienteEncontrado = p;
                    break; // rompemos el bucle porque ya lo encontramos
                }
            }

            // 2 validamos si encontramos al paciente
            if (pacienteEncontrado == null)
            {
                return "Error: Paciente con DUI " + duiPaciente + " no encontrado.";
            }

            // 3 validamos si la especialidad existe en nuestro diccionario
            if (!colasPorEspecialidad.ContainsKey(especialidad))
            {
                return "Error: La especialidad '" + especialidad + "' no existe.";
            }

            // 4 si todo es correcto, añadimos el paciente a la cola correspondiente
            colasPorEspecialidad[especialidad].Enqueue(pacienteEncontrado);

            // 5 devolvemos un mensaje de exito
            return "Paciente " + pacienteEncontrado.NombreCompleto + " asignado a la cola de " + especialidad + ".";
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
    }
}