using Microsoft.EntityFrameworkCore;
using TransportApp.Data;
using TransportApp.Models;

namespace TransportApp.Services
{
    public class ViajeService
    {
        private readonly ApplicationDbContext _context;
        private readonly int _tiempoTransitoMinutos = 5; // Configurable - duración del tránsito
        private readonly int _tiempoAnticipacionAbordaje = 10; // Configurable - minutos antes para abordaje

        public ViajeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Viaje>> GetViajesVisiblesAsync()
        {
            var ahora = DateTime.Now;

            // Verificar si es medianoche (reiniciar automáticamente)
            await VerificarReinicioDiarioAsync(ahora);

            var viajes = await _context.Viajes
                .OrderBy(v => v.HoraSalida)
                .ToListAsync();

            // Actualizar estados basados en tiempo
            await ActualizarEstadosBasadosEnTiempoAsync(viajes, ahora);

            return viajes.Where(v => !v.Oculto).ToList();
        }

        public async Task<List<Viaje>> GetTodosLosViajesAsync()
        {
            var ahora = DateTime.Now;
            await VerificarReinicioDiarioAsync(ahora);

            var viajes = await _context.Viajes
                .OrderBy(v => v.HoraSalida)
                .ToListAsync();

            await ActualizarEstadosBasadosEnTiempoAsync(viajes, ahora);

            return viajes;
        }

        private async Task VerificarReinicioDiarioAsync(DateTime ahora)
        {
            // Verificar si es un nuevo día (después de medianoche 00:00)
            var hoy = ahora.Date;
            var ultimaActualizacion = await _context.Viajes
                .Where(v => v.UltimaActualizacion.Date < hoy)
                .AnyAsync();

            if (ultimaActualizacion)
            {
                // REINICIO AUTOMÁTICO A MEDIANOCHE
                var todosLosViajes = await _context.Viajes.ToListAsync();

                foreach (var viaje in todosLosViajes)
                {
                    // Resetear completamente para el nuevo día
                    viaje.Estado = EstadoViaje.EnEspera; // Todos empiezan en espera
                    viaje.Oculto = false; // Mostrar todos los viajes nuevamente
                    viaje.UltimaActualizacion = ahora;

                    // Ajustar la hora de salida para el día actual
                    viaje.HoraSalida = hoy.Add(viaje.HoraSalida.TimeOfDay);
                }

                await _context.SaveChangesAsync();
            }
        }

        private async Task ActualizarEstadosBasadosEnTiempoAsync(List<Viaje> viajes, DateTime ahora)
        {
            bool cambiosRealizados = false;
            var viajesOrdenados = viajes
                .Where(v => v.Estado != EstadoViaje.FueraDeServicio) // No modificar los fuera de servicio
                .OrderBy(v => v.HoraSalida)
                .ToList();

            foreach (var viaje in viajesOrdenados)
            {
                var estadoAnterior = viaje.Estado;

                // LÓGICA DE ESTADOS BASADA EN TIEMPO:

                // 1. FINALIZADO: Si ya pasó el tiempo de tránsito
                if (ahora > viaje.HoraSalida.AddMinutes(_tiempoTransitoMinutos))
                {
                    if (viaje.Estado == EstadoViaje.EnTransito)
                    {
                        viaje.Estado = EstadoViaje.Finalizado;
                        viaje.Oculto = true; // Ocultar los finalizados
                    }
                }

                // 2. EN TRÁNSITO: Desde la hora de salida hasta +10 minutos
                else if (ahora >= viaje.HoraSalida &&
                         ahora <= viaje.HoraSalida.AddMinutes(_tiempoTransitoMinutos))
                {
                    viaje.Estado = EstadoViaje.EnTransito;
                }

                // 3. ABORDAJE: Desde 20 minutos antes hasta la hora de salida
                else if (ahora >= viaje.HoraSalida.AddMinutes(-_tiempoAnticipacionAbordaje) &&
                         ahora < viaje.HoraSalida)
                {
                    viaje.Estado = EstadoViaje.Abordaje;
                }

                // 4. EN ESPERA: Más de 20 minutos antes de la hora de salida
                else if (ahora < viaje.HoraSalida.AddMinutes(-_tiempoAnticipacionAbordaje))
                {
                    viaje.Estado = EstadoViaje.EnEspera;
                }

                if (estadoAnterior != viaje.Estado)
                {
                    cambiosRealizados = true;
                }
            }

            if (cambiosRealizados)
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> CrearViajeAsync(DateTime horaSalida, string destino)
        {
            // Crear hora completa con solo hora y minutos (reutilizable para cualquier día)
            var nuevaHora = DateTime.Today.AddHours(horaSalida.Hour).AddMinutes(horaSalida.Minute);

            // Verificar si ya existe un viaje en esa hora exacta
            var viajeExistente = await _context.Viajes
                .FirstOrDefaultAsync(v => v.HoraSalida.TimeOfDay == nuevaHora.TimeOfDay);

            if (viajeExistente != null)
            {
                return false; // Ya existe un viaje en esa hora
            }

            var viaje = new Viaje
            {
                HoraSalida = nuevaHora,
                Destino = destino,
                Estado = EstadoViaje.EnEspera, // Siempre empiezan en espera
                UltimaActualizacion = DateTime.Now
            };

            _context.Viajes.Add(viaje);
            await _context.SaveChangesAsync();

            return true;
        }



        public async Task<bool> CambiarEstadoAFueraDeServicioAsync(int id)
        {
            var viaje = await _context.Viajes.FindAsync(id);
            if (viaje == null) return false;

            viaje.Estado = EstadoViaje.FueraDeServicio;
            viaje.UltimaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReactivarViajeAsync(int id)
        {
            var viaje = await _context.Viajes.FindAsync(id);
            if (viaje == null) return false;

            // Restaurar el viaje basado en la hora actual
            var ahora = DateTime.Now;

            // Determinar el estado correcto según la hora
            if (ahora > viaje.HoraSalida.AddMinutes(_tiempoTransitoMinutos))
            {
                viaje.Estado = EstadoViaje.Finalizado;
                viaje.Oculto = true;
            }
            else if (ahora >= viaje.HoraSalida && ahora <= viaje.HoraSalida.AddMinutes(_tiempoTransitoMinutos))
            {
                viaje.Estado = EstadoViaje.EnTransito;
                viaje.Oculto = false;
            }
            else if (ahora >= viaje.HoraSalida.AddMinutes(-_tiempoAnticipacionAbordaje) && ahora < viaje.HoraSalida)
            {
                viaje.Estado = EstadoViaje.Abordaje;
                viaje.Oculto = false;
            }
            else
            {
                viaje.Estado = EstadoViaje.EnEspera;
                viaje.Oculto = false;
            }

            viaje.UltimaActualizacion = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarViajeAsync(int id)
        {
            var viaje = await _context.Viajes.FindAsync(id);
            if (viaje == null) return false;

            _context.Viajes.Remove(viaje);
            await _context.SaveChangesAsync();
            return true;
        }

        public List<string> GetDestinosDisponibles()
        {
            return new List<string>
            {
                "AWC",
                "OSTOMY"
            };
        }

        public async Task<bool> ReiniciarTodosLosEstadosAsync()
        {
            var ahora = DateTime.Now;
            var hoy = ahora.Date;
            var todosLosViajes = await _context.Viajes.ToListAsync();

            foreach (var viaje in todosLosViajes)
            {
                // Reiniciar completamente para el día actual
                viaje.Estado = EstadoViaje.EnEspera;
                viaje.Oculto = false;
                viaje.UltimaActualizacion = ahora;

                // Actualizar la hora de salida para el día actual
                viaje.HoraSalida = hoy.Add(viaje.HoraSalida.TimeOfDay);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // Métodos para obtener configuración
        public int GetTiempoTransito() => _tiempoTransitoMinutos;
        public int GetTiempoAnticipacionAbordaje() => _tiempoAnticipacionAbordaje;

        public async Task<Viaje?> GetViajeAsync(int id)
        {
            return await _context.Viajes.FindAsync(id);
        }

        public async Task ActualizarViajeAsync(Viaje viaje)
        {
            _context.Viajes.Update(viaje);
            await _context.SaveChangesAsync();
        }
    }
}