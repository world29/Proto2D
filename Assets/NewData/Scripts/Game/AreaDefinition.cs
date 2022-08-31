using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Assets.NewData.Scripts
{
    [ExecuteAlways]
    public class AreaDefinition : MonoBehaviour
    {
        [SerializeField]
        private RectInt rect;

        [SerializeField]
        private Cinemachine.CinemachineVirtualCamera vcam;

        [SerializeField]
        private BoxCollider2D cameraBounding;

        [SerializeField]
        private BoxCollider2D triggerVolume;

        private static int priority = 100;

        private void FitBoundings()
        {
            if (cameraBounding)
            {
                cameraBounding.size = rect.size;
                cameraBounding.offset = rect.center;
            }

            if (triggerVolume)
            {
                triggerVolume.size = rect.size;
                triggerVolume.offset = rect.center;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                vcam.Priority = priority++;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            FitBoundings();
        }
#endif
    }
}
