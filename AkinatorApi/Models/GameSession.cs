using System.Collections.Generic;

namespace AkinatorApi.Models
{
    public class GameSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }  // nullable
        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAt { get; set; }

        // Добавляем коллекции
        public ICollection<SessionAnswer>? Answers { get; set; }
        public ICollection<Character>? GuessedCharacters { get; set; }
    }
}
