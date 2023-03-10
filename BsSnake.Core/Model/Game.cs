using System.Diagnostics;
using BsSnake.Contracts;
using BsSnake.Core.SnakeEngines;

namespace BsSnake.Core.Model;

/// <summary>
/// Definuje právě jednu hru.
/// </summary>
public class Game
{
    private readonly GameSettings _settings;
    private readonly List<GameDto> _iterationHistory = new();
    private readonly IReadOnlyList<string> _snakeColors = new []
        {
        "#ff0000",
        "#00ff00",
        "#0000ff",
        "#e3c700",
        "#ff00ff",
        "#00ffff",
        "#000000",
        "#ffffff"
        };

    /// <summary>
    /// Hrací plocha.
    /// </summary>
    public Board Board { get; private set; }

    /// <summary> ctor. </summary>
    /// <param name="settings">Nastavení hry.</param>
    /// <param name="snakes">Seznam hadů ve hře.</param>
    public Game(GameSettings settings, IReadOnlyList<SnakeDefinition> snakes)
    {
        if (snakes.Count == 0) throw new ArgumentException("Seznam hadů nesmí být prázdný.", nameof(snakes));

        _settings = settings;
        Board = new Board(settings.BoardWidth, settings.BoardHeight);
        
        for (var i = 0; i < snakes.Count; i++)
        {
            var snakeDefinition = snakes[i];
            var snake = new RemoteSnakeEngine(snakeDefinition.Url);
            var snakeColor = _snakeColors[i % _snakeColors.Count];
            Board.AddSnake(snake, _settings.StartingEnergy, snakeDefinition.Name, snakeColor);
        }
    }

    /// <summary>
    /// Aktuální iterace hry.
    /// </summary>
    public int Iteration { get; private set; }

    /// <summary>
    /// Zda byla hra dokončena.
    /// </summary>
    public bool Completed { get; private set; }

    /// <summary>
    /// Text popisující důvod ukončení hry.
    /// </summary>
    public string? CompletedText { get; private set; }

    /// <summary>
    /// Seznam stavů hry pro jednotlivé iterace.
    /// </summary>
    public IReadOnlyList<GameDto> IterationHistory => _iterationHistory;

    /// <summary>
    /// Vrátí DTO hry.
    /// </summary>
    public GameDto GetDto()
    {
        var gameDto = new GameDto
        {
            Board = Board.GetDto(),
            You = null!,
            Iteration = Iteration
        };
        return gameDto;
    }

    /// <summary>
    /// Vrátí DTO hry pro konkrétního hada.
    /// </summary>
    public GameDto GetDto(Snake snake)
    {
        var gameDto = new GameDto
        {
            You = snake.GetDto(),
            Board = Board.GetDto(),
            Iteration = Iteration
        };
        return gameDto;
    }

    /// <summary>
    /// Provede inicializaci hry.
    /// </summary>
    /// <returns></returns>
    public async Task InitAsync()
    {
        await InitSnakesAsync();
        
        var gameDto = GetDto();
        _iterationHistory.Add(gameDto);
    }

    /// <summary>
    /// Provede jednu iteraci hry.
    /// </summary>
    public async Task<bool> MoveAsync()
    {
        if (Completed) return false;

        Iteration++;

        await MoveSnakesAsync();
        CheckCollisions();
        CheckFood();
        CheckSnakeEnergy();
        GenerateFood();
        GenerateObstacles();

        var gameDto = GetDto();
        _iterationHistory.Add(gameDto);

        var anySnakeAlive = Board.AliveSnakes.Any();
        if (!anySnakeAlive)
        {
            Completed = true;
            CompletedText = "všichni hadi jsou mrtví";
        }
        else if (Iteration >= _settings.MaximumIterations)
        {
            Completed = true;
            CompletedText = "byl spotřebován vymezený počet iterací";
        }

        return anySnakeAlive;
    }

    private async Task InitSnakesAsync()
    {
        var tasks = new List<Task>();
        
        foreach (var snake in Board.AliveSnakes)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(_settings.MaximumTimeout ?? 10_000));
            var initTask = snake.InitAsync(this, cts.Token);
            initTask = initTask.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    snake.Kill($"Chyba: {t.Exception?.GetBaseException()?.Message}", Iteration);
                }
                else if (t.IsCanceled)
                {
                    snake.Kill("Překročen maximální timeout při komunikaci s hadem.", Iteration);
                }
            });
            tasks.Add(initTask);
        }

        await Task.WhenAll(tasks);
    }

    private async Task MoveSnakesAsync()
    {
        var tasks = new List<Task>();
        
        var sw = Stopwatch.StartNew();

        foreach (var snake in Board.AliveSnakes)
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(_settings.MaximumTimeout ?? 10_000));
            var task = snake.MoveAsync(this, cts.Token);
            task = task.ContinueWith(t =>
            {
                snake.AddResponseTime(sw.Elapsed);

                if (t.IsFaulted)
                {
                    snake.Kill($"Chyba: {t.Exception?.GetBaseException()?.Message}", Iteration);
                }
                else if (t.IsCanceled)
                {
                    snake.Kill("Překročen maximální timeout při komunikaci s hadem.", Iteration);
                }
                else if(Iteration > 3 && snake.Latency.TotalMilliseconds > _settings.AverageTimeout)
                {
                    snake.Kill("Překročen průměrný timeout při komunikaci s hadem.", Iteration);
                }

            });
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }

    private void CheckCollisions()
    {
        var aliveSnakes = Board.AliveSnakes.ToArray();

        // kolize se zdmi
        foreach (var snake in aliveSnakes)
        {
            if (snake.Head.X < 0)
            {
                snake.Kill("Kolize s okrajem hrací plochy.", Iteration);
                continue;
            }

            if (snake.Head.Y < 0)
            {
                snake.Kill("Kolize s okrajem hrací plochy.", Iteration);
                continue;
            }
            
            if (snake.Head.X >= Board.Width)
            {
                snake.Kill("Kolize s okrajem hrací plochy.", Iteration);
                continue;
            }

            if (snake.Head.Y >= Board.Height)
            {
                snake.Kill("Kolize s okrajem hrací plochy.", Iteration);
                continue;
            }
        }


        // kolize hadů
        foreach (var snakeA in aliveSnakes)
        {
            foreach (var snakeB in aliveSnakes)
            {
                // hlava A narazila na hlavu B
                if (snakeA.Head == snakeB.Head && snakeA != snakeB)
                {
                    snakeA.Kill($"Kolize s hlavou hada {snakeB.Name}.", Iteration);
                    snakeB.Kill($"Kolize s hlavou hada {snakeA.Name}.", Iteration);
                    break;
                }

                // hlava A narazila na tělo B (A může být zároveň B)
                if (snakeB.Body.Any(b => b == snakeA.Head))
                {
                    if (snakeA == snakeB) snakeA.Kill("Kolize se svým tělem.", Iteration);
                    else snakeA.Kill($"Kolize s tělem hada {snakeB.Name}.", Iteration);
                    break;
                }
            }   
        }

        // kolize hadů s překážkami
        foreach (var snake in aliveSnakes)
        {
            if (Board.Obstacles.Any(o => o == snake.Head))
            {
                snake.Kill("Kolize s překážkou.", Iteration);
            }
        }
    }

    private void CheckFood()
    {
        foreach (var snake in Board.AliveSnakes)
        {
            foreach (var food in Board.Food.ToArray())
            {
                if (snake.Head == food)
                {
                    snake.Eat(1, _settings.FoodEnergy);
                    Board.Food.Remove(food);
                }
            }
        }
    }

    private void CheckSnakeEnergy()
    {
        foreach (var snake in Board.AliveSnakes)
        {
            if (snake.Energy <= 0)
            {
                snake.Kill("Vyhladovění.", Iteration);
            }
        }
    }

    private void GenerateFood()
    {
        if (Random.Shared.NextDouble() < _settings.FoodProbability)
        {
            Board.AddFood();
        }
    }

    private void GenerateObstacles()
    {
        if (Random.Shared.NextDouble() < _settings.ObstacleProbability)
        {
            Board.AddObstacle();
        }
    }

    /// <summary>
    /// Pro zadaný stav hry v dané iteraci odešle na všechny hady požadavek na směr pohybu. Pohyb však ve hře nevykoná.
    /// </summary>
    /// <param name="iterationIndex">Index iterace hry.</param>
    public async Task SimulateRequestAsync(int iterationIndex)
    {
        if (iterationIndex < 0 || iterationIndex >= IterationHistory.Count) throw new ArgumentOutOfRangeException(nameof(iterationIndex));
        var gameDto = IterationHistory[iterationIndex];
        foreach (var snake in Board.Snakes)
        {
            var gameForSnakeDto = gameDto with
            {
                You = gameDto.Board.Snakes.Single(s => s.Id == snake.Id)
            };
            await snake.SimulateMoveAsync(gameForSnakeDto, CancellationToken.None);
        }
    }
}