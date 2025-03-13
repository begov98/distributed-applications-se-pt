using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManager.Data;
using ParkingManager.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingManager.Controllers
{
    /// <summary>
    /// Контролер за управление на превозните средства.
    /// Предоставя CRUD операции и масово въвеждане на данни.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly ParkingContext _context;

        public VehiclesController(ParkingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Връща странициран списък с превозни средства.
        /// </summary>
        /// <param name="pageNumber">Номер на страницата (по подразбиране 1).</param>
        /// <param name="pageSize">Брой елементи на страница (по подразбиране 10).</param>
        /// <returns>Списък от превозни средства.</returns>
        // GET: api/Vehicles?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetVehicles(int pageNumber = 1, int pageSize = 10)
        {
            var vehicles = await _context.Vehicles
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return Ok(vehicles);
        }

        /// <summary>
        /// Връща детайли на конкретно превозно средство по ID.
        /// </summary>
        /// <param name="id">ID-то на превозното средство.</param>
        /// <returns>Детайли на превозното средство.</returns>
        // GET: api/Vehicles/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
                return NotFound();
            return Ok(vehicle);
        }

        /// <summary>
        /// Създава ново превозно средство.
        /// </summary>
        /// <param name="vehicle">Обект с данни за новото превозно средство.</param>
        /// <returns>Създаденото превозно средство с неговия идентификатор.</returns>
        // POST: api/Vehicles
        [HttpPost]
        public async Task<IActionResult> CreateVehicle([FromBody] Vehicle vehicle)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.VehicleID }, vehicle);
        }

        /// <summary>
        /// Актуализира съществуващо превозно средство.
        /// </summary>
        /// <param name="id">ID-то на превозното средство.</param>
        /// <param name="vehicle">Обект с актуализирани данни за превозното средство.</param>
        /// <returns>Няма съдържание при успешна актуализация.</returns>
        // PUT: api/Vehicles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] Vehicle vehicle)
        {
            if (id != vehicle.VehicleID)
                return BadRequest();

            _context.Entry(vehicle).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Vehicles.Any(e => e.VehicleID == id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        /// <summary>
        /// Изтрива превозно средство по ID.
        /// </summary>
        /// <param name="id">ID-то на превозното средство.</param>
        /// <returns>Няма съдържание при успешно изтриване.</returns>
        // DELETE: api/Vehicles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
                return NotFound();

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Позволява добавяне на JSON масив (използва се за пълнене на базата с примерна информация)
        /// </summary>
        /// <param name="vehicles"></param>
        /// <returns></returns>
        // POST: api/Vehicles/bulk
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateVehicles([FromBody] List<Vehicle> vehicles)
        {
            if (vehicles == null || vehicles.Count == 0)
            {
                return BadRequest("Няма подадени данни.");
            }

            foreach (var vehicle in vehicles)
            {
                if (!TryValidateModel(vehicle))
                {
                    return BadRequest(ModelState);
                }
                _context.Vehicles.Add(vehicle);
            }

            await _context.SaveChangesAsync();
            return Ok(vehicles);
        }

    }
}
