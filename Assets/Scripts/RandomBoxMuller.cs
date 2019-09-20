using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    // 固定シード値の乱数生成器
    public class RandomFixedState
    {
        private Random.State m_state;

        public float value { get { return next(); } }
        public Vector2 insideUnitCircle { get { return nextInsideUnitCircle(); } }

        public RandomFixedState(int seed)
        {
            SetSeed(seed);
        }

        public void SetSeed(int seed)
        {
            var prev_state = Random.state;
            Random.InitState(seed);
            m_state = Random.state;
            Random.state = prev_state;
        }

        private float next()
        {
            var prev_state = Random.state;
            Random.state = m_state;

            var result = Random.value;

            m_state = Random.state;
            Random.state = prev_state;

            return result;
        }

        private Vector2 nextInsideUnitCircle()
        {
            var prev_state = Random.state;
            Random.state = m_state;

            var result = Random.insideUnitCircle;

            m_state = Random.state;
            Random.state = prev_state;

            return result;
        }
    }

    // 正規分布に従う乱数生成器
    public class RandomBoxMuller
    {
        public RandomFixedState Source { get; }

        public RandomBoxMuller()
            : this((int)System.DateTime.Now.Ticks)
        {
        }

        public RandomBoxMuller(int seed)
        {
            Source = new RandomFixedState(seed);
        }

        public float Next(float mu = 0, float sigma = 1, bool useCosine = true)
        {
            float x = Source.value;
            float y = Source.value;
            float result = 0;

            if (useCosine)
            {
                result = Mathf.Sqrt(-2 * Mathf.Log(x)) * Mathf.Cos(2 * Mathf.PI * y);
            }
            else
            {
                result = Mathf.Sqrt(-2 * Mathf.Log(x)) * Mathf.Sin(2 * Mathf.PI * y);
            }

            return result * sigma + mu;
        }

        public Vector2 NextPair(float mu = 0, float sigma = 1)
        {
            float x = Source.value;
            float y = Source.value;

            Vector2 result;

            result.x = Mathf.Sqrt(-2 * Mathf.Log(x)) * Mathf.Cos(2 * Mathf.PI * y);
            result.y = Mathf.Sqrt(-2 * Mathf.Log(x)) * Mathf.Sin(2 * Mathf.PI * y);

            return result * sigma + new Vector2(mu, mu);
        }
    }
}
