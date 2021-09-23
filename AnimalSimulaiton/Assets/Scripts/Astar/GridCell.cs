using System.Collections.Generic;

namespace Simulation.Pathfinding
{
    internal class GridCell<T> where T : Obstacle
    {
        protected readonly List<T> Obstacles = new List<T>();

        internal void AddObstacle(T obstacle)
        {
            Obstacles.Add(obstacle);
        }
    }
}