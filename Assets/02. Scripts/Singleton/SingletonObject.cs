using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SingletonObject<T> : MonoBehaviour where T : MonoBehaviour
{
    private static readonly object _lock = new object();

    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    GameObject obj = GameObject.Find(typeof(T).Name);

                    if (obj == null)
                    {
                        obj = new GameObject(typeof(T).Name);
                        _instance = obj.AddComponent<T>();
                    }
                    else
                    {
                        _instance = obj.GetComponent<T>();
                    }

                    DontDestroyOnLoad(obj);
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;

            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
