using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class JumpRing : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem jumpRingParticle;

        private IPlayerMove _playerMove;

        private void Awake()
        {
            Transform parent = transform.parent;
            _playerMove = parent.GetComponent<IPlayerMove>();
        }

        private void LateUpdate()
        {
            if (_playerMove.IsJumpPerformed)
            {
                if (_playerMove.Velocity.x != 0)
                {
                    float angleRad = Mathf.Atan2(_playerMove.Velocity.y, _playerMove.Velocity.x);
                    transform.rotation = Quaternion.Euler(0, 0, angleRad * Mathf.Rad2Deg + 90);
                }
                else
                {
                    transform.rotation = Quaternion.identity;
                }

                jumpRingParticle.Play();
            }
        }
    }
}
