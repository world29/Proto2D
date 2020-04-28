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

            PlayerPrefs.Save();
        }
    }
}
