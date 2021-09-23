using UnityEngine;
using UnityEngine.UI;

namespace Simulation.UI
{
    internal class AnimalAmountAdjuster : MonoBehaviour
    {
        [SerializeField] private Slider _nValueSlider = default;
        [SerializeField] private Slider _mValueSlider = default;

        private void Awake()
        {
            _nValueSlider.onValueChanged.AddListener(Adjust);
            Adjust(_nValueSlider.value);
        }

        private void Adjust(float value)
        {
            _mValueSlider.maxValue = value * value * .5f;
        }

        public void Dispose()
        {
            _nValueSlider.onValueChanged.RemoveListener(Adjust);
        }
    }
}