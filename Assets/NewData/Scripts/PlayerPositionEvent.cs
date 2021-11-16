using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IPlayerPositionEvent : IEventSystemHandler
{
    void OnChangePlayerPosition(Vector3 position);
}
