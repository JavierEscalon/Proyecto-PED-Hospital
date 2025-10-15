using System;

namespace SistemaGestionHospital
{
    public class Paciente
    {
        // propiedades para almacenar la informacion del paciente
        // usamos prop y doble tab para crear estas propiedades rapidamente
        public string DUI { get; set; }
        public string NombreCompleto { get; set; }
        public int Edad { get; set; }
        public string MotivoCita { get; set; }

        // constructor para facilitar la creacion de un nuevo paciente
        public Paciente(string dui, string nombre, int edad, string motivo)
        {
            DUI = dui;
            NombreCompleto = nombre;
            Edad = edad;
            MotivoCita = motivo;
        }
    }
}