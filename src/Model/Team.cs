namespace Tataru.Model;

public class Team
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Character> Characters { get; } = new();
}
