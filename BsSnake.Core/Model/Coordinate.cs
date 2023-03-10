using BsSnake.Contracts;

namespace BsSnake.Core.Model;

/// <summary>
/// Definuje souřadnici na hrací ploše.
/// </summary>
public class Coordinate
{
    /// <summary>
    /// Pozice X.
    /// </summary>
    public int X { get; }
    
    /// <summary>
    /// Pozice Y.
    /// </summary>
    public int Y { get; }

    /// <summary> ctor. </summary>
    /// <param name="x"><inheritdoc cref="X"/></param>
    /// <param name="y"><inheritdoc cref="Y"/></param>
    public Coordinate(int x, int y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Posune souřadnici o jeden bod v zadaném směru.
    /// </summary>
    public Coordinate Move(Direction direction)
    {
        switch (direction)
        {
            case Direction.Right:
                return new Coordinate(X + 1, Y);
            case Direction.Left:
                return new Coordinate(X - 1, Y);
            case Direction.Up:
                return new Coordinate(X, Y + 1);
            case Direction.Down:
                return new Coordinate(X, Y - 1);
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    /// <summary>
    /// Vrátí DTO objekt souřadnice.
    /// </summary>
    public CoordinateDto GetDto()
    {
        return new CoordinateDto(X, Y);
    }

    /// <summary>
    /// Operátor ekvivalence.
    /// </summary>
    public static bool operator == (Coordinate left, Coordinate right) => left.X == right.X && left.Y == right.Y;

    /// <summary>
    /// Operátor neekvivalence.
    /// </summary>
    public static bool operator !=(Coordinate left, Coordinate right) => !(left == right);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Coordinate coordinate && this == coordinate;

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}