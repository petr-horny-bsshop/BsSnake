using BsSnake.Contracts;
using BsSnake.Core.SnakeEngines;

namespace BsSnake.Core.Model;

/// <summary>
/// Definice hada.
/// </summary>
public class Snake
{
    private readonly ISnakeEngine _engine;
    private readonly Queue<Coordinate> _body = new Queue<Coordinate>();
    private int _foodToDigest = 0;
    private TimeSpan _timeSpent;
    private int _requests;

    /// <summary>
    /// Jedinečný identifikátor instance hada.
    /// </summary>
    public string Id { get; } = Guid.NewGuid().ToString();
    
    /// <summary>
    /// Název hada.
    /// </summary>
    public string Name { get; private init; }
    
    /// <summary>
    /// Url adresa hada (pokud se jedná o vzdáleného hada).
    /// </summary>
    public string? Url => (_engine as RemoteSnakeEngine)?.Url;

    /// <summary>
    /// Zda je had živý.
    /// </summary>
    public bool Alive { get; private set; } = true;

    /// <summary>
    /// Energie hada.
    /// </summary>
    public int Energy { get; private set; }

    /// <summary>
    /// Příčina smrti hada.
    /// </summary>
    public string? DeathCause { get; private set; }

    /// <summary>
    /// Iterace, ve které došlo ke smrti hada.
    /// </summary>
    public int? DeathIteration { get; private set; }

    /// <summary>
    /// Průměrná latence při komunikaci s hadem.
    /// </summary>
    public TimeSpan Latency => _requests > 0 ? _timeSpent / _requests : TimeSpan.Zero;

    /// <summary>
    /// Souřadnice těla hada.
    /// </summary>
    public IReadOnlyCollection<Coordinate> Body => _body;

    /// <summary>
    /// Souřadnice hlavy hada.
    /// </summary>
    public Coordinate Head { get; private set; } = new Coordinate(0, 0);

    /// <summary>
    /// Celková délka hada včetně hlavy.
    /// Had bez těla (pouze hlava) má délku 1.
    /// </summary>
    public int Length => _body.Count + 1;

    /// <summary>
    /// Barva hada.
    /// Jedná se o html kód barvy.
    /// </summary>
    public string Color { get; private set; }

    /// <summary>
    /// Vytvoří novou instanci hada.
    /// </summary>
    /// <param name="headPosition">Pozice hlavy.</param>
    /// <param name="energy">Počáteční energie.</param>
    /// <param name="engine">Engine, který ovládá hada.</param>
    /// <param name="name">Název hada.</param>
    /// <param name="color">Html barva hada.</param>
    public Snake(Coordinate headPosition, int energy, ISnakeEngine engine, string name, string color)
    {
        _engine = engine;
        Head = headPosition;
        Color = color;
        Name = name;
        Energy = energy;
    }

    /// <summary>
    /// Zabije hada.
    /// </summary>
    /// <param name="cause">Příčina smrti hada.</param>
    /// <param name="iteration">Iterace, ve které došlo ke smrti hada.</param>
    public void Kill(string cause, int iteration)
    {
        Alive = false;
        DeathCause = cause;
        DeathIteration = iteration;
    }

    /// <summary>
    /// Provede inicializaci hada.
    /// </summary>
    public async Task InitAsync(Game game, CancellationToken cancellationToken)
    {
        var gameDto = game.GetDto(this);
        await _engine.InitAsync(gameDto, cancellationToken);
    }

    /// <summary>
    /// Provede tah hada.
    /// </summary>
    public async Task MoveAsync(Game game, CancellationToken cancellationToken)
    {
        var gameDto = game.GetDto(this);
        var response = await _engine.MoveAsync(gameDto, cancellationToken);

        var hasBody = _body.Count > 0;

        if (hasBody)
        {
            if (_foodToDigest > 0) _foodToDigest--;
            else _body.Dequeue();
            _body.Enqueue(Head);
        }
        else
        {
            if (_foodToDigest > 0)
            {
                _foodToDigest--;
                _body.Enqueue(Head);
            }
        }

        Head = Head.Move(response.Direction);

        Energy--;
    }

    /// <summary>
    /// Simuluje tah hada, tzn. dotáže se hada na jeho další směr, ale pozici hada ve hře neaktualizuje.
    /// Určeno pro ladění hada.
    /// </summary>
    public async Task<Direction> SimulateMoveAsync(GameDto game, CancellationToken cancellationToken)
    {
        var response = await _engine.MoveAsync(game, cancellationToken);
        return response.Direction;
    }

    /// <summary>
    /// Informuje hada, že snědl jídlo se zadanou energií.
    /// </summary>
    /// <param name="count">Počet jídla, který had snědl.</param>
    /// <param name="energy">Celková energie získaná snědením daného počtu jídla.</param>
    public void Eat(int count, int energy)
    {
        _foodToDigest += count;
        Energy += energy;
    }

    /// <summary>
    /// Vrátí DTO hada.
    /// </summary>
    public SnakeDto GetDto()
    {
        var dto = new SnakeDto
        {
            Id = Id,
            Name = Name,
            Url = Url,
            Head = Head.GetDto(),
            Body = Body.GetDto(),
            Length = Length,
            Energy = Energy,
            Color = Color,
            Alive = Alive,
            DeathCause = DeathCause,
            DeathIteration = DeathIteration,
            Latency = (int)Latency.TotalMilliseconds
        };
        return dto;
    }

    /// <summary>
    /// Přidá čas do průměrné latence.
    /// </summary>
    public void AddResponseTime(TimeSpan duration)
    {
        _timeSpent += duration;
        _requests++;
    }
}