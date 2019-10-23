using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Proto2D
{
    [CustomPropertyDrawer(typeof(RandomValue))]
    public class RandomValueEditor : PropertyDrawer
    {
        bool m_expanded = false;

        public override void OnGUI(Rect position, SerializedProperty i_property, GUIContent i_label)
        {
            //EditorGUI.PropertyField(position, property, label, true);

            Rect pos = position;
            pos.height = EditorGUIUtility.singleLineHeight;

            i_property.isExpanded = EditorGUI.Foldout(pos, i_property.isExpanded, i_label);
            m_expanded = i_property.isExpanded;
            if (!i_property.isExpanded)
            {
                return;
            }

            pos.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.indentLevel++;
            {
                RandomValue.ValueType randomValueType;
                {
                    var prop = i_property.FindPropertyRelative("m_type");
                    var prop_label = new GUIContent(prop.displayName);
                    EditorGUI.PropertyField(pos, prop, prop_label, true);
                    float height = EditorGUI.GetPropertyHeight(prop, prop_label, true);

                    randomValueType = (RandomValue.ValueType)prop.enumValueIndex;

                    pos.y += height;
                }
                {
                    if (randomValueType == RandomValue.ValueType.Range)
                    {
                        // horizontal layout
                        EditorGUIUtility.labelWidth = 50;
                        var rect = new Rect(pos.x, pos.y, pos.width / 2, pos.height);

                        float height = 0;
                        {
                            var prop = i_property.FindPropertyRelative("m_min");
                            var prop_label = new GUIContent(prop.displayName);
                            EditorGUI.PropertyField(rect, prop, prop_label, true);
                            height = EditorGUI.GetPropertyHeight(prop, prop_label, true);
                        }
                        rect.x += rect.width;
                        {
                            var prop = i_property.FindPropertyRelative("m_max");
                            var prop_label = new GUIContent(prop.displayName);
                            EditorGUI.PropertyField(rect, prop, prop_label, true);
                            height = EditorGUI.GetPropertyHeight(prop, prop_label, true);
                        }
                        pos.y += height;
                    }
                    else // ValueType.Constant
                    {
                        var prop = i_property.FindPropertyRelative("m_value");
                        var prop_label = new GUIContent(prop.displayName);
                        EditorGUI.PropertyField(pos, prop, prop_label, true);
                        float height = EditorGUI.GetPropertyHeight(prop, prop_label, true);
                        pos.y += height;
                    }
                }
            }
            EditorGUI.indentLevel--;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!m_expanded)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            return EditorGUIUtility.singleLineHeight * 3;
        }
    }
}
