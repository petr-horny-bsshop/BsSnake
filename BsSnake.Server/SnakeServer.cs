using BsSnake.Contracts;
using BsSnake.Core.SnakeEngines;

namespace BsSnake.Server;

internal class SnakeServer
{
    private SimpleSnakeEngine _snakeEngine;

    private SnakeServer(int id, WebApplication app)
    {
        app.MapGet($"/snake/{id}", () =>
        {
            return Results.Ok("ok");
        }); 

        app.MapPost($"/snake/{id}/init", async (GameDto game) =>
        {
            _snakeEngine = new SimpleSnakeEngine();
            await _snakeEngine.InitAsync(game, CancellationToken.None);
            return Results.Ok();
        }); 

        app.MapPost($"/snake/{id}/move", async (GameDto game) =>
        {
            var result = await _snakeEngine.MoveAsync(game, CancellationToken.None);
            return Results.Ok(result);
        }); 
    }

    public static SnakeServer Map(int id, WebApplication app)
    {
        var result = new SnakeServer(id, app);
        return result;
    }
}