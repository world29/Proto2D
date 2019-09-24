using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Proto2D
{
    public class PhysicsBox
    {
        Rect m_rect;
        GameObject m_object;
        Room m_room;

        public PhysicsBox(Rect rect)
        {
            m_rect = rect;

            Init();
        }

        public void Init()
        {
            m_object = new GameObject("PhysicsBox");
            m_object.transform.position = new Vector3(m_rect.center.x, m_rect.center.y, 0);

            Rigidbody rigidbody = m_object.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.drag = .1f;
            rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

            BoxCollider collider = m_object.AddComponent<BoxCollider>();
            collider.size = new Vector3(m_rect.size.x, m_rect.size.y, 1);

            m_room = m_object.AddComponent<Room>();
        }

        public void Destroy()
        {
            GameObject.DestroyImmediate(m_object);
        }
    }

    public class ProceduralDungeonGeneratorWindow : EditorWindow
    {
        List<PhysicsBox> m_rooms = new List<PhysicsBox>();

        // 部屋を生成する空間の半径
        float radius = 150;

        // 部屋の幅の平均値
        float roomWidthAve = 50;
        // 部屋の高さの平均値
        float roomHeightAve = 50;
        // 部屋の広さの分散
        float roomSigma = 1;

        // 乱数生成器
        private RandomBoxMuller m_random;


        [MenuItem("Window/ProceduralDungeonGenerator")]
        static void Open()
        {
            EditorWindow.GetWindow<ProceduralDungeonGeneratorWindow>("ProceduralDungeonGenerator");
        }

        void OnReset()
        {
            m_random = new RandomBoxMuller();

            m_rooms.ForEach(item => item.Destroy());

            m_rooms.Clear();
        }

        void OnGenerate()
        {
            if (m_random == null)
            {
                OnReset();
            }

            // 幅と高さを持つ部屋を円の中にランダムに配置
            Vector2 center = m_random.Source.insideUnitCircle * radius;

            // 各部屋のサイズを指定するのに正規分布を使用する
            Vector2 size;
            size.x = m_random.Next(roomWidthAve, roomSigma, true);
            size.y = m_random.Next(roomHeightAve, roomSigma, false);

            m_rooms.Add(new PhysicsBox(new Rect(center, size)));
        }

        void OnSimulate(int stepCount = 1)
        {
            Physics.autoSimulation = false;
            for (int i = 0; i < stepCount; i++)
            {
                Physics.Simulate(Time.fixedDeltaTime);
            }
            Physics.autoSimulation = true;
        }

        private void Update()
        {
            Repaint();
        }

        void OnGUI()
        {
            radius = EditorGUILayout.Slider("radius", radius, 1, 50.0f);
            roomWidthAve = EditorGUILayout.Slider("width", roomWidthAve, 1, 10.0f);
            roomHeightAve = EditorGUILayout.Slider("height", roomHeightAve, 1, 10.0f);
            roomSigma = EditorGUILayout.Slider("sigma", roomSigma, 1, 10.0f);

            if (GUILayout.Button("Reset"))
                OnReset();

            if (GUILayout.RepeatButton("Generate"))
                OnGenerate();

            if (GUILayout.RepeatButton("Simulate"))
                OnSimulate();
        }
    }
}
