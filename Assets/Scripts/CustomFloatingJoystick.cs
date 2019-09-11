using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public struct FlickData
{

}

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
    public bool Touched { get { return touched; } }
    public bool Flicked { get { return flicked; } }
    public Vector2 FlickDirection { get { return positionTouchEnded - positionTouchBegan; } }

    public float flickThresholdTime = .2f;

    [SerializeField] private bool touched = false;
    [SerializeField] private bool flicked = false;
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
        //
        positionTouchBegan = eventData.position;

        timeTouchBegan = Time.timeSinceLevelLoad;

        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        //
        positionTouchEnded = eventData.position;

        if (Direction == Vector2.zero)
        {
            touched = true;
        }
        else if ((Time.timeSinceLevelLoad - timeTouchBegan) < flickThresholdTime)
        {
            flicked = true;
        }

        base.OnPointerUp(eventData);
    }
}
