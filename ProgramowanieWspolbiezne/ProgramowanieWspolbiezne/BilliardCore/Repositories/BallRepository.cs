using ProgramowanieWspolbiezne.BilliardCore.Abstractions;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ProgramowanieWspolbiezne.BilliardCore.Repositories;

public class BallRepository : IBallRepository
{
    private readonly ConcurrentDictionary<string, IBall> _balls = new();
    private readonly object _modificationLock = new();

    public IEnumerable<IBall> GetBalls()
    {
        return _balls.Values.ToList().AsReadOnly();
    }

    public void AddBall(IBall ball)
    {
        lock (_modificationLock)
        {
            string key = $"{ball.X}_{ball.Y}";
            _balls.TryAdd(key, ball);
        }
    }

    public void RemoveBall(IBall ball)
    {
        lock (_modificationLock)
        {
            var key = _balls.FirstOrDefault(x => x.Value == ball).Key;
            if (key != null)
            {
                _balls.TryRemove(key, out _);
            }
        }
    }

    public void Clear()
    {
        lock (_modificationLock)
        {
            _balls.Clear();
        }
    }

    public void UpdateAllPositions()
    {
        Parallel.ForEach(_balls.Values, ball =>
        {
            ball.UpdatePosition();
        });
    }
}