namespace AkinatorApi.Models
{
    public class SessionAnswer
{
    public int Id { get; set; }
    public int GameSessionId { get; set; }
    public GameSession GameSession { get; set; }
    public int QuestionId { get; set; }
    public bool Answer { get; set; }
}
}