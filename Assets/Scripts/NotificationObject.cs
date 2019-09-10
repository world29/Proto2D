using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class NotificationObject<T>
{
    public delegate void NotificationAction(T t);

    private T data;

    public NotificationObject()
    {
        Value = default(T);
    }

    public NotificationObject(T t)
    {
        Value = t;
    }

    ~NotificationObject()
    {
        Dispose();
    }

    public UnityAction<T> OnChanged;

    public T Value
    {
        get
        {
            return data;
        }
        set
        {
            data = value;
            Notify();
        }
    }

    private void Notify()
    {
        if (OnChanged != null)
            OnChanged(data);
    }

    public void Dispose()
    {
        OnChanged = null;
    }
}
