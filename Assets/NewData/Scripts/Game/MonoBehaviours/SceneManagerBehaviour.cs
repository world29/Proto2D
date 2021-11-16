using UnityEngine;
using System.Collections;

using Assets.NewData.Scripts;

public class SceneManagerBehaviour : MonoBehaviour
{
    void Awake()
    {
        // Unity シーンをまたいで存在する
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        var sceneManager = SceneManager.GetInstance();

        sceneManager.ChangeScene(new TitleMenuScene());
    }

    void Update()
    {
        var sceneManager = SceneManager.GetInstance();

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.F10))
        {
            // goto next scene;
            if (sceneManager.CurrentScene is TitleMenuScene)
            {
                sceneManager.ChangeScene(new GameplayScene());
            }
            else if (sceneManager.CurrentScene is GameplayScene)
            {
                sceneManager.ChangeScene(new GameoverMenuScene());
            }
            else if (sceneManager.CurrentScene is GameoverMenuScene)
            {
                sceneManager.ChangeScene(new TitleMenuScene());
            }
            else
            {
                sceneManager.ChangeScene(new TitleMenuScene());
            }

            return;
        }
#endif

        sceneManager.Update(Time.deltaTime);
    }
}
