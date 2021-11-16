using UnityEngine;
using System.Collections;

public class TitleMenuScene : Assets.NewData.Scripts.IScene
{
    const string kTitleCanvasPrefabPath = "TitleCanvas";

    private GameObject m_titleCanvasPrefab;

    public void OnUpdate(float deltaTime)
    {
    }

    public void OnEnterScene()
    {
        Debug.Log("Enter TitleMenuScene");

        CoroutineHandler.StartStaticCoroutine(LoadSceneCoroutine());
    }

    private IEnumerator LoadSceneCoroutine()
    {
        ResourceRequest request = Resources.LoadAsync(kTitleCanvasPrefabPath);

        yield return ScreenFader.GetInstance().FadeOut(0.5f);

        while (!request.isDone)
        {
            yield return null;
        }

        m_titleCanvasPrefab = request.asset as GameObject;

        var op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("TitleMenu");
        op.completed += OnSceneLoaded;
    }

    private void OnSceneLoaded(AsyncOperation obj)
    {
        Debug.Log("UnityEngine.SceneManagement.SceneManager.LoadSceneAsync.completed");

        GameObject.Instantiate(m_titleCanvasPrefab);

        CoroutineHandler.StartStaticCoroutine(ScreenFader.GetInstance().FadeIn(0.5f));
    }

    public void OnExitScene()
    {
        Debug.Log("Exit TitleMenuScene");
    }
}
