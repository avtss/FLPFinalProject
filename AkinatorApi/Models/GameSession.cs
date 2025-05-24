public class GameSession
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }

    public ICollection<SessionAnswer>? SessionAnswers { get; set; }  // ответы
    public ICollection<Character>? GuessedCharacters { get; set; }   // отгаданные персонажи
}
