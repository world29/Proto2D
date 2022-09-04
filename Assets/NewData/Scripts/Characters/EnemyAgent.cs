using UnityEngine;

namespace Assets.NewData.Scripts
{
    public class EnemyAgent : MonoBehaviour
    {
        [SerializeField]
        private Proto2D.AI.BehaviourTree behaviourTree;

        [SerializeField]
        private bool behaviourTreeDebug = false;

        [SerializeField]
        private Proto2D.EnemyBehaviour enemy;

        private Animator _animator;
        private Proto2D.AI.BehaviourTree _runtimeBehaviourTree = null;

        void Awake()
        {
            TryGetComponent(out _animator);

            if (behaviourTree)
            {
                // BehaviourTree は ScriptableObject なのでシングルインスタンス。
                // BehaviourTree の状態を他のオブジェクトと共有したくないのでコピーする。
                // エディタ上で各ノードの状態をリアルタイムに確認したい場合は、behaviourTreeDebug を true に設定し、
                // シングルインスタンスの状態を直接参照・編集する。
                if (behaviourTreeDebug)
                {
                    _runtimeBehaviourTree = behaviourTree;
                }
                else
                {
                    _runtimeBehaviourTree = behaviourTree.Copy() as Proto2D.AI.BehaviourTree;
                }
                _runtimeBehaviourTree.Setup();
            }
        }

        void Update()
        {
            if (_runtimeBehaviourTree)
            {
                _runtimeBehaviourTree.Evaluate(enemy);
            }
        }
    }
}