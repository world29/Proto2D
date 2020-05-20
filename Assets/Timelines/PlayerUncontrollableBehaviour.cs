using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class PlayerUncontrollableBehaviour : PlayableBehaviour
{
    public GameObject playerObject;
    public RuntimeAnimatorController animatorController;

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
        if (playerObject == null) { return; }

        var controller = playerObject.GetComponent<PlayerController>();
        controller.enabled = false;

        var animator = playerObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = null;
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (playerObject == null) { return; }

        var animator = playerObject.GetComponent<Animator>();
        animator.runtimeAnimatorController = animatorController;

        var controller = playerObject.GetComponent<PlayerController>();
        controller.enabled = true;
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }
}
