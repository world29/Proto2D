using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyMovement
{
    Vector3 CalculateVelocity(Vector3 prevVelocity, float gravity);
}
