using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UniRx;
using UniRx.Triggers;

namespace Proto2D
{
    public class TriggerTimeline : MonoBehaviour
    {
        [SerializeField]
        PlayableDirector m_playableDirector;

        private void Start()
        {
            this.OnTriggerEnter2DAsObservable()
                .Where(collider => collider.gameObject.CompareTag("Player"))
                .First()
                .Subscribe(x => PlayTimeline());
        }

        private void PlayTimeline()
        {
            m_playableDirector.Play();
        }
    }
}
