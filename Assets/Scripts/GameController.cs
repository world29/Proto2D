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

    [Header("10m登るたび増える進捗度")]
    public float m_progressPerTenMeter = 1;

    [HideInInspector]
    public NotificationObject<float> m_progress;

    // 進捗度の最大値
    // この値に達するとステージクリアとする。
    private float m_maxProgress;

    private GameObject m_player;
    private float m_nextHightToProgress;

    private bool isGameOver;
    private bool isGameClear;

    void Start()
    {
        m_progress = new NotificationObject<float>(0);
        m_progress.OnChanged = OnProgressChanged;
        m_maxProgress = 200;

        m_nextHightToProgress = 10;

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

    private void LateUpdate()
    {
        if (m_player)
        {
            if (m_player.transform.position.y > m_nextHightToProgress)
            {
                Debug.LogFormat("player got progress point :{0} meter", m_nextHightToProgress);

                AddProgressValue(m_progressPerTenMeter);

                m_nextHightToProgress += 10;
            }
        }
    }

    // マップの初期化が終了したときに、MapController から呼ばれる
    // プレイヤーのスポーン位置がマップ生成に依存するため。
    public void OnMapInitialized()
    {
        // 再生時にプレイヤーが存在しなければ、スポーナーの位置にプレイヤーを生成
        m_player = GameObject.FindGameObjectWithTag("Player");
        if (m_player == null)
        {
            GameObject playerSpawner = GameObject.FindGameObjectWithTag("PlayerSpawner");
            if (playerSpawner)
            {
                m_player = GameObject.Instantiate(playerPrefab, playerSpawner.transform.position, Quaternion.identity);
            }
        }
    }

    public void AddProgressValue(float value)
    {
        m_progress.Value += value;
    }

    public void Pause()
    {
        Time.timeScale = 0;
    }

    public void Resume()
    {
        Time.timeScale = 1;
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
        replayText.text = "You died.\nPress \'R\' to replay!";
    }

    public void GameClear()
    {
        isGameClear = true;
        replayText.text = "Congratulations!\nPress \'R\' to replay!";

        Pause();
    }

    void OnProgressChanged(float value)
    {
        if (value >= m_maxProgress)
        {
            GameClear();
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(sceneNameToLoad);

        Resume();
    }
}
