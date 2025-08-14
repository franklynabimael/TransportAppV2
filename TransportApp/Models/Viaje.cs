using System.ComponentModel.DataAnnotations;

namespace TransportApp.Models
{
    public class Viaje
    {
        public int Id { get; set; }

        [Required]
        public DateTime HoraSalida { get; set; }

        [Required]
        [StringLength(100)]
        public string Destino { get; set; } = string.Empty;

        public EstadoViaje Estado { get; set; } = EstadoViaje.EnEspera;

        public bool Oculto { get; set; } = false;

        // Para controlar el reinicio diario
        public DateTime UltimaActualizacion { get; set; } = DateTime.Now;
    }

    public enum EstadoViaje
    {
        Abordaje,      
        EnTransito,    
        Finalizado,    
        EnEspera,      
        FueraDeServicio // Estado manual (no afecta la secuencia)
    }
}