using System;
using Cysharp.Threading.Tasks;
using Simulation.Pathfinding;

namespace Simulation
{
    internal sealed class Animal : SimulationGridObject, IDisposable
    {
        private AnimalMovement _movement;

        internal bool IsIdle => _movement.IsIdle;

        internal AnimalMovement Movement => _movement;

        internal void Initialize(float speed, SimulationTime simulationTime)
        {
            if (Grid == null)
                throw new Exception("First set grid");

            _movement = new AnimalMovement(speed, Root, Grid, simulationTime);
            _movement.OnDestinationChanged += DestinationChangedHandler;
        }

        public UniTask Next()
        {
            return _movement.MoveToNext();
        }

        private void DestinationChangedHandler(PathCell cell)
        {
            SetPositionWithoutRefresh(cell.Position);
        }

        public void Dispose()
        {
            _movement.OnDestinationChanged -= DestinationChangedHandler;
            _movement.Dispose();
        }
    }
}