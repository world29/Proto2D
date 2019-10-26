using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Proto2D.AI
{
    [CustomNodeEditor(typeof(SetProperty))]
    public class StateNodeEditorSetProperty : StateNodeEditor
    {
        private int m_selected = 0;
        string[] m_options;

        public override void OnCreate()
        {
            base.OnCreate();

            SetProperty node = target as SetProperty;

            // パブリックなプロパティをリストアップ
            System.Type type = typeof(EnemyBehaviour);
            var props = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);

            m_options = props.Select(m => m.Name).ToArray();

            int index = m_options.ToList().FindIndex(name => name == node.m_propertyName);
            m_selected = index < 0 ? 0 : index;
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            SetProperty node = target as SetProperty;

            m_selected = EditorGUILayout.Popup("Property", m_selected, m_options);
            node.m_propertyName = m_options[m_selected];
        }
    }
}
