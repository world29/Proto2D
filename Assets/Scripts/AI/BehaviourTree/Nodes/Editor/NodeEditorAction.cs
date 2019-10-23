using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Proto2D.AI
{
    [CustomNodeEditor(typeof(Action))]
    public class StateNodeEditorAction : StateNodeEditor
    {
        public override void OnBodyGUI()
        {
            base.OnBodyGUI();

            //　プログレスバーで progress を表示
            EditorGUILayout.Space();

            float progress = 0;
            Action actionNode = target as Action;
            if (actionNode.GetStatus() == NodeStatus.RUNNING)
            {
                progress = actionNode.GetProgress();
            }
            Rect rect = GUILayoutUtility.GetRect(20, 5);
            EditorGUI.ProgressBar(rect, progress, "");
        }
    }
}
