using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Proto2D
{
    public class PlayerCommandGoToBehaviour : PlayableBehaviour
    {
        public GameObject playerObject;
        public Transform targetPosition;

        // Called when the owning graph starts playing
        public override void OnGraphStart(Playable playable)
        {
        }

        // Called when the owning graph stops playing
        public override void OnGraphStop(Playable playable)
        {
        }

        // Called when the state of the playable is set to Play
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
        }

        // Called when the state of the playable is set to Paused
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
        }

        // Called each frame while the state is set to Play
        public override void PrepareFrame(Playable playable, FrameData info)
        {
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (playerObject == null) { return; }

            var playerController = playerObject.GetComponent<PlayerController>();

            var diff = targetPosition.position.x - playerObject.transform.position.x;
            if (Mathf.Abs(diff) > 1f)
            {
                var x = Mathf.Sign(diff);
                playerController.SetDirectionalInput(new Vector2(x, 0));
            }
            else
            {
                playerController.SetDirectionalInput(Vector2.zero);
            }
        }
    }
}
