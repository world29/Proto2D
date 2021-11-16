using System.Collections;
using System.Collections.Generic;

namespace Assets.NewData.Scripts {

public interface IScene
{
    void OnUpdate(float deltaTime); 
    void OnEnterScene();
    void OnExitScene();
}

public class SceneManager
{
    public IScene CurrentScene { get; private set; }

    ~SceneManager()
    {
        CurrentScene.OnExitScene();
    }

    public void ChangeScene(IScene nextScene)
    {
        if (CurrentScene != null)
        {
            CurrentScene.OnExitScene();
        }

        if (nextScene != null)
        {
            nextScene.OnEnterScene();
        }

        CurrentScene = nextScene;
    }

    public void Update(float deltaTime)
    {
        if (CurrentScene != null)
        {
            CurrentScene.OnUpdate(deltaTime);
        }
    }

    // singleton
    private static SceneManager Instance = new SceneManager();
    public static SceneManager GetInstance()
    {
        return Instance;
    }
}

}
