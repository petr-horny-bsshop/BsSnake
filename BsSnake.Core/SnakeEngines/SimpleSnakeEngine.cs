using BsSnake.Contracts;
using BsSnake.Core.Model;

namespace BsSnake.Core.SnakeEngines
{
    /// <summary>
    /// Jednoduchý engine, který ovládá hada.
    /// </summary>
    public class SimpleSnakeEngine : ISnakeEngine
    {
        /// <inheritdoc />
        public Task<ResponseDto> MoveAsync(GameDto status, CancellationToken cancellationToken)
        {
            var allowedDirections = new HashSet<Direction>
            {
                Direction.Right,
                Direction.Up,
                Direction.Down,
                Direction.Left
            };

            // kolize s okraji hrací plochy
            if (status.You.Head.X >= status.Board.Width - 1) allowedDirections.Remove(Direction.Right);
            if (status.You.Head.X <= 0) allowedDirections.Remove(Direction.Left);
            if (status.You.Head.Y <= 0) allowedDirections.Remove(Direction.Down);
            if (status.You.Head.Y >= status.Board.Height - 1) allowedDirections.Remove(Direction.Up);

            // kolize s ostatními hady (včetně sebe)
            foreach (var snake in status.Board.Snakes.Where(s => s.Alive))
            {
                foreach (var coordinate in snake.Body)
                {
                    if (status.You.Head.IsRightTo(coordinate)) allowedDirections.Remove(Direction.Left);
                    if (status.You.Head.IsLeftTo(coordinate)) allowedDirections.Remove(Direction.Right);
                    if (status.You.Head.IsAboveTo(coordinate)) allowedDirections.Remove(Direction.Down);
                    if (status.You.Head.IsBellowTo(coordinate)) allowedDirections.Remove(Direction.Up);
                }

                if (status.You.Head.IsRightTo(snake.Head)) allowedDirections.Remove(Direction.Left);
                if (status.You.Head.IsLeftTo(snake.Head)) allowedDirections.Remove(Direction.Right);
                if (status.You.Head.IsAboveTo(snake.Head)) allowedDirections.Remove(Direction.Down);
                if (status.You.Head.IsBellowTo(snake.Head)) allowedDirections.Remove(Direction.Up);
            }

            // kolize s překážkami
            foreach (var coordinate in status.Board.Obstacles)
            {
                if (status.You.Head.IsRightTo(coordinate)) allowedDirections.Remove(Direction.Left);
                if (status.You.Head.IsLeftTo(coordinate)) allowedDirections.Remove(Direction.Right);
                if (status.You.Head.IsAboveTo(coordinate)) allowedDirections.Remove(Direction.Down);
                if (status.You.Head.IsBellowTo(coordinate)) allowedDirections.Remove(Direction.Up);
            }

            var preferredDirection = DecideDirection2(status);
            if (allowedDirections.Contains(preferredDirection)) return Task.FromResult(new ResponseDto {Direction = preferredDirection});

            var result = allowedDirections.OrderByDescending(d => DistanceToMyBody(status, d)).FirstOrDefault();
            return Task.FromResult(new ResponseDto { Direction = result });
        }

        /// <inheritdoc />
        public Task InitAsync(GameDto gameDto, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private Direction DecideDirection2(GameDto status)
        {
            var distanceToFood = status.Board.Food.OrderBy(f => status.You.Head.DistanceTo(f));

            foreach (var food in distanceToFood)
            {
                var direction = status.You.Head.DirectionTo(food);
                var distanceToMyBody = DistanceToMyBody(status, direction);
                if (distanceToMyBody > status.You.Body.Length) return direction;
            }

            return default;
        }

        private int DistanceToMyBody(GameDto status, Direction direction)
        {
            var head = status.You.Head;
            var distance = 0;
            while (true)
            {
                distance++;
                head = head.Move(direction);
                if (IsInCollisionWithSelf(status, head)) break;
                if (head.X <= 0 || head.X >= status.Board.Width) return int.MaxValue;
                if (head.Y <= 0 || head.Y >= status.Board.Height) return int.MaxValue;
            }
            return distance;
        }

        private bool IsInCollisionWithSelf(GameDto status, CoordinateDto coordinate)
        {
            if (status.You.Body.Any(b => b == coordinate)) return true;
            return false;
        }
    }
}