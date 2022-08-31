using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    // 正射影カメラの描画範囲をタイルマップの横幅を基準に調整する
    // PC など横長の画面は考慮していない点に注意
    [ExecuteAlways]
    public class CameraOrthoScaler : MonoBehaviour
    {
        [SerializeField]
        private Cinemachine.CinemachineVirtualCamera vcam;

        [SerializeField]
        private float horizontalTiles = 9f;

        private void Start()
        {
            FitCameraOrthoSize();
        }

        private void FitCameraOrthoSize()
        {
            if (vcam)
            {
                float currentAspectPortrait = 1.0f / vcam.m_Lens.Aspect;

                vcam.m_Lens.OrthographicSize = currentAspectPortrait * horizontalTiles / 2;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            FitCameraOrthoSize();
        }
#endif
    }
}
