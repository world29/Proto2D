using UnityEngine;
using System.Collections;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinematicBarsClip : PlayableAsset, ITimelineClipAsset
{
    public enum VisibilityType
    {
        Show,
        Hide,
    }

    [SerializeField]
    public VisibilityType type;


    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        var playable = ScriptPlayable<CinematicBarsBehaviour>.Create(graph);

        return playable;
    }
}
