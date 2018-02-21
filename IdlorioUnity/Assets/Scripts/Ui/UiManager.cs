using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    private static UiManager _instance;
    public static UiManager GetInstance() { return _instance; }

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

    [SerializeField] GameObject _machineSelectedPanel = null;
    [SerializeField] Text _machineSelectedPanelInfo = null;

    public void EnableMachineSelectedPanel(string machineInfo)
    {
        if (_machineSelectedPanel != null)
        {
            _machineSelectedPanel.SetActive(true);
            _machineSelectedPanelInfo.text = machineInfo;
        }
    }

    public void DisableMachineSelectedPanel()
    {
        if (_machineSelectedPanel != null)
        {
            _machineSelectedPanel.SetActive(false);
            _machineSelectedPanelInfo.text = "";
        }
    }
}