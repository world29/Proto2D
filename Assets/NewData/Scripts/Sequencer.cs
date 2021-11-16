using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace App
{
    /// <summary>
    /// アプリ全体のシーケンスの進行管理
    /// </summary>
    public class Sequencer : MonoBehaviour
    {
        const string kTitleCanvasPrefabPath = "UI/TitleCanvas";

        GameObject m_titleCanvas;

        public enum ESequence {
            Init,
            Title,
            Stage,
        };

        private void Start()
        {
            StartCoroutine(SequenceCoroutine());
        }

        IEnumerator SequenceCoroutine()
        {
            yield return InitSequenceCoroutine();

            yield return TitleSequenceCoroutine();

            yield return StageSequenceCoroutine();
        }

        IEnumerator InitSequenceCoroutine()
        {
            Debug.Log("[Sequence] Init");

            yield return ScreenFade.GetInstance().FadeOut(0.2f);
        }

        IEnumerator TitleSequenceCoroutine()
        {
            Debug.Log("[Sequence] StageStart");

            ResourceRequest request = Resources.LoadAsync(kTitleCanvasPrefabPath);
            while (!request.isDone)
            {
                yield return null;
            }
            var prefab = request.asset as GameObject;
            m_titleCanvas = GameObject.Instantiate(prefab);

            yield return ScreenFade.GetInstance().FadeIn(1.0f);

            yield return new WaitForSeconds(5.0f);

            yield return ScreenFade.GetInstance().FadeOut(1.0f);

            GameObject.Destroy(m_titleCanvas);

            yield return new WaitForSeconds(1.0f);
        }

        IEnumerator StageSequenceCoroutine()
        {
            Debug.Log("[Sequence] Stage");

            yield return ScreenFade.GetInstance().FadeIn(1.0f);
        }
    }
}
