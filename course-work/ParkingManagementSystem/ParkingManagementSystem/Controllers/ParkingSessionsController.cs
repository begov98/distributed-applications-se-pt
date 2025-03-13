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
    /// Контролер за управление на сесиите на паркиране.
    /// Предоставя CRUD операции, търсене и масово въвеждане на данни.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingSessionsController : ControllerBase
    {
        private readonly ParkingContext _context;

        public ParkingSessionsController(ParkingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Връща странициран списък с сесии на паркиране.
        /// </summary>
        /// <param name="pageNumber">Номер на страницата (по подразбиране 1).</param>
        /// <param name="pageSize">Брой сесии на страница (по подразбиране 10).</param>
        /// <returns>Списък от сесии на паркиране.</returns>
        // GET: api/ParkingSessions?pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetParkingSessions(int pageNumber = 1, int pageSize = 10)
        {
            var sessions = await _context.ParkingSessions
                .Include(ps => ps.ParkingLot)
                .Include(ps => ps.Vehicle)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return Ok(sessions);
        }

        /// <summary>
        /// Връща детайли на конкретна сесия на паркиране по ID.
        /// </summary>
        /// <param name="id">ID-то на сесията на паркиране.</param>
        /// <returns>Детайли на сесията на паркиране.</returns>
        // GET: api/ParkingSessions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetParkingSession(int id)
        {
            var session = await _context.ParkingSessions
                .Include(ps => ps.ParkingLot)
                .Include(ps => ps.Vehicle)
                .FirstOrDefaultAsync(ps => ps.SessionID == id);
            if (session == null)
                return NotFound();
            return Ok(session);
        }

        /// <summary>
        /// Създава нова сесия на паркиране.
        /// </summary>
        /// <param name="session">Обект с данни за новата сесия на паркиране.</param>
        /// <returns>Създадената сесия на паркиране с нейния идентификатор.</returns>
        // POST: api/ParkingSessions
        [HttpPost]
        public async Task<IActionResult> CreateParkingSession([FromBody] ParkingSession session)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.ParkingSessions.Add(session);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetParkingSession), new { id = session.SessionID }, session);
        }

        /// <summary>
        /// Актуализира съществуваща сесия на паркиране.
        /// </summary>
        /// <param name="id">ID-то на сесията на паркиране.</param>
        /// <param name="session">Обект с актуализирани данни за сесията на паркиране.</param>
        /// <returns>Няма съдържание при успешна актуализация.</returns>
        // PUT: api/ParkingSessions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParkingSession(int id, [FromBody] ParkingSession session)
        {
            if (id != session.SessionID)
                return BadRequest();

            _context.Entry(session).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ParkingSessions.Any(e => e.SessionID == id))
                    return NotFound();
                else
                    throw;
            }
            return NoContent();
        }

        /// <summary>
        /// Изтрива сесия на паркиране по ID.
        /// </summary>
        /// <param name="id">ID-то на сесията на паркиране.</param>
        /// <returns>Няма съдържание при успешно изтриване.</returns>
        // DELETE: api/ParkingSessions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParkingSession(int id)
        {
            var session = await _context.ParkingSessions.FindAsync(id);
            if (session == null)
                return NotFound();

            _context.ParkingSessions.Remove(session);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Търси сесии на паркиране по регистрационния номер на превозно средство.
        /// </summary>
        /// <param name="plateNumber">Регистрационният номер на превозното средство.</param>
        /// <returns>Списък от сесии на паркиране, които отговарят на зададения критерий.</returns>
        // GET: api/ParkingSessions/search?plateNumber=ABC123
        [HttpGet("search")]
        public async Task<IActionResult> SearchParkingSessions(string plateNumber)
        {
            var sessions = await _context.ParkingSessions
                .Include(ps => ps.ParkingLot)
                .Include(ps => ps.Vehicle)
                .Where(ps => ps.Vehicle.PlateNumber.Contains(plateNumber))
                .ToListAsync();
            return Ok(sessions);
        }

        /// <summary>
        /// Позволява добавяне на JSON масив (използва се за пълнене на базата с примерна информация)
        /// </summary>
        /// <param name="sessions"></param>
        /// <returns></returns>
        // POST: api/ParkingSessions/bulk
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateParkingSessions([FromBody] List<ParkingSession> sessions)
        {
            if (sessions == null || sessions.Count == 0)
            {
                return BadRequest("Няма подадени данни.");
            }

            foreach (var session in sessions)
            {
                if (!TryValidateModel(session))
                {
                    return BadRequest(ModelState);
                }
                _context.ParkingSessions.Add(session);
            }

            await _context.SaveChangesAsync();
            return Ok(sessions);
        }

    }
}
