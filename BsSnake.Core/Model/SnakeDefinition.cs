using System.Text.Json.Serialization;

namespace BsSnake.Core.Model
{
    /// <summary>
    /// Definice hada.
    /// </summary>
    public record SnakeDefinition
    {
        /// <summary>
        /// Název hada.
        /// </summary>
        [JsonPropertyName("name")]
        public required string Name { get; init; }

        /// <summary>
        /// Url adresa hada.
        /// </summary>
        [JsonPropertyName("url")]
        public required string Url { get; init; }
    }
}
