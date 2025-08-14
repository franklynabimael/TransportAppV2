using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TransportApp.Models;
using TransportApp.Services;

namespace TransportApp.Pages
{
    //[Authorize] // Protegida
    public class CreateModel : PageModel
    {
        private readonly ViajeService _viajeService;

        public CreateModel(ViajeService viajeService)
        {
            _viajeService = viajeService;
        }

        [BindProperty, Required]
        public DateTime HoraSalida { get; set; } = DateTime.Today.AddHours(DateTime.Now.Hour + 1);

        [BindProperty, Required]
        public string DestinoSeleccionado { get; set; } = string.Empty;

        public List<string> DestinosDisponibles { get; set; } = new();
        public List<Viaje> ViajesDelDia { get; set; } = new();
        public string Mensaje { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            DestinosDisponibles = _viajeService.GetDestinosDisponibles();
            ViajesDelDia = await _viajeService.GetTodosLosViajesAsync();
        }

        public async Task<IActionResult> OnPostCrearAsync()
        {
            DestinosDisponibles = _viajeService.GetDestinosDisponibles();
            ViajesDelDia = await _viajeService.GetTodosLosViajesAsync();

            if (!ModelState.IsValid)
            {
                Error = "Por favor completa todos los campos correctamente.";
                return Page();
            }

            var resultado = await _viajeService.CrearViajeAsync(HoraSalida, DestinoSeleccionado);

            if (resultado)
            {
                Mensaje = $"Viaje agregado exitosamente para las {HoraSalida:HH:mm} hacia {DestinoSeleccionado}";
                // Limpiar el formulario
                HoraSalida = DateTime.Today.AddHours(DateTime.Now.Hour + 1);
                DestinoSeleccionado = string.Empty;
                // Refrescar la lista
                ViajesDelDia = await _viajeService.GetTodosLosViajesAsync();
            }
            else
            {
                Error = "Ya existe un viaje programado para esa hora. Elige otra hora.";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostFueraDeServicioAsync(int idViaje)
        {
            DestinosDisponibles = _viajeService.GetDestinosDisponibles();

            var resultado = await _viajeService.CambiarEstadoAFueraDeServicioAsync(idViaje);

            if (resultado)
            {
                Mensaje = "Viaje puesto fuera de servicio exitosamente.";
            }
            else
            {
                Error = "No se pudo cambiar el estado del viaje.";
            }

            ViajesDelDia = await _viajeService.GetTodosLosViajesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostReactivarAsync(int idViaje)
        {
            DestinosDisponibles = _viajeService.GetDestinosDisponibles();

            var resultado = await _viajeService.ReactivarViajeAsync(idViaje);

            if (resultado)
            {
                Mensaje = "Viaje reactivado exitosamente.";
            }
            else
            {
                Error = "No se pudo reactivar el viaje.";
            }

            ViajesDelDia = await _viajeService.GetTodosLosViajesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostEliminarAsync(int idViaje)
        {
            DestinosDisponibles = _viajeService.GetDestinosDisponibles();

            var resultado = await _viajeService.EliminarViajeAsync(idViaje);

            if (resultado)
            {
                Mensaje = "Viaje eliminado exitosamente.";
            }
            else
            {
                Error = "No se pudo eliminar el viaje.";
            }

            ViajesDelDia = await _viajeService.GetTodosLosViajesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostReiniciarEstadosAsync()
        {
            DestinosDisponibles = _viajeService.GetDestinosDisponibles();

            var resultado = await _viajeService.ReiniciarTodosLosEstadosAsync();

            if (resultado)
            {
                Mensaje = "Todos los estados han sido reiniciados. El primer viaje disponible está ahora en abordaje.";
            }
            else
            {
                Error = "No se pudieron reiniciar los estados.";
            }

            ViajesDelDia = await _viajeService.GetTodosLosViajesAsync();
            return Page();
        }
    }
}