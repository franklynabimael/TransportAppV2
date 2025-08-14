using Microsoft.AspNetCore.Mvc.RazorPages;
using TransportApp.Models;
using TransportApp.Services;

namespace TransportApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ViajeService _viajeService;

        public IndexModel(ViajeService viajeService)
        {
            _viajeService = viajeService;
        }

        public List<Viaje> Viajes { get; set; } = new();

        public async Task OnGetAsync()
        {
            Viajes = await _viajeService.GetViajesVisiblesAsync();
        }
    }
}