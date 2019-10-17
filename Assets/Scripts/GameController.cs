using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : SingletonMonoBehaviour<GameController>
{
    public Text replayText;

    [Header("ゲームオーバー時に読み込まれるシーン名")]
    public string sceneNameToLoad;

    [Header("プレイヤー (再生時にスポーン)")]
    public GameObject playerPrefab;

    private bool isGameOver;
    private bool isGameClear;

    void Start()
    {
        isGameOver = false;
        isGameClear = false;
        if (replayText)
        {
            replayText.text = "";
        }

        // 再生時にプレイヤーが存在しなければ、スポーナーの位置にプレイヤーを生成
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            GameObject playerSpawner = GameObject.FindGameObjectWithTag("PlayerSpawner");
            if (playerSpawner)
            {
                GameObject.Instantiate(playerPrefab, playerSpawner.transform.position, Quaternion.identity);
            }
        }
    }

    void Update()
    {
        // デバッグビルド時、R キーを押すとシーンをリロードする
        if (Debug.isDebugBuild && Input.GetKey(KeyCode.R))
        {
            ReloadScene();
        }

        if (!isGameOver && !isGameClear)
        {
            return;
        }

        if (Input.touchCount > 0)
        {
            ReloadScene();
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

    private void ReloadScene()
    {
        SceneManager.LoadScene(sceneNameToLoad);
    }
}
