using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    public class InputMonitor : MonoBehaviour
    {
        private void Update()
        {
            var inputProvider = ServiceLocatorProvider.Instance.Current.Resolve<IInputProvider>();

            var moveDirection = inputProvider.GetMoveDirection();
            if (moveDirection != Vector2.zero)
            {
                Debug.LogFormat("[InputMonitor] move: {0}", moveDirection.ToString());
            }

            if (inputProvider.GetJump())
            {
                Debug.Log("[InputMonitor] jump");
            }

            Vector2 attackDirection = Vector2.zero;
            if (inputProvider.GetAttack(out attackDirection))
            {
                Debug.LogFormat("[InputMonitor] attack: {0}", attackDirection.ToString());
            }
        }
    }
}
