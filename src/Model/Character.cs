namespace Tataru.Model;

public class Character
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<Team> Teams { get; } = new();
}
