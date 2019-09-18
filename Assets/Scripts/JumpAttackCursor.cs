using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAttackCursor : MonoBehaviour
{
    private Canvas canvas;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector3 targetDir = targetPosition - player.transform.position;
        targetDir.x *= player.transform.localScale.x;

        float angleDeg = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;

        canvas.transform.localRotation = Quaternion.Euler(0, 0, angleDeg);
    }
}
