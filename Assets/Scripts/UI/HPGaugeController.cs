using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;

namespace Proto2D
{
    public class HPGaugeController : MonoBehaviour
    {
        [SerializeField]
        int m_maxHealth;

        [SerializeField]
        int m_currentHealth;

        [SerializeField]
        HPGaugeNodeController m_nodePrefab;

        [SerializeField]
        RectTransform m_content;

        private List<HPGaugeNodeController> m_healthNodes = new List<HPGaugeNodeController>();

        private PlayerController m_player;

        private void Awake()
        {
            // HP の最大値アップ
            this.ObserveEveryValueChanged(x => x.m_maxHealth)
                .DistinctUntilChanged()
                .Subscribe(maxHealth => 
                {
                    while (maxHealth > m_healthNodes.Count) {
                        AddHealthNode();
                    }
                });

            // HP 更新
            this.ObserveEveryValueChanged(x => x.m_currentHealth)
                .DistinctUntilChanged()
                .Pairwise()
                .Subscribe(health =>
                {
                    var diff = health.Current - health.Previous;
                    if (diff > 0)
                    {
                        // 回復
                        for (var i = 0; i < diff; i++)
                        {
                            var index = health.Previous + i;
                            if (0 <= index && index < m_healthNodes.Count)
                            {
                                m_healthNodes[index].Heal();
                            }
                        }
                    }
                    else
                    {
                        // ダメージ
                        for (var i = diff; i < 0; i++)
                        {
                            var index = health.Previous + i;
                            if (0 <= index && index < m_healthNodes.Count)
                            {
                                m_healthNodes[index].Damage();
                            }
                        }
                    }
                });

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void Start()
        {
        }

        private void LateUpdate()
        {
            if (m_player == null)
            {
                var go = GameObject.FindGameObjectWithTag("Player");
                m_player = go.GetComponent<PlayerController>();

                m_maxHealth = (int)m_player.initialHealth;
                m_player.health.OnChanged += OnPlayerHealthChanged;
                OnPlayerHealthChanged(m_player.health.Value);
            }
        }

        private void OnPlayerHealthChanged(float health)
        {
            Debug.LogFormat("player health: {0}", (int)health);
            m_currentHealth = (int)health;
        }

        private void OnSceneLoaded(Scene scn, LoadSceneMode mode)
        {
        }

        private void OnSceneUnloaded(Scene scn)
        {
            m_player = null;
        }

        private void AddHealthNode()
        {
            var clone = GameObject.Instantiate(m_nodePrefab);
            clone.GetComponent<RectTransform>().SetParent(m_content, false);
            m_healthNodes.Add(clone);
        }
    }
}
