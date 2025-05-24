using Microsoft.AspNetCore.Mvc;
using AkinatorApi.Data;
using AkinatorApi.Models;
using System.Threading.Tasks;

namespace AkinatorApi.Controllers
{
    [ApiController]
    [Route("api/sessionanswers")]
    public class SessionAnswersController : ControllerBase
    {
        private readonly AkinatorDbContext _db;

        public SessionAnswersController(AkinatorDbContext db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> AddAnswer([FromBody] SessionAnswer answer)
        {
            answer.GameSession = null;
            _db.SessionAnswers.Add(answer);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAnswer), new { id = answer.Id }, answer);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnswer(int id)
        {
            var answer = await _db.SessionAnswers.FindAsync(id);
            if (answer == null) return NotFound();
            return Ok(answer);
        }
    }
}