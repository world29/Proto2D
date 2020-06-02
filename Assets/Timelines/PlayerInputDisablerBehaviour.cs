using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Proto2D
{
    // A behaviour that is attached to a playable
    public class PlayerInputDisablerBehaviour : PlayableBehaviour
    {
        public GameObject playerObject;

        // Called when the owning graph starts playing
        public override void OnGraphStart(Playable playable)
        {
            playerObject.GetComponent<PlayerInput>().enabled = false;
        }

        // Called when the owning graph stops playing
        public override void OnGraphStop(Playable playable)
        {
            playerObject.GetComponent<PlayerInput>().enabled = true;
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
    }
}
