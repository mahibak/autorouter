using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactorySizeManager : MonoBehaviour
{
    private static FactorySizeManager _instance;

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
    
    private Vector3 _factorySize = new Vector3(40, 0, 20);
    private Vector3 _machineEditorSize = new Vector3(20, 0, 10);
    private bool _isEditingMachine = false;

    public static void SetIsEditingMachine(bool isEditingMachine)
    {
        _instance._isEditingMachine = isEditingMachine;
    }

    public static Vector3 GetCurrentSize()
    {
        if (_instance._isEditingMachine)
        {
            return _instance._machineEditorSize;
        }
        else
        {
            return _instance._factorySize;
        }
    }
}
