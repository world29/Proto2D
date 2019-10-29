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
            switch (node.PrevStatus)
            {
                case NodeStatus.READY:
                    GUI.color = Color.white;
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
                case NodeStatus.ABORTED:
                    GUI.color = Color.black;
                    break;
            }
            string title = target.name;
            if (node.EvaluationOrder > 0)
            {
                title = string.Format("#{0} {1}", node.EvaluationOrder, target.name);
            }
            GUILayout.Label(title, XNodeEditor.NodeEditorResources.styles.nodeHeader, GUILayout.Height(30));
            GUI.color = Color.white;
        }
    }
}
