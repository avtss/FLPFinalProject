public class QuestionInfo
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public List<string> Options { get; set; } = new();

    public int AddedByUserId { get; set; }
    public User? AddedByUser { get; set; }
    public DateTime AddedAt { get; set; }
}
