public class Character
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int AddedByUserId { get; set; }
    public DateTime AddedAt { get; set; }

    // ������������� �������� � ������������, ������� ������� ���������
    public User? AddedByUser { get; set; }
}
