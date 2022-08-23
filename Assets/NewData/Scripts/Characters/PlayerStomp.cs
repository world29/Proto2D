using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Assets.NewData.Scripts
{
    public class PlayerStomp : MonoBehaviour
    {
        [SerializeField]
        private PlayerMove playerMove;

        [SerializeField]
        private float stompJumpForce = 8f;

        private void Awake()
        {
        }

        private void Update()
        {
        }

        public void OnStompHit()
        {
            Debug.Log("Stomp hit!");

            playerMove.Velocity = new Vector2(playerMove.Velocity.x, stompJumpForce);

            playerMove.ChangeStateToStomp();
        }
    }
}
