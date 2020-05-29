using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class PlayerUncontrollableBehaviour : PlayableBehaviour
{
    public GameObject playerObject;

    private bool m_played = false; // Play のあとに Pause が呼び出されたときだけ処理を行うための番兵

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

        m_played = true;
    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        if (!m_played) { return; }
        if (playerObject == null) { return; }

        var controller = playerObject.GetComponent<PlayerController>();
        controller.enabled = true;

        // プレイヤーの親子付けを変更し、シーン直下のオブジェクトにする
        playerObject.transform.SetParent(null);
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        
    }
}
