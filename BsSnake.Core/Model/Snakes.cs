using System.Text.Json;

namespace BsSnake.Core.Model;

/// <summary>
/// Helper třída pro načítání a ukládání definic hadů.
/// </summary>
public static class Snakes
{
    /// <summary>
    /// Načte definice hadů ze souboru snakes.json.
    /// </summary>
    public static IReadOnlyList<SnakeDefinition> LoadSnakeDefinitions()
    {
        if (!File.Exists("snakes.json")) return Array.Empty<SnakeDefinition>();
        var json = File.ReadAllText("snakes.json");
        var result = JsonSerializer.Deserialize<SnakeDefinition[]>(json) ?? Array.Empty<SnakeDefinition>();
        return result;
    }

    /// <summary>
    /// Uloží definice hadů do souboru snakes.json.
    /// </summary>
    public static void SaveSnakeDefinitions(IList<SnakeDefinition> snakeDefinitions)
    {
        var json = JsonSerializer.Serialize(snakeDefinitions);
        File.WriteAllText("snakes.json", json);
    }
}