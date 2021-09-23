using UnityEngine;

namespace Simulation
{
    internal sealed class SimulationGridObjectFactory
    {
        private readonly SimulationGrid _simulationGrid;
        
        internal SimulationGridObjectFactory(SimulationGrid grid)
        {
            _simulationGrid = grid;
        }
        
        internal T Spawn<T>(T prefab, Vector2Int position) where T : SimulationGridObject
        {
            var simulationGridRoot = _simulationGrid.Root;
            var gridObject = Object.Instantiate(prefab, simulationGridRoot);

            gridObject.Root.localScale = _simulationGrid.GetLocalScaleForObject();
            gridObject.InitializeGrid(_simulationGrid, position);

            return gridObject;
        }
    }
}