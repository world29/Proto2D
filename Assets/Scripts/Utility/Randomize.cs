using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    static public class Randomize
    {
        // 異なる確率からひとつを選ぶ
        // https://docs.unity3d.com/2018.4/Documentation/Manual/RandomNumbers.html
        public static int ChooseWithProbabilities(float[] probs)
        {
            float total = 0;

            foreach (float elem in probs)
            {
                total += elem;
            }

            float randomPoint = Random.value * total;

            for (int i = 0; i < probs.Length; i++)
            {
                if (randomPoint < probs[i])
                {
                    return i;
                }
                else
                {
                    randomPoint -= probs[i];
                }
            }
            return probs.Length - 1;
        }
    }
}
