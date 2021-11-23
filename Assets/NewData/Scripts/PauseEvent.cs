using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.NewData.Scripts
{
    public interface IPauseEvent : IEventSystemHandler
    {
        void OnPauseRequested();

        void OnResumeRequested();
    }
}
