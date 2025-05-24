public class AddDistinguishingGameRequest
{
    public string ExistingGameName { get; set; } = "";
    public List<string> ExistingGameAnswers { get; set; } = new();

    public string NewGameName { get; set; } = "";
    public List<string> NewGameAnswers { get; set; } = new();

    public string NewQuestionText { get; set; } = "";
    public List<string> NewQuestionOptions { get; set; } = new();
    public string ExistingGameAnswer { get; set; } = "";
    public string NewGameAnswer { get; set; } = "";
}
