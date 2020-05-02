using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

public class DestroyTest : MonoBehaviour
{
    [SerializeField]
    GameObject m_prefab;

    GameObject m_clone;

    interface IHoge
    {
        void SayHello();
    }

    public class Hoge : Proto2D.Disposable, IHoge
    {
        GameObject m_clone = null;

        public void SayHello()
        {
            Debug.Log("Hello");
        }

        public Hoge(GameObject prefab)
        {
            Debug.Log("Hoge()");

            m_clone = GameObject.Instantiate(prefab);
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                Debug.Log("Hoge.Dispose()");

                // dispose managed resources
                GameObject.Destroy(m_clone);

                Debug.Log("Hoge.Dispose() done.");
            }
            base.Dispose(isDisposing);
        }
    }

    Hoge m_hoge = null;

    private void Start()
    {
#if true
        m_hoge = new Hoge(m_prefab);

        // スペースを押すとデストロイする
        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.Space))
            .Subscribe(_ =>
            {
                if (m_hoge != null) {
                    m_hoge.Dispose();
                    m_hoge = null;
                }
            });
#else
        m_clone = GameObject.Instantiate(m_prefab);

        // スペースを押すとデストロイする
        this.UpdateAsObservable()
            .Where(_ => Input.GetKey(KeyCode.Space))
            .Subscribe(_ =>
            {
                Destroy(gameObject);
            });
#endif
    }

    private void OnDestroy()
    {
        Debug.Log("DestroyTest.OnDestroy()");

        if (m_clone)
        {
            Destroy(m_clone);
        }
    }
}
