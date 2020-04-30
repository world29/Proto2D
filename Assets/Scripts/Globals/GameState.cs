using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace Proto2D
{
    // ゲームステートはゲーム中にひとつだけ存在し、ゲームの進捗状況などを管理する
    public class GameState : SingletonMonoBehaviour<GameState>
    {
        Dictionary<string, bool> m_boolDict = new Dictionary<string, bool>();
        Dictionary<string, int> m_intDict = new Dictionary<string, int>();

        private static readonly string KEY_COIN_COUNT = "coins";

        // ステージクリアフラグを設定
        public void SetStageCompleted(string stageName, bool completed)
        {
            SetBool(stageName, completed);
        }

        // ステージクリアフラグを取得
        public bool GetStageCompleted(string stageName)
        {
            return GetBool(stageName);
        }

        // コイン数を設定
        public void SetCoinCount(int count)
        {
            SetInt(KEY_COIN_COUNT, count);
        }

        // コイン数を取得
        public int GetCoinCount()
        {
            return GetInt(KEY_COIN_COUNT);
        }

        public void ResetAll()
        {
            m_intDict.Clear();
            m_boolDict.Clear();

            PlayerPrefs.DeleteAll();
        }

        void SetInt(string key, int value)
        {
            m_intDict[key] = value;
        }

        int GetInt(string key, int defaultValue = 0)
        {
            if (!m_intDict.ContainsKey(key))
            {
                int tempValue = PlayerPrefs.GetInt(key, defaultValue);
                m_intDict[key] = tempValue;
            }
            return m_intDict[key];
        }

        void SetBool(string key, bool value)
        {
            m_boolDict[key] = value;
        }

        bool GetBool(string key, bool defaultValue = false)
        {
            // 辞書に存在しなければ PlayerPrefs を見に行く
            if (!m_boolDict.ContainsKey(key))
            {
                int tempValue = PlayerPrefs.GetInt(key, defaultValue ? 1 : 0);
                m_boolDict[key] = (tempValue == 1);
            }
            return m_boolDict[key];
        }

        private void OnApplicationQuit()
        {
            // アプリケーション終了時に辞書の内容を PlayerPrefs に保存
            foreach (var key in m_boolDict.Keys)
            {
                PlayerPrefs.SetInt(key, m_boolDict[key] ? 1 : 0);
            }
            foreach (var key in m_intDict.Keys)
            {
                PlayerPrefs.SetInt(key, m_intDict[key]);
            }

            PlayerPrefs.Save();
        }
    }
}
