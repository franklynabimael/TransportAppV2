using TransportApp.Models;

namespace TransportApp.Helpers
{
    public static class EstadoViajeHelper
    {
        public static string GetRowClass(EstadoViaje estado) => estado switch
        {
            EstadoViaje.EnTransito => "table-warning",
            EstadoViaje.Abordaje => "table-success",
            EstadoViaje.FueraDeServicio => "table-danger",
            EstadoViaje.Finalizado => "table-secondary",
            _ => ""
        };

        public static string GetBadgeClass(EstadoViaje estado) => estado switch
        {
            EstadoViaje.Abordaje => "bg-success",
            EstadoViaje.EnTransito => "bg-warning text-dark",
            EstadoViaje.Finalizado => "bg-secondary",
            EstadoViaje.EnEspera => "bg-info",
            EstadoViaje.FueraDeServicio => "bg-danger",
            _ => "bg-secondary"
        };

        public static string GetIconClass(EstadoViaje estado) => estado switch
        {
            EstadoViaje.Abordaje => "fas fa-users",
            EstadoViaje.EnTransito => "fas fa-route",
            EstadoViaje.Finalizado => "fas fa-check-circle",
            EstadoViaje.EnEspera => "fas fa-clock",
            EstadoViaje.FueraDeServicio => "fas fa-exclamation-triangle",
            _ => "fas fa-question"
        };

        public static string GetEstadoTexto(EstadoViaje estado) => estado switch
        {
            EstadoViaje.Abordaje => "Abordaje",
            EstadoViaje.EnTransito => "En Tránsito",
            EstadoViaje.Finalizado => "Finalizado",
            EstadoViaje.EnEspera => "En Espera",
            EstadoViaje.FueraDeServicio => "Fuera de Servicio",
            _ => "Desconocido"
        };
    }
}
