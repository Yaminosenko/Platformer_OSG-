using UnityEngine;

namespace FigmentGames
{
    [System.Serializable]
    public class RangedInt
    {
        [SerializeField] private int _value;
        public int value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value < minLimit)
                    value = minLimit;
                else if (value > maxLimit)
                    value = maxLimit;

                _value = value;
            }
        }

        public int minLimit;
        public int maxLimit;

        public RangedInt(int value, int minLimit, int maxLimit)
        {
            this.value = value;
            this.minLimit = minLimit;
            this.maxLimit = maxLimit;
        }
    }

    [System.Serializable]
    public class RangedFloat
    {
        [SerializeField] private float _value;
        public float value
        {
            get
            {
                return _value;
            }
            set
            {
                if (value < minLimit)
                    value = minLimit;
                else if (value > maxLimit)
                    value = maxLimit;

                _value = value;
            }
        }

        public float minLimit;
        public float maxLimit;

        public RangedFloat(float value, float minLimit, float maxLimit)
        {
            this.value = value;
            this.minLimit = minLimit;
            this.maxLimit = maxLimit;
        }
    }
}
