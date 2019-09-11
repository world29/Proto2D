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

    [SerializeField] private bool touched = false;
    private float timeTouchBegan;
    private float timeTouchEnded;

    protected override void Start()
    {
        //
        base.Start();
    }

    private void LateUpdate()
    {
        touched = false;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        //
        timeTouchBegan = Time.timeSinceLevelLoad;

        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        //
        timeTouchEnded = Time.timeSinceLevelLoad;
        if (Direction == Vector2.zero)
        {
            touched = true;
        }

        base.OnPointerUp(eventData);
    }
}
