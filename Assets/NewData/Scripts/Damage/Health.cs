using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class Health : MonoBehaviour
    {
        [SerializeField]
        private float hp = 10f;

        // スクリプトからコールバックを登録するためのプロパティ
        public UnityEngine.Events.UnityAction OnHealthZero;

        public void ChangeHealth(float amountToChange)
        {
            hp += amountToChange;

            if (hp <= 0)
            {
                OnHealthZero?.Invoke();
            }
        }
    }
}
