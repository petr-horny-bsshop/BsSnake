namespace BsSnake.Contracts
{
    /// <summary>
    /// Odpověď hada v aktuální iteraci.
    /// </summary>
    public record ResponseDto
    {
        /// <summary>
        /// Směr, kterým se má had vydat.
        /// </summary>
        public Direction Direction { get; set; }
    }
}
