// 参考 URL
// http://carving.roguelive.chicappa.jp/?eid=44

using UnityEngine;
using UnityEditor;

class PrefabReplacement : EditorWindow
{
    string[] prefabs;
    int selected;
    Vector2 scrollPosition;

    // プレハブ差し替え
    [MenuItem("Assets/PrefabReplacement %&h")]
    static void ReplacementPrefabs()
    {
        GetWindow<PrefabReplacement>();
    }

    // 起動
    void Awake()
    {
        // プレハブをリストアップ
        this.prefabs = System.IO.Directory.GetFiles(
    Application.dataPath, "*.prefab", System.IO.SearchOption.AllDirectories);

        // ディスクのパスからアセットパスに変換
        for (int i = 0, last = this.prefabs.Length; i < last; ++i)
        {
            this.prefabs[i] =
            "Assets" + this.prefabs[i].Substring(Application.dataPath.Length);
        }
        this.selected = 0;
        this.scrollPosition = Vector2.zero;
    }

    // ウィンドウの処理
    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();
        this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition);
        this.selected = GUILayout.SelectionGrid(this.selected, this.prefabs, 1);
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.Space(20);
        if (GUILayout.Button("OK"))
        {
            Replacement(this.prefabs[this.selected]);
        }
        GUILayout.Space(10);
        if (GUILayout.Button("Close"))
        {
            this.Close();
        }
    }

    // プレハブを置換
    public static void Replacement(string path)
    {
        // 差し替えるプレハブを読み込み
        GameObject newPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

        // ヒエラルキーウィンドウで選択されているGameObjectを順に処理
        foreach (GameObject obj in Selection.gameObjects)
        {
            // 新しいプレハブをインスタンス化
            GameObject newObj =
      PrefabUtility.InstantiatePrefab(newPrefab) as GameObject;

            // 差し替え前のGameObjectの情報をコピー
            newObj.transform.position = obj.transform.position;
            newObj.transform.eulerAngles = obj.transform.eulerAngles;
            newObj.transform.SetParent(obj.transform.parent);

            // 古いプレハブ削除
            DestroyImmediate(obj);
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
        UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }
}
