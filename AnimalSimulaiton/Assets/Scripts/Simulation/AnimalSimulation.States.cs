using System;
using UnityEngine;

namespace Simulation
{
    internal partial class AnimalSimulation
    {
        internal ESimulationState State { get; private set; }

        internal Action<ESimulationState> OnStateChanged;

        private void SetState(ESimulationState state)
        {
            if (state == ESimulationState.NotInitialized)
                return;
            
            State = state;
            
            OnStateChanged?.Invoke(state);
        }
    }
}