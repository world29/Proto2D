using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class CameraAutoScroll : MonoBehaviour
    {
        [Header("スクロール速度")]
        public float m_scrollSpeed = 1;

        void Start()
        {
        }

        //MEMO: プレイヤーの位置をみて何か処理をすることになったら、LateUpdate() に変更する
        private void Update()
        {
            Vector3 delta = Vector3.up * m_scrollSpeed * Time.deltaTime;
            gameObject.transform.Translate(delta);
        }
    }
}
