using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proto2D
{
    public class UIStatusController : MonoBehaviour
    {
        public Slider m_progressSlider;

        void Start()
        {
            Debug.Assert(GameController.Instance.m_progress != null);
            GameController.Instance.m_progress.OnChanged += OnProgressChanged;
        }

        void Update()
        {
        }

        void OnProgressChanged(float value)
        {
            m_progressSlider.value = value;
        }

        void UpdateUI()
        {
        }
    }
}

