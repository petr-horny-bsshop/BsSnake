using BsSnake.Contracts;

namespace BsSnake.Core.Model;

internal static class CoordinateDtoExtensions
{
    public static bool IsLeftTo(this CoordinateDto my, CoordinateDto other) => other.Y == my.Y && my.X +1 == other.X;
    public static bool IsRightTo(this CoordinateDto my, CoordinateDto other) => other.Y == my.Y && my.X -1 == other.X;
    public static bool IsAboveTo(this CoordinateDto my, CoordinateDto other) => other.X == my.X && my.Y -1 == other.Y;
    public static bool IsBellowTo(this CoordinateDto my, CoordinateDto other) => other.X == my.X && my.Y +1 == other.Y;
    public static int DistanceTo(this CoordinateDto my, CoordinateDto other) => Math.Abs(other.X - my.X) + Math.Abs(my.Y - other.Y);
    public static CoordinateDto[] GetDto(this IEnumerable<Coordinate> coordinates) => coordinates.Select(c => c.GetDto()).ToArray();

    public static CoordinateDto Move(this CoordinateDto coordinate, Direction direction)
    {
        switch (direction)
        {
            case Direction.Right:
                return new CoordinateDto(coordinate.X + 1, coordinate.Y);
            case Direction.Left:
                return new CoordinateDto(coordinate.X - 1, coordinate.Y);
            case Direction.Up:
                return new CoordinateDto(coordinate.X, coordinate.Y + 1);
            case Direction.Down:
                return new CoordinateDto(coordinate.X, coordinate.Y - 1);
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }

    public static Direction DirectionTo(this CoordinateDto coordinate, CoordinateDto coordinate2)
    {
        if (coordinate.X < coordinate2.X) return Direction.Right;
        if (coordinate.X > coordinate2.X) return Direction.Left;
        if (coordinate.Y < coordinate2.Y) return Direction.Up;
        if (coordinate.Y > coordinate2.Y) return Direction.Down;
        return default;
    }
}