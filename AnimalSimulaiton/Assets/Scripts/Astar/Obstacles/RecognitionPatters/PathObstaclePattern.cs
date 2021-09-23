using UnityEngine;

namespace Simulation.Pathfinding
{
    internal sealed class PathObstaclePattern : ObstaclePattern
    {
        private readonly SimulationGrid _simulationGrid;

        internal PathObstaclePattern(SimulationGrid simulationGrid)
        {
            _simulationGrid = simulationGrid;
        }

        internal override bool IsObstacle(AstarCell cell)
        {
            var position = cell.Position;
            var toDirection = position - cell.Origin.Position;
            
            return _simulationGrid[position.x, position.y].HasObstacles(cell.ArrivingTime, toDirection);
        }
    }
}