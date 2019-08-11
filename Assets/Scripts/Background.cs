using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [Header("カメラ追従モード")]
    /*public*/ bool followCamera = true;

    [Range(1, 20), Header("視差(カメラ追従モードが無効の場合のみ)")]
    /*public*/ float parallax = 1;

    private void Start()
    {
        Vector3 camPos = Camera.main.transform.position;
        transform.position = new Vector3(transform.position.x, camPos.y, transform.position.z);
    }

    private void LateUpdate()
    {
        if (followCamera)
        {
            float shiftY = Camera.main.transform.position.y - transform.position.y;
            transform.Translate(0, shiftY, 0);
        }
        else
        {
            float newY = (transform.position.y - Camera.main.transform.position.y) / parallax;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }
}
