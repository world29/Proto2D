using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Controller2D target;

    public float verticalOffset;
    public float verticalSmoothTime;
    public Vector2 focusAreaSize;

    [Header("下方向に追従")]
    public bool followDownward = true;

    [Header("横方向に追従")]
    public bool followSideway = false;

    FocusArea focusArea;

    float smoothVelocityY;

    private void Start()
    {
    }

    private void Update()
    {
        if (target == null)
        {
            PlayerController pc = FindObjectOfType<PlayerController>();
            if (pc)
            {
                target = pc.gameObject.GetComponent<Controller2D>();
                focusArea = new FocusArea(target.collider.bounds, focusAreaSize);
            }
        }
    }

    private void LateUpdate()
    {
        if (target)
        {
            focusArea.Update(target.collider.bounds, followDownward, followSideway);
        }

        Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;

        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, .2f);
        Gizmos.DrawCube(focusArea.centre, focusAreaSize);
    }

    struct FocusArea
    {
        public Vector2 centre;
        public Vector2 velocity;
        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds, bool followDownward, bool followSideway)
        {
            if (followSideway)
            {
                float shiftX = 0;
                if (targetBounds.min.x < left)
                {
                    shiftX = targetBounds.min.x - left;
                }
                else if (targetBounds.max.x > right)
                {
                    shiftX = targetBounds.max.x - right;
                }
                left += shiftX;
                right += shiftX;
            }

            float shiftY = 0;
            if (targetBounds.min.y < bottom && followDownward)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            bottom += shiftY;
            top += shiftY;

            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(0, shiftY);
        }
    }
}
