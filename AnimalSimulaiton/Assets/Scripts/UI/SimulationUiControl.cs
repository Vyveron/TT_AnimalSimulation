using System;
using UnityEngine;
using UnityEngine.UI;

namespace Simulation.UI
{
    internal sealed class SimulationUiControl : MonoBehaviour, IDisposable
    {
        [SerializeField] private AnimalSimulation _simulation = default;
        [SerializeField] private TextSlider _nValueSlider = default;
        [SerializeField] private TextSlider _mValueSlider = default;
        [SerializeField] private TextSlider _vValueSlider = default;
        [SerializeField] private Button _startButton = default;

        private void Awake()
        {
            _startButton.onClick.AddListener(StartSimulation);
        }

        private void StartSimulation()
        {
            _simulation.InitializeSimulation(
                Mathf.RoundToInt(_nValueSlider.Value), 
                Mathf.RoundToInt(_mValueSlider.Value), 
                Mathf.RoundToInt(_vValueSlider.Value));
        }

        public void Dispose()
        {
            if (_nValueSlider != null)
                _nValueSlider.Dispose();

            if (_mValueSlider != null)
                _mValueSlider.Dispose();

            if (_vValueSlider != null)
                _vValueSlider.Dispose();

            _startButton.onClick.RemoveListener(StartSimulation);
        }
    }
}