using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Proto2D
{
    public class OrbManager : SingletonMonoBehaviour<OrbManager>
    {
        [SerializeField]
        OrbController m_orbPrefab;

        [SerializeField]
        Canvas m_canvas;

        //TODO: 複数同時にドロップした場合に位置が重ならないように、生成位置をばらつかせるためのパラメータを追加する
        public void DropOrb(Vector3 worldPosition)
        {
            Camera camera = m_canvas.worldCamera;
            if (m_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                camera = null;
            }

            // ワールド空間からキャンバス座標へ変換
            var screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldPosition);
            Vector2 localPosition = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_canvas.GetComponent<RectTransform>(), screenPoint, camera, out localPosition);

            // オーブを Canvas の子として生成する
            var orb = GameObject.Instantiate(m_orbPrefab);
            orb.transform.SetParent(m_canvas.transform, false);
            orb.GetComponent<RectTransform>().localPosition = localPosition;
        }
    }
}
