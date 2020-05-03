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
    public Color areaColor = new Color(1, 0, 1, .3f);

    public bool Touched { get { return touched; } }
    public bool Flicked { get { return flicked; } }
    public Vector2 FlickDirection { get { return positionTouchEnded - positionTouchBegan; } }

    [SerializeField]
    bool deadZoneYEnabled = false;
    [SerializeField]
    float deadZoneY = 0;

    [SerializeField] private bool touched = false;
    [SerializeField] private bool flicked = false;
    private bool touching = false;

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
        touching = true;
        positionTouchBegan = eventData.position;
        timeTouchBegan = Time.timeSinceLevelLoad;

        //
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
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

    protected override void HandleInput(float magnitude, Vector2 normalised, Vector2 radius, Camera cam)
    {
        if (deadZoneYEnabled)
        {
            var vec = normalised * magnitude;

            float x = vec.x;
            float y = vec.y;

            if (Mathf.Abs(x) < DeadZone)
            {
                x = 0;
            }

            if (Mathf.Abs(y) < deadZoneY)
            {
                y = 0;
            }

            input = new Vector2(x, y).normalized;
        }
        else
        {
            base.HandleInput(magnitude, normalised, radius, cam);
        }
    }

    private void OnDrawGizmos()
    {
        // タッチ可能領域の可視化
        Vector3[] v = new Vector3[4];

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.GetWorldCorners(v);

        Vector2 center = (v[2] + v[0]) * .5f;
        Vector2 size = (v[2] - v[0]);

        Gizmos.color = areaColor;
        Gizmos.DrawCube(center, size);
    }

}
