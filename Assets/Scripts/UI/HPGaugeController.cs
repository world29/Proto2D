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
        int m_currentShield;

        [SerializeField]
        HPGaugeNodeController m_nodePrefab;

        [SerializeField]
        HPGaugeNodeController m_shieldNodePrefab;

        [SerializeField]
        RectTransform m_hpContainer;

        [SerializeField]
        RectTransform m_shieldContainer;

        private List<HPGaugeNodeController> m_healthNodes = new List<HPGaugeNodeController>();
        private List<HPGaugeNodeController> m_shieldNodes = new List<HPGaugeNodeController>();

        private PlayerHealth m_playerHealth;
        private PlayerShield m_playerShield;

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

            // シールド更新
            this.ObserveEveryValueChanged(x => x.m_currentShield)
                .DistinctUntilChanged()
                .Subscribe(shield =>
                {
                    // 増えたとき
                    while (shield > m_shieldNodes.Count)
                    {
                        AddShieldNode();
                    }
                    // 減ったとき
                    while (shield < m_shieldNodes.Count)
                    {
                        var item = m_shieldNodes[m_shieldNodes.Count - 1];
                        m_shieldNodes.Remove(item);

                        // ダメージアニメーション終了後にノードを削除する
                        Observable.FromCoroutine(item.DamageAnimationFlow)
                            .Subscribe(_ => item.transform.parent = null);
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
            if (m_playerHealth == null)
            {
                var playerObject = GameObject.FindGameObjectWithTag("Player");
                m_playerHealth = playerObject.GetComponent<PlayerHealth>();
                m_playerShield = playerObject.GetComponent<PlayerShield>();

                m_playerHealth.maxHealth
                    .Subscribe(maxHp => m_maxHealth = (int)maxHp)
                    .AddTo(playerObject);

                m_playerHealth.currentHealth
                    .Subscribe(hp => m_currentHealth = (int)hp)
                    .AddTo(playerObject);

                m_playerShield.currentShield
                    .Subscribe(shield => m_currentShield = shield)
                    .AddTo(playerObject);
            }
        }

        private void OnSceneLoaded(Scene scn, LoadSceneMode mode)
        {
        }

        private void OnSceneUnloaded(Scene scn)
        {
            m_playerHealth = null;
        }

        private void AddHealthNode()
        {
            var hpNode = GameObject.Instantiate(m_nodePrefab);
            hpNode.GetComponent<RectTransform>().SetParent(m_hpContainer, false);
            m_healthNodes.Add(hpNode);

            //TODO: HP ゲージの初期状態をダメージ状態にする
            hpNode.Damage();
        }

        private void AddShieldNode()
        {
            var clone = GameObject.Instantiate(m_shieldNodePrefab);
            clone.GetComponent<RectTransform>().SetParent(m_shieldContainer, false);
            m_shieldNodes.Add(clone);
        }
    }
}
