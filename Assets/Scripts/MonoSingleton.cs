using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class MonoSingleton<T>:MonoBehaviour where T : MonoBehaviour
{
    private static T _value = null;

    public static T Instance
    {
        get => _value;
    }


    protected virtual void Awake()
    {
        if (object.ReferenceEquals(null, MonoSingleton<T>._value))
        {
            MonoSingleton<T>._value = this as T;
            GameObject.DontDestroyOnLoad(this);

        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}

