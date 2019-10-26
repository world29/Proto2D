using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Proto2D.AI
{
    [CreateNodeMenu("BehaviourTree/Action/SetProperty")]
    public class SetProperty : Action
    {
        [Tooltip("値を文字列で入力してください。\n次の型に対応: int, float, bool, string")]
        public string m_valueString;

        [HideInInspector, SerializeField]
        public string m_propertyName;

        public override NodeStatus Evaluate(EnemyBehaviour enemyBehaviour)
        {
            System.Type type = typeof(EnemyBehaviour);
            System.Reflection.FieldInfo fieldInfo = type.GetField(m_propertyName);
            if (fieldInfo != null)
            {
                if (fieldInfo.FieldType == typeof(int))
                {
                    int value;
                    if (int.TryParse(m_valueString, out value))
                    {
                        fieldInfo.SetValue(enemyBehaviour, value);
                    }
                }
                else if (fieldInfo.FieldType == typeof(float))
                {
                    float value;
                    if (float.TryParse(m_valueString, out value))
                    {
                        fieldInfo.SetValue(enemyBehaviour, value);
                    }
                }
                else if (fieldInfo.FieldType == typeof(bool))
                {
                    bool value;
                    if (bool.TryParse(m_valueString, out value))
                    {
                        fieldInfo.SetValue(enemyBehaviour, value);
                    }
                }
                else if (fieldInfo.FieldType == typeof(string))
                {
                    fieldInfo.SetValue(enemyBehaviour, m_valueString);
                }
            }

            return NodeStatus.SUCCESS;
        }
    }
}
