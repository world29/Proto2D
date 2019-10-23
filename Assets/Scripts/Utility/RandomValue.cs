using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    [System.Serializable]
    public struct RandomValue
    {
        public enum ValueType { Range, Constant }

        public float Value { get { return getNext(); } }

        [SerializeField]
        private ValueType m_type;
        [SerializeField]
        private float m_min;
        [SerializeField]
        private float m_max;
        [SerializeField]
        private float m_value;

        public RandomValue(ValueType type = ValueType.Range)
        {
            m_type = type;
            m_min = 0;
            m_max = 1;
            m_value = 0;
        }

        private float getNext()
        {
            if (m_type == ValueType.Range)
            {
                return Random.Range(m_min, m_max);
            }

            // if ValueType.Constant
            return m_value;
        }
    }
}
