using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Proto2D
{
    public interface IGameOverReceiver : IEventSystemHandler
    {
        void OnGameOver();
    }
}
