using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text replayText;

    [Header("ゲームオーバー時に読み込まれるシーン名")]
    public string sceneNameToLoad;

    private bool isGameOver;
    private bool isGameClear;

    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
        isGameClear = false;
        replayText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver && !isGameClear)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Return))
        {
            SceneManager.LoadScene(sceneNameToLoad);
        }
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public bool IsGameClear()
    {
        return isGameClear;
    }

    public void GameOver()
    {
        isGameOver = true;
        replayText.text = "Hit Enter to replay!";
    }

    public void GameClear()
    {
        isGameClear = true;
        replayText.text = "Congratulations!\nHit Enter to replay!";
    }
}
