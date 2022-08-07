using System.Collections;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    // フレームレートを固定する
    public class FrameRate : MonoBehaviour
    {
        [SerializeField]
        private int frameRate = 60;

        void Awake()
        {
            Application.targetFrameRate = frameRate;
        }
    }
}