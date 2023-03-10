using System.Text.Json.Serialization;

namespace BsSnake.Contracts;

/// <summary>
/// Informace o hrací ploše.
/// </summary>
public record BoardDto
{
    /// <summary>
    /// Výška hrací plochy.
    /// </summary>
    [JsonPropertyName("height")]
    public required int Height { get; init; }

    /// <summary>
    /// Šířka hrací plochy.
    /// </summary>
    [JsonPropertyName("width")]
    public required int Width { get; init; }

    /// <summary>
    /// Informace o políčkách s jídlem.
    /// </summary>
    [JsonPropertyName("food")]
    public required CoordinateDto[] Food { get; init; }

    /// <summary>
    /// Informace o políčkách s překážkami.
    /// </summary>
    [JsonPropertyName("obstacles")]
    public required CoordinateDto[] Obstacles { get; init; }

    /// <summary>
    /// Informace o všech hadech na hrací ploše.
    /// Jsou zde i hadi hráčů, kteří již zemřeli.
    /// Je zde i had aktuálního hráče.
    /// </summary>
    [JsonPropertyName("snakes")]
    public required SnakeDto[] Snakes { get; init; }
}