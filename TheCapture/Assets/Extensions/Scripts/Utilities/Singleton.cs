using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Instead of destroying any new instances, it overrides the current instance. This is handy for
/// resetting the state and save you doing it manually.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }
    protected virtual void Awake() => Instance = this as T;

    protected void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}

/// <summary>
/// This transforms the static instance into a basic singleton. This will destroy ane new
/// versions created, leaving the original instance intact.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if(Instance != null) Destroy(gameObject);
        base.Awake();
    }
}

/// <summary>
/// Persistent version of the singleton. This will survive through scene loads.
/// Perfect for system classes which require stateful, persistent data, or audio sources.
/// where music play through loading screens.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
}
