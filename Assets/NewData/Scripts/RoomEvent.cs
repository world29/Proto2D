using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IRoomEvent : IEventSystemHandler
{
    void OnRoomGenerated(string name, Bounds bounds, int group);

    void OnRoomEnemySpawned(IReadOnlyCollection<GameObject> enemyObjects, int group);

    //void OnRoomDeleted(string name);
}
