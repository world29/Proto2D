using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyState
{
    void OnEnter(GameObject context);

    void OnExit(GameObject context);

    IEnemyState Update(GameObject context);
}
