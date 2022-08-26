using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Assets.NewData.Scripts
{
    public class AreaTrigger : MonoBehaviour
    {
        [SerializeField]
        private Cinemachine.CinemachineVirtualCamera areaCamera;

        private int _initialPriority;

        private static int priority = 100;

        void Awake()
        {
            _initialPriority = areaCamera.Priority;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                areaCamera.Priority = priority++;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
        }
    }
}
