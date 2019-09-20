using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public struct Room
{
    public Rect rect;
}

public class MapGenerateWindow : ZoomTestWindow
{
    List<Room> m_rooms = new List<Room>();

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
        size.y = m_random.Next(roomWidthAve, roomDistribSigma, false);

        Room room = new Room();
        room.rect = new Rect(center, size);

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

        GUI.backgroundColor = Color.green;
        GUI.Box(areaRect, m_rooms.Count.ToString());
        GUI.color = Color.white;

        EditorZoomArea.Begin(_zoom, areaRect);
        {
            //
            foreach(Room room in m_rooms)
            {
                Rect rect = new Rect(room.rect.center + areaCenter, room.rect.size);

                GUI.backgroundColor = Color.blue;
                GUI.Box(rect, "");
                GUI.color = Color.white;
            }
        }
        EditorZoomArea.End();

    }
}
