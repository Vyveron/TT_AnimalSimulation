using System;
using TMPro;
using UnityEngine;

namespace Simulation.UI
{
    internal sealed class StateIndicator : MonoBehaviour, IDisposable
    {
        [SerializeField] private TextMeshProUGUI _stateField = default;
        [SerializeField] private AnimalSimulation _animalSimulation = default;

        private void Awake()
        {
            _animalSimulation.OnStateChanged += HandleState;
        }

        private void HandleState(ESimulationState state)
        {
            _stateField.text = state.ToString();
        }

        public void Dispose()
        {
            _animalSimulation.OnStateChanged -= HandleState;
        }
    }
}