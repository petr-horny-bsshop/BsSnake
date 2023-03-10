using System.Text.Json.Serialization;

namespace BsSnake.Contracts;

/// <summary>
/// Informace o hadovi.
/// </summary>
public record SnakeDto
{
    /// <summary>
    /// Jedinečný identifikátor hada v rámci hry.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Název hada.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Url adresa hada.
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    /// Pozice hlavy.
    /// </summary>
    [JsonPropertyName("head")]
    public required CoordinateDto Head { get; init; }

    /// <summary>
    /// Pozice jednotlivých článků těla kromě hlavy.
    /// </summary>
    [JsonPropertyName("body")]
    public required CoordinateDto[] Body { get; init; }

    /// <summary>
    /// Délka hada včetně hlavy.
    /// Had bez těla, tj. pouze s hlavou má délku 1.
    /// </summary>
    [JsonPropertyName("length")]
    public required int Length { get; init; }

    /// <summary>
    /// Energie hada.
    /// Každým krokem se energie snižuje o 1.
    /// </summary>
    [JsonPropertyName("energy")]
    public required int Energy { get; init; }

    /// <summary>
    /// Zda je had naživu.
    /// </summary>
    [JsonPropertyName("alive")]
    public required bool Alive { get; init; }

    /// <summary>
    /// Barva hada.
    /// </summary>
    [JsonPropertyName("color")]
    public required string Color { get; init; }

    /// <summary>
    /// Průměrná latence hada.
    /// </summary>
    [JsonPropertyName("latency")]
    public required int Latency { get; init; }

    /// <summary>
    /// Důvod smrti hada.
    /// </summary>
    [JsonPropertyName("deathCause")]
    public required string? DeathCause { get; init; }

    /// <summary>
    /// Iterace, ve které had zemřel.
    /// </summary>
    [JsonPropertyName("deathIteration")]
    public required int? DeathIteration { get; init; }
}