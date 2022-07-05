using UnityEngine;
using System.Collections;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class CinematicBarsMixerBehaviour : PlayableBehaviour
{
    public TimelineClip[] clips;
    public CinematicBars cinematicBars;

    private PlayableDirector playableDirector;

    public override void OnPlayableCreate(Playable playable)
    {
        playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var time = playableDirector.time;

        foreach (var clip in clips)
        {
            if (time >= clip.start && time <= clip.end)
            {
                var asset = clip.asset as CinematicBarsClip;

                var t = (float)((time - clip.start) / clip.duration);

                if (asset.type == CinematicBarsClip.VisibilityType.Show)
                {
                    cinematicBars.ChangeHeightByPercent(t);
                }
                else
                {
                    cinematicBars.ChangeHeightByPercent(1f - t);
                }
            }
        }
    }
}
