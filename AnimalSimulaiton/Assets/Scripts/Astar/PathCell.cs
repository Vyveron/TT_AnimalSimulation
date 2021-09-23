using UnityEngine;

namespace Simulation.Pathfinding
{
    internal struct PathCell
    {
        internal readonly int ArrivingTime;
        
        internal Vector2Int Position;

        internal PathCell(Vector2Int position, int arrivingTime)
        {
            Position = position;
            ArrivingTime = arrivingTime;
        }
    }
}