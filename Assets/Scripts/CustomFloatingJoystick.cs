using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//
// if (joystick.Flicked) {
//     joystick.Directional;
// }
// else if (joystick.Touched) {
// }
// else {
//     joystick.Directional;
// }

public class CustomFloatingJoystick : FloatingJoystick
{
    public float flickThresholdTime = .2f;
    public Rect touchableRect = new Rect(0, 0, 1, 1);
    public Color touchableAreaColor = new Color(1, 0, 1, .3f);

    public bool Touched { get { return touched; } }
    public bool Flicked { get { return flicked; } }
    public Vector2 FlickDirection { get { return positionTouchEnded - positionTouchBegan; } }

    [SerializeField] private bool touched = false;
    [SerializeField] private bool flicked = false;
    private bool touching = false;
    private int touchingFingerId;

    private float timeTouchBegan;
    private Vector2 positionTouchBegan;
    private Vector2 positionTouchEnded;

    protected override void Start()
    {
        //
        base.Start();
    }

    private void LateUpdate()
    {
        touched = false;
        flicked = false;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        // タッチ可能領域外なら無視する
        if (!GetTouchableScreenRect().Contains(eventData.position)) return;

        touching = true;
        touchingFingerId = eventData.pointerId;
        positionTouchBegan = eventData.position;
        timeTouchBegan = Time.timeSinceLevelLoad;

        //
        base.OnPointerDown(eventData);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (!touching || touchingFingerId != eventData.pointerId) return;

        //
        base.OnDrag(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (!touching || touchingFingerId != eventData.pointerId) return;

        touching = false;
        positionTouchEnded = eventData.position;

        if (Direction == Vector2.zero)
        {
            touched = true;
        }
        else if ((Time.timeSinceLevelLoad - timeTouchBegan) < flickThresholdTime)
        {
            flicked = true;
        }

        //
        base.OnPointerUp(eventData);
    }

    private Rect GetTouchableScreenRect()
    {
        return new Rect(
            touchableRect.x * Camera.main.pixelWidth,
            touchableRect.y * Camera.main.pixelHeight,
            touchableRect.width * Camera.main.pixelWidth,
            touchableRect.height * Camera.main.pixelHeight);
    }

    private void OnDrawGizmos()
    {
        Rect rect = GetTouchableScreenRect();

        Gizmos.color = touchableAreaColor;
        Gizmos.DrawCube(rect.center, rect.size);
    }

}
