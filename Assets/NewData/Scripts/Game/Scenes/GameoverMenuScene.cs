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
    }

    public void OnExitScene()
    {
        Debug.Log("Exit GameoverMenuScene");
    }
}
