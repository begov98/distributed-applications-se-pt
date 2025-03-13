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
    /// Контролер за управление на паркинг локациите.
    /// Предоставя CRUD операции, търсене и странициране.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingLotsController : ControllerBase
    {
        private readonly ParkingContext _context;

        public ParkingLotsController(ParkingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Връща странициран списък с паркинг локации.
        /// </summary>
        /// <param name="pageNumber">Номер на страницата (по подразбиране 1).</param>
        /// <param name="pageSize">Брой елементи на страница (по подразбиране 10).</param>
        /// <returns>Списък от паркинг локации.</returns>
        // GET: api/ParkingLots?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetParkingLots(int pageNumber = 1, int pageSize = 10)
        {
            var parkingLots = await _context.ParkingLots
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return Ok(parkingLots);
        }

        /// <summary>
        /// Връща детайли на конкретна паркинг локация по ID.
        /// </summary>
        /// <param name="id">ID-то на паркинг локацията.</param>
        /// <returns>Детайли на паркинг локацията.</returns>
        // GET: api/ParkingLots/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetParkingLot(int id)
        {
            var parkingLot = await _context.ParkingLots.FindAsync(id);
            if (parkingLot == null)
            {
                return NotFound();
            }
            return Ok(parkingLot);
        }

        /// <summary>
        /// Създава нова паркинг локация.
        /// </summary>
        /// <param name="parkingLot">Обект с данни за новата паркинг локация.</param>
        /// <returns>Създадената паркинг локация с нейния идентификатор.</returns>
        // POST: api/ParkingLots
        [HttpPost]
        public async Task<IActionResult> CreateParkingLot([FromBody] ParkingLot parkingLot)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.ParkingLots.Add(parkingLot);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetParkingLot), new { id = parkingLot.ParkingLotID }, parkingLot);
        }

        /// <summary>
        /// Актуализира съществуваща паркинг локация.
        /// </summary>
        /// <param name="id">ID-то на паркинг локацията.</param>
        /// <param name="parkingLot">Обект с актуализирани данни за паркинг локацията.</param>
        /// <returns>Няма съдържание при успешна актуализация.</returns>
        // PUT: api/ParkingLots/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParkingLot(int id, [FromBody] ParkingLot parkingLot)
        {
            if (id != parkingLot.ParkingLotID)
                return BadRequest();

            _context.Entry(parkingLot).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ParkingLots.Any(e => e.ParkingLotID == id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        /// <summary>
        /// Изтрива паркинг локация по ID.
        /// </summary>
        /// <param name="id">ID-то на паркинг локацията.</param>
        /// <returns>Няма съдържание при успешно изтриване.</returns>
        // DELETE: api/ParkingLots/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParkingLot(int id)
        {
            var parkingLot = await _context.ParkingLots.FindAsync(id);
            if (parkingLot == null)
                return NotFound();

            _context.ParkingLots.Remove(parkingLot);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Търси паркинг локации по име или адрес.
        /// </summary>
        /// <param name="name">Ключова дума за търсене.</param>
        /// <returns>Списък от паркинг локации, отговарящи на критерия.</returns>
        // GET: api/ParkingLots/search?name=abc
        [HttpGet("search")]
        public async Task<IActionResult> SearchParkingLots(string name)
        {
            var results = await _context.ParkingLots
                .Where(p => p.Name.Contains(name) || p.Address.Contains(name))
                .ToListAsync();
            return Ok(results);
        }

        /// <summary>
        /// Позволява добавяне на JSON масив (използва се за пълнене на базата с примерна информация)
        /// </summary>
        /// <param name="parkingLots"></param>
        /// <returns></returns>
        // POST: api/ParkingLots/bulk
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateParkingLots([FromBody] List<ParkingLot> parkingLots)
        {
            if (parkingLots == null || parkingLots.Count == 0)
            {
                return BadRequest("Няма подадени данни.");
            }

            foreach (var parkingLot in parkingLots)
            {
                if (!TryValidateModel(parkingLot))
                {
                    return BadRequest(ModelState);
                }
                _context.ParkingLots.Add(parkingLot);
            }

            await _context.SaveChangesAsync();
            return Ok(parkingLots);
        }
    }
}
