using UnityEngine;

namespace FigmentGames
{
    [System.Serializable]
    public struct MinMaxRangeInt
    {
        [SerializeField] private int _minValue;
        public int minValue { get { return _minValue; } }
        [SerializeField] private int _maxValue;
        public int maxValue { get { return _maxValue; } }

        public MinMaxRangeInt(int minValue, int maxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }
    }

    [System.Serializable]
    public struct MinMaxRangeFloat
    {
        [SerializeField] private float _minValue;
        public float minValue { get { return _minValue; } }
        [SerializeField] private float _maxValue;
        public float maxValue { get { return _maxValue; } }

        public MinMaxRangeFloat(float minValue, float maxValue)
        {
            _minValue = minValue;
            _maxValue = maxValue;
        }
    }

    public class MinMaxRangeSlider : PropertyAttribute
    {
        public float minLimit;
        public float maxLimit;

        public MinMaxRangeSlider(float minLimit, float maxLimit)
        {
            this.minLimit = minLimit;
            this.maxLimit = maxLimit;
        }

        public MinMaxRangeSlider(int minLimit, int maxLimit)
        {
            this.minLimit = minLimit;
            this.maxLimit = maxLimit;
        }
    }
}
