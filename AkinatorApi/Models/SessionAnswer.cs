public class SessionAnswer
{
    public int Id { get; set; }
    public int GameSessionId { get; set; }
    public GameSession? GameSession { get; set; }

    public int QuestionId { get; set; }
    public string Answer { get; set; }

    public int AddedByUserId { get; set; }  // добавленное поле
    public User? AddedByUser { get; set; }
    public DateTime AddedAt { get; set; }
}