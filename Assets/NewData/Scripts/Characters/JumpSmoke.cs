using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class JumpSmoke : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem jumpSmokeParticle;

        [SerializeField]
        private float minSpeed = 5f;

        private IPlayerMove _playerMove;
        private bool _prevGround;
        private float _prevSpeedY;

        void Awake()
        {
            _playerMove = transform.parent.GetComponent<IPlayerMove>();
            _prevGround = true;
        }

        void LateUpdate()
        {
            if (!_prevGround && _playerMove.IsGround && Mathf.Abs(_prevSpeedY) > minSpeed)
            {
                jumpSmokeParticle.Play();
            }

            _prevGround = _playerMove.IsGround;
            _prevSpeedY = _playerMove.Velocity.y;
        }
    }
}
