using UnityEngine;
using System.Collections;

public class GameoverMenuScene : Assets.NewData.Scripts.IScene
{
    public void OnUpdate(float deltaTime)
    {
    }

    public void OnEnterScene()
    {
        Debug.Log("Enter GameoverMenuScene");

        CoroutineHandler.StartStaticCoroutine(LoadSceneCoroutine());
    }

    private IEnumerator LoadSceneCoroutine()
    {
        yield return ScreenFader.GetInstance().FadeOut(0.5f);

        var op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("GameoverMenu");
        op.completed += OnSceneLoaded;
    }

    private void OnSceneLoaded(AsyncOperation obj)
    {
        Debug.Log("UnityEngine.SceneManagement.SceneManager.LoadSceneAsync.completed");

        CoroutineHandler.StartStaticCoroutine(ScreenFader.GetInstance().FadeIn(0.5f));
    }

    public void OnExitScene()
    {
        Debug.Log("Exit GameoverMenuScene");
    }
}
