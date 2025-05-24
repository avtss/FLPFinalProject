using Microsoft.EntityFrameworkCore;
using AkinatorApi.Models; 

namespace AkinatorApi.Data
{
	public class AkinatorDbContext : DbContext
	{
		public AkinatorDbContext(DbContextOptions<AkinatorDbContext> options) : base(options) { }

		public DbSet<User> Users { get; set; }
		public DbSet<GameSession> GameSessions { get; set; }
		public DbSet<Character> Characters { get; set; }
		public DbSet<SessionAnswer> SessionAnswers { get; set; }
	}
}
