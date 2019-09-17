using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Proto2D.AI
{
    [CustomNodeEditor(typeof(Node))]
    public class StateNodeEditor : XNodeEditor.NodeEditor
    {
        public override void OnHeaderGUI()
        {
            GUI.color = Color.white;
            Node node = target as Node;
            switch (node.GetStatus())
            {
                case NodeStatus.READY:
                    GUI.color = Color.yellow;
                    break;
                case NodeStatus.RUNNING:
                    GUI.color = Color.blue;
                    break;
                case NodeStatus.SUCCESS:
                    GUI.color = Color.green;
                    break;
                case NodeStatus.FAILURE:
                    GUI.color = Color.red;
                    break;
            }
            string title = target.name;
            GUILayout.Label(title, XNodeEditor.NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
            GUI.color = Color.white;
        }

        public override void OnBodyGUI()
        {
            base.OnBodyGUI();
            Node node = target as Node;

            Action actionNode = node as Action;
            if (actionNode && actionNode.GetStatus() == NodeStatus.RUNNING)
            {
                EditorGUI.ProgressBar(new Rect(3, 0, GetWidth() - 6, 5), actionNode.GetProgress(), "");
            }
        }
    }
}
