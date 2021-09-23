using System;

namespace Simulation
{
    internal sealed class SimulationTime
    {
        internal float TimeScale = 1f;
        
        internal int CurrentTime { get; private set; }

        internal Action OnTick;

        internal void Increment()
        {
            CurrentTime++;
            OnTick?.Invoke();
        }
    }
}