using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.NewData.Scripts
{
    // PlatformBehaviour は Unity の実行順序設定で DefaultTime より後に設定する。
    // プレイヤーや敵など動くアクターが DefaultTime で OnPassengerStay を呼び出した後に、Update, LateUpdate を処理する。
    public abstract class PlatformBehaviour : MonoBehaviour, IPlatform
    {
        private HashSet<Transform> _passengers;
        private HashSet<Transform> _passengersPrevFrame;

        protected abstract void OnPassengerEnter(Transform passenger);
        protected abstract void OnPassengerExit(Transform passenger);
        protected abstract void OnPassengerStay(Transform passenger);

        // IPlatform
        public void OnLandingPlatform(Transform passenger)
        {
            _passengers.Add(passenger);
        }

        protected void Awake()
        {
            _passengers = new HashSet<Transform>();
            _passengersPrevFrame = new HashSet<Transform>();
        }

        protected void Update()
        {
            // 前フレームでは乗っていたが今フレームでは乗っていない (降りた)オブジェクトに対する処理
            foreach (var passenger in _passengersPrevFrame)
            {
                if (!_passengers.Contains(passenger))
                {
                    OnPassengerExit(passenger);
                }
            }

            // 上に乗っているものに対して処理を行う
            foreach (var passenger in _passengers)
            {
                if (!_passengersPrevFrame.Contains(passenger))
                {
                    OnPassengerEnter(passenger);
                }
                else
                {
                    OnPassengerStay(passenger);
                }
            }
        }

        protected void LateUpdate()
        {
            // 上に乗っているオブジェクトリストをクリアする
            _passengersPrevFrame = new HashSet<Transform>(_passengers);
            _passengers.Clear();
        }
    }
}
