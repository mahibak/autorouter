using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class manages all our singletons, making sure they're created and calling appropriate updates.
/// </summary>
public class SingletonManager : MonoBehaviour
{
    private static SingletonManager _instance;

	private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        GDK.CreateInstance();
        MachineManager.CreateInstance();
        InputManager.CreateInstance();
    }

    private void OnEnable()
    {
        Camera.onPreCull += DrawObjects;
    }

    private void OnDisable()
    {
        Camera.onPreCull -= DrawObjects;
    }

    private void Update()
    {
        GDK.GetInstance().Update();
        MachineManager.GetInstance().Update();
    }

    private void DrawObjects(Camera camera)
    {
        GDK.GetInstance().DrawObjects();
    }

    private void OnGUI()
    {
        GDK.GetInstance().OnGUI();
    }
}
