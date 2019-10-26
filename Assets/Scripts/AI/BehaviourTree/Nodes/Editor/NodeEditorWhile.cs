using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Proto2D.AI
{
    [CustomNodeEditor(typeof(While))]
    public class StateNodeEditorWhile : StateNodeEditor
    {
        private int m_selected = 0;
        string[] m_options;

        public override void OnCreate()
        {
            base.OnCreate();

            While whileNode = target as While;

            // 引数がなく、戻り値が bool 型の public メンバ関数のみリストアップ
            System.Type type = typeof(EnemyBehaviour);
            var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
            var methods1 = methods.Where(m => m.ReturnType == typeof(bool) && m.GetParameters().Length == 0);

            m_options = methods1.Select(m => m.Name).ToArray();

            int index = m_options.ToList().FindIndex(name => name == whileNode.m_methodName);
            m_selected = index < 0 ? 0 : index;
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            While whileNode = target as While;

            m_selected = EditorGUILayout.Popup("Condition", m_selected, m_options);
            whileNode.m_methodName = m_options[m_selected];
        }
    }
}
