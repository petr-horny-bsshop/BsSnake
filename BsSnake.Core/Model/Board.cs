using System.Diagnostics.CodeAnalysis;
using BsSnake.Contracts;

namespace BsSnake.Core.Model;

/// <summary>
/// Definuje hrací plochu.
/// Souřadnice [0,0] odpovídá levému dolnímu rohu.
/// </summary>
public class Board
{
    private readonly List<Snake> _snakes = new List<Snake>();

    /// <summary>
    /// Šířka hrací plochy.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Výška hrací plochy.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Seznam pozic s potravou na hrací ploše.
    /// </summary>
    public IList<Coordinate> Food { get; } = new List<Coordinate>();

    /// <summary>
    /// Seznam pozic s překážkami na hrací ploše.
    /// </summary>
    public IList<Coordinate> Obstacles { get; } = new List<Coordinate>();

    /// <summary>
    /// Seznam všech hadů na hrací ploše.
    /// </summary>
    public IReadOnlyCollection<Snake> Snakes => _snakes;
    
    /// <summary>
    /// Seznam živých hadů na hrací ploše.
    /// </summary>
    public IEnumerable<Snake> AliveSnakes => _snakes.Where(s => s.Alive);

    /// <summary> ctor. </summary>
    /// <param name="width"><inheritdoc cref="Width"/></param>
    /// <param name="height"><inheritdoc cref="Height"/></param>
    public Board(int width, int height)
    {
        Width = width;
        Height = height;
    }

    /// <summary>
    /// Přidá hada na hrací plochu.
    /// </summary>
    /// <param name="snakeEngine">Engine hada.</param>
    /// <param name="energy">Počáteční energie.</param>
    /// <param name="name">Název hada.</param>
    /// <param name="color">Barva  hada.</param>
    public Snake AddSnake(ISnakeEngine snakeEngine, int energy, string name, string color)
    {
        var snake = new Snake(GetFreeCell(), energy, snakeEngine, name, color);
        _snakes.Add(snake);
        return snake;
    }

    /// <summary>
    /// Vrátí DTO s informacemi o hrací ploše.
    /// </summary>
    public BoardDto GetDto()
    {
        var dto = new BoardDto
        {
            Food = Food.GetDto(),
            Obstacles = Obstacles.GetDto(),
            Height = Height,
            Width = Width,
            Snakes = Snakes.Select(s => s.GetDto()).ToArray()
        };
        return dto;
    }

    /// <summary>
    /// Přidá potravu na náhodně vybrané volné pole.
    /// </summary>
    public void AddFood()
    {
        if (TryGetFreeCell(out var coordinate))
        {
            Food.Add(coordinate);
        }
    }

    /// <summary>
    /// Přidá překážku na náhodně vybrané volné pole.
    /// </summary>
    public void AddObstacle()
    {
        if (TryGetFreeCell(out var coordinate))
        {
            Obstacles.Add(coordinate);
        }
    }

    private Coordinate GetFreeCell()
    {
        if (TryGetFreeCell(out var coordinate)) return coordinate;
        throw new Exception("Nepovedlo se najít volnou buňku.");
    }

    private bool TryGetFreeCell([NotNullWhen(true)]out Coordinate? coordinate, int maxAttempts=100)
    {
        for(var attempt=0; attempt<maxAttempts; attempt++)
        {
            var x = Random.Shared.Next(0, Width);
            var y = Random.Shared.Next(0, Height);
            coordinate = new Coordinate(x, y);
            if (IsCellFree(coordinate)) return true;
        }

        coordinate = default;
        return false;
    }

    private bool IsCellFree(Coordinate coordinate)
    {
        foreach (var snake in AliveSnakes)
        {
            if (snake.Head == coordinate) return false;
            if (snake.Body.Any(b => b == coordinate)) return false;
        }

        if (Food.Any(f => f == coordinate)) return false;
        return true;
    }

}