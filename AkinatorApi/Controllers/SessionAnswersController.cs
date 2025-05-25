using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AkinatorApi.Data;
using AkinatorApi.Models;

namespace AkinatorApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionAnswersController : ControllerBase
    {
        private readonly AkinatorDbContext _context;

        public SessionAnswersController(AkinatorDbContext context)
        {
            _context = context;
        }

        // POST: api/SessionAnswers
        [HttpPost]
        public async Task<IActionResult> AddAnswer([FromBody] AddAnswerRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Answer))
                return BadRequest("Invalid answer data.");

            var sessionAnswer = new SessionAnswer
            {
                GameSessionId = request.GameSessionId,
                QuestionId = request.QuestionId,
                Answer = request.Answer,
                AddedByUserId = request.AddedByUserId,
                AddedAt = DateTime.UtcNow
            };

            _context.SessionAnswers.Add(sessionAnswer);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAnswerById), new { id = sessionAnswer.Id }, sessionAnswer);
        }

        // GET: api/SessionAnswers/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnswerById(int id)
        {
            var answer = await _context.SessionAnswers
                .Include(sa => sa.AddedByUser)
                .FirstOrDefaultAsync(sa => sa.Id == id);

            if (answer == null)
                return NotFound();

            return Ok(answer);
        }

        // GET: api/SessionAnswers/BySession/{sessionId}
        [HttpGet("BySession/{sessionId}")]
        public async Task<IActionResult> GetAnswersBySession(int sessionId)
        {
            var answers = await _context.SessionAnswers
                .Where(sa => sa.GameSessionId == sessionId)
                .Include(sa => sa.AddedByUser)
                .ToListAsync();

            return Ok(answers);
        }
    }

    public class AddAnswerRequest
    {
        public int GameSessionId { get; set; }
        public int QuestionId { get; set; }
        public string Answer { get; set; } = "";
        public int AddedByUserId { get; set; }
    }
}
