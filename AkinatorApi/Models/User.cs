public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Навигационное свойство: все игровые сессии пользователя
    public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();

    // Навигационное свойство: все добавленные персонажи (если нужно)
    public ICollection<Character> AddedCharacters { get; set; } = new List<Character>();
}
