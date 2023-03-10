using System.Text.Json;
using System.Text.Json.Serialization;

namespace BsSnake.Core.Model;

/// <summary>
/// Nastavení hry.
/// </summary>
public record GameSettings
{
    /// <summary>
    /// Průměrný dovolený timeout v milisekundách.
    /// </summary>
    private const int AVERAGE_TIMEOUT = 1_000;
    
    /// <summary>
    /// Maximální dovolený timeout v milisekundách.
    /// </summary>
    private const int MAXIMUM_TIMEOUT = 10_000;

    /// <summary>
    /// Název hry (nastavení).
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Šířka hrací plochy.
    /// </summary>
    [JsonPropertyName("boardWidth")]
    public required int BoardWidth { get; init; }

    /// <summary>
    /// Výška hrací plochy.
    /// </summary>
    [JsonPropertyName("boardHeight")]
    public required int BoardHeight { get; init; }

    /// <summary>
    /// Pravděpodobnost, že se v každé iteraci na volném políčku vytvoří jídlo.
    /// </summary>
    [JsonPropertyName("foodProbability")]
    public required double FoodProbability { get; init; }

    /// <summary>
    /// Pravděpodobnost, že se v každé iteraci na volném políčku vytvoří překážka.
    /// </summary>
    [JsonPropertyName("obstacleProbability")]
    public required double ObstacleProbability { get; init; }

    /// <summary>
    /// Průměrný dovolený timeout v milisekundách.
    /// Pokud je NULL použije se výchozí hodnota <see cref="AVERAGE_TIMEOUT"/>.
    /// </summary>
    [JsonPropertyName("averageTimeout")]
    public int? AverageTimeout { get; init; }

    /// <summary>
    /// Maximální dovolený timeout v milisekundách.
    /// Pokud je NULL použije se výchozí hodnota <see cref="MAXIMUM_TIMEOUT"/>.
    /// </summary>
    [JsonPropertyName("maximumTimeout")]
    public int? MaximumTimeout { get; init; }

    /// <summary>
    /// Maximální počet iterací, po jehož překročení dojde k ukončení hry.
    /// </summary>
    [JsonPropertyName("maximumIterations")]
    public int? MaximumIterations { get; init; }

    /// <summary>
    /// Počáteční energie hadů.
    /// </summary>
    [JsonPropertyName("startingEnergy")]
    public int StartingEnergy { get; init; }

    /// <summary>
    /// Energie, kterou had získá po konzumaci.
    /// </summary>
    [JsonPropertyName("energyIncrease")]
    public int FoodEnergy { get; init; }

    /// <summary>
    /// Vytvoří výchozí sadu nastavení her.
    /// </summary>
    public static IReadOnlyCollection<GameSettings> CreateDefault()
    {
        var firstRound = new GameSettings
        {
            Name = "1. kolo",
            BoardWidth = 12,
            BoardHeight = 12,
            FoodProbability = 0.3,
            ObstacleProbability = 0.0,
            MaximumIterations = 250,
            StartingEnergy = 100,
            FoodEnergy = 10,

            AverageTimeout = AVERAGE_TIMEOUT,
            MaximumTimeout = MAXIMUM_TIMEOUT
        };

        var secondRound = firstRound with
        {
            Name = "2. kolo",
            ObstacleProbability = 0.1,
            FoodProbability = 0.1,
            MaximumIterations = 500,
        };

        var thirdRound = secondRound with
        {
            Name = "3. kolo",
            ObstacleProbability = 0.1,
            MaximumIterations = 5000,
        };

        return new [] { firstRound, secondRound, thirdRound };
    }

    /// <summary>
    /// Načte nastavení her ze souboru games.json.
    /// </summary>
    public static IReadOnlyCollection<GameSettings> Load()
    {
        var filePath = "games.json";
        if (!File.Exists(filePath)) return CreateDefault();
        var json = File.ReadAllText(filePath);
        var result = JsonSerializer.Deserialize<GameSettings[]>(json) ?? CreateDefault();
        return result;
    }

    /// <summary>
    /// Vytvoří výchozí nastavení her a uloží je do souboru games.json.
    /// </summary>
    public static IReadOnlyCollection<GameSettings> CreateAndSaveDefault()
    {
        var games = CreateDefault();
        var json = JsonSerializer.Serialize(games, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("games.json", json);
        return games;
    }
}