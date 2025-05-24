using Microsoft.AspNetCore.Mvc;
using AkinatorApi.Data;
using AkinatorApi.Models;
using System.Threading.Tasks;

namespace AkinatorApi.Controllers
{
    [ApiController]
    [Route("api/characters")]
    public class CharactersController : ControllerBase
    {
        private readonly AkinatorDbContext _db;

        public CharactersController(AkinatorDbContext db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> AddCharacter([FromBody] Character character)
        {
            character.AddedByUser = null;
            character.AddedAt = DateTime.UtcNow;
            _db.Characters.Add(character);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCharacter), new { id = character.Id }, character);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCharacter(int id)
        {
            var character = await _db.Characters.FindAsync(id);
            if (character == null) return NotFound();
            return Ok(character);
        }
    }
}