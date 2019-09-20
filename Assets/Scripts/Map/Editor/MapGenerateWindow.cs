using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PhysicsBox
{
    Rect m_rect;
    GameObject m_object;
    public Rigidbody2D m_rigidbody;
    BoxCollider2D m_collider;
    int m_maxIteration = 1000;

    public PhysicsBox(Rect rect)
    {
        m_rect = rect;

        Init();
    }

    public void Init()
    {
        m_object = new GameObject("Boxies/PhysBox");
        m_object.transform.position = new Vector3(m_rect.center.x, m_rect.center.y, 0);

        m_rigidbody = m_object.AddComponent<Rigidbody2D>();
        m_collider = m_object.AddComponent<BoxCollider2D>();
        m_collider.size = m_rect.size;
    }

    public void RunSimulation()
    {
        Physics.autoSimulation = false;
        for (int i = 0; i < m_maxIteration; i++)
        {
            Physics2D.Simulate(Time.fixedDeltaTime);

        }
        Physics.autoSimulation = true;
    }

    public void DrawBox()
    {
        GUI.backgroundColor = Color.blue;
        GUI.Box(new Rect(m_object.transform.position, m_collider.size), "");
        GUI.color = Color.white;
    }
}

public struct Room
{
    public Rect rect;
}

public class MapGenerateWindow : ZoomTestWindow
{
    List<PhysicsBox> m_rooms = new List<PhysicsBox>();

    float radius = 150;

    float roomWidthAve = 50;
    float roomHeightAve = 50;
    float roomDistribSigma = 1;

    Rect areaRect = new Rect(100, 100, 400, 400);
    Vector2 areaCenter = new Vector2(200, 200);

    private Proto2D.RandomBoxMuller m_random;

    [MenuItem("Window/Map Generate")]
    private static void Init()
    {
        MapGenerateWindow window = EditorWindow.GetWindow<MapGenerateWindow>(false, "Map Generate");
        window.minSize = new Vector2(600.0f, 300.0f);
        window.wantsMouseMove = true;
        window.Show();
        EditorWindow.FocusWindowIfItsOpen<MapGenerateWindow>();
    }

    void OnReset()
    {
        m_random = new Proto2D.RandomBoxMuller();

        m_rooms.Clear();
    }

    void OnGenerateRooms()
    {
        if (m_random == null)
        {
            OnReset();
        }

        // 幅と高さを持つ部屋を円の中にランダムに配置
        Vector2 center = m_random.Source.insideUnitCircle * radius;

        // 各部屋のサイズを指定するのに正規分布を使用する
        Vector2 size;
        size.x = m_random.Next(roomWidthAve, roomDistribSigma, true);
        size.y = m_random.Next(roomHeightAve, roomDistribSigma, false);

        PhysicsBox room = new PhysicsBox(new Rect(center, size));
        m_rooms.Add(room);
    }

    private void Update()
    {
        Repaint();
    }

    void OnGUI()
    {
        base.OnGUI();

        roomDistribSigma = EditorGUILayout.Slider("sigma", roomDistribSigma, 1, 100.0f);

        if (GUILayout.Button("Reset"))
        {
            OnReset();
        }

        if (GUILayout.RepeatButton("Generate"))
        {
            OnGenerateRooms();
        }

        if (GUILayout.RepeatButton("Simulate"))
        {
            Physics2D.autoSimulation = false;
            //for (int i = 0; i < 1; i++)
            {
                Physics2D.Simulate(Time.fixedDeltaTime);
            }
            Physics2D.autoSimulation = true;
        }

        GUI.backgroundColor = Color.green;
        GUI.Box(areaRect, m_rooms.Count.ToString());
        GUI.color = Color.white;

        EditorZoomArea.Begin(_zoom, areaRect);
        {
            //
            foreach(PhysicsBox room in m_rooms)
            {
                room.DrawBox();
            }
        }
        EditorZoomArea.End();

    }
}
