using UnityEngine;
using System.Collections;

public class CinematicBars : MonoBehaviour
{
    [SerializeField]
    private RectTransform topBarRect;

    [SerializeField]
    private RectTransform bottomBarRect;

    [SerializeField]
    private float barHeight = 100;

    private void Start()
    {
        ChangeHeight(0f);
    }

    public void ChangeHeight(float height)
    {
        var sizeDelta = new Vector2(0, height);

        topBarRect.sizeDelta = bottomBarRect.sizeDelta = sizeDelta;
    }

    public void ChangeHeightByPercent(float percent)
    {
        var sizeDelta = new Vector2(0, barHeight * percent);

        topBarRect.sizeDelta = bottomBarRect.sizeDelta = sizeDelta;
    }
}
