using Simulation.Pathfinding;
using UnityEngine;

namespace Simulation
{
    internal sealed class SimulationGridCell : GridCell<PathObstacle>
    {
        internal bool HasObstacles(int arrivingTime, Vector2Int toDirection)
        {
            foreach (var obstacle in Obstacles)
            {
                if (obstacle.IsActive(arrivingTime, toDirection))
                    return true;
            }

            return false;
        }

        internal void DecreaseObstacleValues()
        {
            foreach (var obstacle in Obstacles)
                obstacle.ArrivingTime--;

            for (var i = Obstacles.Count - 1; i >= 0; i--)
            {
                if (Obstacles[i].ArrivingTime < 0)
                    Obstacles.RemoveAt(i);
            }
        }

        internal bool IsOccupied()
        {
            return HasObstacles(0, Vector2Int.zero);
        }
    }
}