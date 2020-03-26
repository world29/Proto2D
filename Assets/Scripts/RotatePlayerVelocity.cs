using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePlayerVelocity : MonoBehaviour
{
    public float OffsetRotate = 0f;

    // Start is called before the first frame update
    void Start()
    {
        GameObject context = GameObject.FindGameObjectWithTag("Player");
        if (context == null) { return; }
        PlayerController player = context.GetComponent<PlayerController>();
        if(player)
        {
            float angleDeg = Mathf.Atan2(player.velocity.y, player.velocity.x) * Mathf.Rad2Deg;
            transform.Rotate(new Vector3(0f,0f,angleDeg+OffsetRotate+270));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
