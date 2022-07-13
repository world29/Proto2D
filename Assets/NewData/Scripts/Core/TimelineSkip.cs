using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace Assets.NewData.Scripts
{
    public class TimelineSkip : MonoBehaviour
    {
        private PlayableDirector currentDirector;
        private bool isSceneSkipped = true;
        private float timeToSkipTo;

        private void Update()
        {
            if (InputSystem.Input.Cutscene.MoveNext.triggered && !isSceneSkipped)
            {
                currentDirector.time = timeToSkipTo;
                isSceneSkipped = true;
            }
        }

        public void GetDirector(PlayableDirector director)
        {
            isSceneSkipped = false;
            currentDirector = director;
        }

        public void GetSkipTime(float skipTime)
        {
            timeToSkipTo = skipTime;
        }
    }
}