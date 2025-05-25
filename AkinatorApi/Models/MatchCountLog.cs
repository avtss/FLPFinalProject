public class MatchCountLog
{
    public int Id { get; set; }
    public int? UserId { get; set; }           // ��� ����� ������, ���� ���� ������������
    public DateTime RequestedAt { get; set; }
    public string AnswersJson { get; set; }   // ��������������� ������
    public int MatchesCount { get; set; }

    public User? User { get; set; }
}
