using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PhysicsSimulation : MonoBehaviour
{
    public int maxIteration = 1000;

    [ContextMenu("Run Simulation")]
    public void RunSimulation()
    {
        Rigidbody[] simulatedBodies = GameObject.FindObjectsOfType<Rigidbody>();

        Physics.autoSimulation = false;
        for (int i = 0; i < maxIteration; i++)
        {
            if (simulatedBodies.All(rb => rb.IsSleeping()))
            {
                print(i);
                break;
            }
            Physics.Simulate(Time.fixedDeltaTime);
        }
        Physics.autoSimulation = true;
    }
}
