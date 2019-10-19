﻿using System.Collections;
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

    [HideInInspector]
    public NotificationObject<float> m_progress;

    private bool isGameOver;
    private bool isGameClear;

    void Start()
    {
        m_progress = new NotificationObject<float>(0);

        isGameOver = false;
        isGameClear = false;
        if (replayText)
        {
            replayText.text = "";
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

    // マップの初期化が終了したときに、MapController から呼ばれる
    // プレイヤーのスポーン位置がマップ生成に依存するため。
    public void OnMapInitialized()
    {
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

    public void AddProgressValue(float value)
    {
        m_progress.Value += value;
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
