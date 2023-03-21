using System.Text.Json.Serialization;

namespace BsSnake.Contracts;

/// <summary>
/// Směr hada.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Direction
{
    /// <summary>
    /// Vpravo.
    /// </summary>
    Right,
    
    /// <summary>
    /// Vlevo.
    /// </summary>
    Left,
    
    /// <summary>
    /// Nahoru.
    /// </summary>
    Up,
    
    /// <summary>
    /// Dolu.
    /// </summary>
    Down
}