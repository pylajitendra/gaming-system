namespace GameService.API.Models;

public class Game
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Genre { get; set; }

    public DateTime CreatedAt { get; set; }

    // Relation with PlayerService
    public int? PlayerId { get; set; }
}