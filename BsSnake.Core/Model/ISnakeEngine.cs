using BsSnake.Contracts;

namespace BsSnake.Core.Model
{
    /// <summary>
    /// Engine, který ovládá pohyb hada.
    /// </summary>
    public interface ISnakeEngine
    {
        /// <summary>
        /// Řekne hadovi, aby se připravil na novou hru.
        /// </summary>
        Task InitAsync(GameDto gameDto, CancellationToken cancellationToken);

        /// <summary>
        /// Zeptá se hada, kterým směrem má pohnout.
        /// </summary>
        Task<Direction> MoveAsync(GameDto game, CancellationToken cancellationToken);
    }
}
