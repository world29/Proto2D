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

        private void Update()
        {
            //Debug.Log($"(w:h): ({Screen.width}, {Screen.height})");

            if (vcam)
            {
                float currentAspectPortrait = (float)Screen.height / Screen.width;

                vcam.m_Lens.OrthographicSize = currentAspectPortrait * horizontalTiles / 2;
            }
        }
    }
}
