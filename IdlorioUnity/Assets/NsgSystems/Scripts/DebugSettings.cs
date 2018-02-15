using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DebugKit
{
    Example1,
    Example2,
    Count
}

public class DebugSettings : MonoBehaviour
{
    private static DebugSettings _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnValidate()
    {
        if (_debugToggles.Length != (int)DebugKit.Count)
        {
            // We changed the enum size, so we must update the array.
            bool[] temp = _debugToggles;
            _debugToggles = new bool[(int)DebugKit.Count];

            for (int i = 0; i < temp.Length && i < _debugToggles.Length; ++i)
            {
                _debugToggles[i] = temp[i];
            }
        }
    }

    [HideInInspector] public bool[] _debugToggles = new bool[(int)DebugKit.Count];

    public static bool IsActive(DebugKit debugKit)
    {
        if (_instance != null && (int)debugKit < _instance._debugToggles.Length)
        {
            return _instance._debugToggles[(int)debugKit];
        }

        return false;
    }

    public void ToggleDebug(DebugKit debugKit)
    {
        if (_instance != null && (int)debugKit < _instance._debugToggles.Length)
        {
            _instance._debugToggles[(int)debugKit] = !_instance._debugToggles[(int)debugKit];
        }
    }

    public void SetDebugEnabled(DebugKit debugKit, bool enabled)
    {
        if (_instance != null && (int)debugKit < _instance._debugToggles.Length)
        {
            _instance._debugToggles[(int)debugKit] = enabled;
        }
    }
}
