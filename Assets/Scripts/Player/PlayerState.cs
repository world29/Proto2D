using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerState
{
    void OnEnter(GameObject context);

    void OnExit(GameObject context);

    IPlayerState Update(GameObject context);
}
