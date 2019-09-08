using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    Animator[] animators;

    void Start()
    {
        animators = GetComponentsInChildren<Animator>();
    }

    void Update()
    {
        for (int i = 0; i < animators.GetLength(0); i++)
        {
            if (!animators[i].GetCurrentAnimatorStateInfo(0).IsName("End"))
            {
                // 再生中のアニメーションが存在する
                return;
            }
        }

        // すべてのアニメーション再生が終了したら自身を削除する
        Destroy(gameObject);
    }
}
