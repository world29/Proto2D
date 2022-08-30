﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
                if (instance == null)
                {
                    Debug.LogWarning(typeof(T) + "is nothing");
                }
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (CheckInstance())
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    protected bool CheckInstance()
    {
        if (this == Instance) { return true; }

        Destroy(gameObject);

        return false;
    }
}