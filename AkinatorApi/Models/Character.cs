public class Character
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int AddedByUserId { get; set; }
    public DateTime AddedAt { get; set; }

    // Навигационное свойство — пользователь, который добавил персонажа
    public User? AddedByUser { get; set; }
}
