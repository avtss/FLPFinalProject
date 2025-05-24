using Microsoft.AspNetCore.Mvc;
using AkinatorApi.Data;
using AkinatorApi.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace AkinatorApi.Controllers
{
    [ApiController]
    [Route("api/sessions")]
    public class GameSessionsController : ControllerBase
    {
        private readonly AkinatorDbContext _db;

        public GameSessionsController(AkinatorDbContext db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> CreateSession([FromBody] GameSession session)
        {
            session.StartedAt = DateTime.UtcNow;
            _db.GameSessions.Add(session);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSession), new { id = session.Id }, session);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSession(int id)
        {
            var session = await _db.GameSessions
                .Include(s => s.SessionAnswers)
                .Include(s => s.GuessedCharacters)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null) return NotFound();
            return Ok(session);
        }
    }
}