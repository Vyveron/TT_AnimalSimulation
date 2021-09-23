using UnityEngine;

namespace Simulation.Pathfinding
{
    internal sealed class PathObstacle : Obstacle
    {
        internal int ArrivingTime;
        
        private readonly Vector2Int _fromDirection;

        internal PathObstacle(int arrivingTime, Vector2Int fromDirection)
        {
            ArrivingTime = arrivingTime;
            _fromDirection = fromDirection;
        }

        internal bool IsActive(int arrivingTime, Vector2Int toDirection)
        {
            return arrivingTime == ArrivingTime || 
                (arrivingTime - 1 == ArrivingTime && (_fromDirection == -toDirection || _fromDirection == Vector2Int.zero));
        }
    }
}