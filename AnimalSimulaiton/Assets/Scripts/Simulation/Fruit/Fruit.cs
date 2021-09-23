using Simulation.Extensions;
using UnityEngine;

namespace Simulation
{
    internal sealed class Fruit : SimulationGridObject
    {
        private FruitParticlePool _particles;
        
        internal void Initialize(FruitParticlePool particles)
        {
            _particles = particles;
        }

        internal bool TryMoveRandomly(Vector2Int center, int radius)
        {
            var appropriateSpace = Grid.GetInRadius(center, radius, 1);
            var freeSpace = Grid.GetFreeSpace(appropriateSpace);

            if (freeSpace.Count == 0)
                return false;
            
            var newPosition = freeSpace.PickRandom();

            SetPosition(newPosition);

            return true;
        }

        internal void PlayParticles()
        {
            _particles.Play(transform.position);
        }
    }
}