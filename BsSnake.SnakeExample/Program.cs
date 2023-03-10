using BsSnake.Contracts;

namespace BsSnake.SnakeExample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var app = builder.Build();


            app.MapGet("/", () => Results.Ok("toto je ukázkový had")); 

            app.MapPost("/init", (GameDto _) => Results.Ok()); 

            app.MapPost("/move", (GameDto game) =>
            {
                var directions = new[] { Direction.Up, Direction.Down, Direction.Left, Direction.Right };

                var direction = directions[Random.Shared.Next(0, directions.Length)];

                return Results.Ok(direction);
            }); 

            app.Run();
        }
    }
}