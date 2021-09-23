using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Simulation.UI
{
    internal class TextSlider : MonoBehaviour, IDisposable
    {
        [SerializeField] private Slider _slider = default;
        [SerializeField] private TextMeshProUGUI _textField = default;

        private string _text;

        internal Slider Slider => _slider;

        internal Action<float> OnChanged;

        internal float Value => _slider.value;
        
        private void Awake()
        {
            _slider.onValueChanged.AddListener(HandleChange); 
            _text = _textField.text;
        }

        private void Start()
        {
            HandleChange(_slider.value);
        }

        private void HandleChange(float value)
        {
            _textField.text = _text + _slider.value;
            
            OnChanged?.Invoke(value);
        }
        
        public void Dispose()
        {
            _slider.onValueChanged.RemoveListener(HandleChange);
        }
    }
}