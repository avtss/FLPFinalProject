public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // ������������� ��������: ��� ������� ������ ������������
    public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();

    // ������������� ��������: ��� ����������� ��������� (���� �����)
    public ICollection<Character> AddedCharacters { get; set; } = new List<Character>();
}
