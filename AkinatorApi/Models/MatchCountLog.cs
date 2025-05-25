public class MatchCountLog
{
    public int Id { get; set; }
    public int? UserId { get; set; }           // кто делал запрос, если есть пользователь
    public DateTime RequestedAt { get; set; }
    public string AnswersJson { get; set; }   // сериализованные ответы
    public int MatchesCount { get; set; }

    public User? User { get; set; }
}
