using Microsoft.AspNetCore.Mvc;
using AkinatorApi.Data;
using AkinatorApi.Models;
using System.Threading.Tasks;

namespace AkinatorApi.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly AkinatorDbContext _db;

        public UsersController(AkinatorDbContext db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }
    }
}
