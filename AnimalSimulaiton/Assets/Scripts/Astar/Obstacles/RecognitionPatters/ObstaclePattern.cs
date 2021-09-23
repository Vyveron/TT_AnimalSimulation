namespace Simulation.Pathfinding
{
    internal abstract class ObstaclePattern
    {
        internal abstract bool IsObstacle(AstarCell cell);
    }
}