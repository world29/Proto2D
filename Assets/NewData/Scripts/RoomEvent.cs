using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IRoomEvent : IEventSystemHandler
{
    void OnRoomGenerated(string name, Bounds bounds);

    //void OnRoomDeleted(string name);
}
