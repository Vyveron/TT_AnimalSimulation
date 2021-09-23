using System;
using Simulation.UI;
using UnityEngine;

namespace Simulation
{
    internal sealed class TimeScaler : MonoBehaviour, IDisposable
    {
        [SerializeField] private TextSlider _timeSlider = default;
        [SerializeField] private AnimalSimulation _animalSimulation = default;

        private void Awake()
        {
            _timeSlider.OnChanged += ScaleTime;
        }

        private void Update()
        {
            _timeSlider.Slider.interactable = _animalSimulation.IsRunning;
        }

        private void ScaleTime(float value)
        {
            if (_animalSimulation.IsRunning == false)
                return;
            
            _animalSimulation.Time.TimeScale = value;
        }
        
        public void Dispose()
        {
            if (_timeSlider == null)
                return;
            
            _timeSlider.OnChanged -= ScaleTime;
            _timeSlider.Dispose();
        }
    }
}