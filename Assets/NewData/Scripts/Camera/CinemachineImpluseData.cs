using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Assets.NewData.Scripts
{
    [CreateAssetMenu(fileName = "CinemachineImpluseData", menuName = "Proto2D/CinemachineImpluseData", order = 0)]
    public class CinemachineImpluseData : ScriptableObject
    {
        static readonly Vector3 k_DefaultVelocity = Vector3.down;

        [SerializeField, CinemachineImpulseDefinitionProperty]
        CinemachineImpulseDefinition m_ImpulseDefinition = new CinemachineImpulseDefinition();

        public CinemachineImpulseDefinition Definition => m_ImpulseDefinition;

        public void GenerateImpluse(Vector3 position)
        {
            m_ImpulseDefinition.CreateEvent(position, k_DefaultVelocity);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_ImpulseDefinition.OnValidate();
        }
#endif
    }
}
