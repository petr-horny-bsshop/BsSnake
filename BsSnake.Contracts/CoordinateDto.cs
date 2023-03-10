using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace BsSnake.Contracts;

/// <summary>
/// Souřadnice na hrací ploše.
/// Pozice (0,0) reprezentuje levý spodní roh.
/// </summary>
public record CoordinateDto
{
    /// <summary>
    /// Pozice x.
    /// Počítáno od 0.
    /// </summary>
    [JsonPropertyName("x")]
    public required int X { get; init; }
    
    /// <summary>
    /// Pozice y.
    /// Počítáno od 0.
    /// </summary>
    [JsonPropertyName("y")]
    public required int Y { get; init; }

    /// <summary> ctor. </summary>
    /// <param name="x"><inheritdoc cref="X"/></param>
    /// <param name="y"><inheritdoc cref="Y"/></param>
    [SetsRequiredMembers]
    public CoordinateDto(int x, int y)
    {
        X = x;
        Y = y;
    }
}