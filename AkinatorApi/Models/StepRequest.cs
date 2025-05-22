namespace AkinatorApi.Models
{
    public class StepRequest
    {
        public string SessionId { get; set; } = "";
        public string? Answer { get; set; } = null; // 'y' или 'n', null для первого шага
    }
}
