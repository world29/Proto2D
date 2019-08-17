using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Player))]
public class PlayerInput : MonoBehaviour
{
    [Header("ドラッグとみなされるマウスポインタの移動量の最小値")]
    public float dragDistanceMin = 5;

    Player player;

    private MouseState mouseState;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();

        mouseState.dragDistanceMin = dragDistanceMin;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.SetDirectionalInput(directionalInput);

        mouseState.Update();

        if (GetJumpButtonDown() || mouseState.isClicked)
        {
            player.OnJumpInputDown();
        }

        if (GetJumpButtonUp())
        {
            player.OnJumpInputUp();
        }

        if (mouseState.isDragged)
        {
            //Debug.LogFormat("MouseDragged ({0} - {1})", mouseState.dragStartPos.ToString(), mouseState.dragEndPos.ToString());

            Vector3 direction = mouseState.dragEndPos - mouseState.dragStartPos;
            direction.Normalize();

            player.OnJumpAttackInput(direction);

            Vector3 origin = transform.position;
            Debug.DrawLine(origin, origin + direction, Color.red, 1, false);
        }
    }

    bool GetJumpButtonDown()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    bool GetJumpButtonUp()
    {
        return Input.GetKeyUp(KeyCode.Space);
    }

    struct MouseState
    {
        public bool isClicked;
        public bool isDragged;
        public bool isDragging;
        public Vector3 dragStartPos;
        public Vector3 dragEndPos;
        public float dragDistanceMin;

        public void Reset()
        {
            isClicked = false;
            isDragged = false;
        }

        public void Update()
        {
            Reset();

            if (isDragging)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    isDragging = false;

                    dragEndPos = Input.mousePosition;

                    // ドラッグの始点と終点の距離が最小値未満なら無視する
                    if (Vector3.Distance(dragEndPos, dragStartPos) < dragDistanceMin)
                    {
                        isClicked = true;
                    }
                    else
                    {
                        isDragged = true;
                    }
                }
            }
            else
            {
                // start dragging
                if (Input.GetMouseButtonDown(0))
                {
                    dragStartPos = Input.mousePosition;
                    isDragging = true;
                }
            }

        }
    }
}
