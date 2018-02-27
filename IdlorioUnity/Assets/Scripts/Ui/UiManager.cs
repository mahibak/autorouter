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

        _machineSelectedPanel.SetActive(false);
        _selectionOptionsPanel.SetActive(false);
        _moveOptionsPanel.SetActive(false);
    }

    [SerializeField] private GameObject _machineSelectedPanel = null;
    [SerializeField] private Text _machineSelectedPanelInfo = null;

    [SerializeField] private GameObject _selectionOptionsPanel = null;
    [SerializeField] private GameObject _moveOptionsPanel = null;

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

    public void EnableSelectionOptionsPanel()
    {
        if (_selectionOptionsPanel != null)
        {
            _selectionOptionsPanel.SetActive(true);
        }
    }

    public void DisableSelectionOptionsPanel()
    {
        if (_selectionOptionsPanel != null)
        {
            _selectionOptionsPanel.SetActive(false);
        }
    }

    public void EnableMoveOptionsPanel()
    {
        if (_moveOptionsPanel != null)
        {
            _moveOptionsPanel.SetActive(true);
        }
    }

    public void DisableMoveOptionsPanel()
    {
        if (_moveOptionsPanel != null)
        {
            _moveOptionsPanel.SetActive(false);
        }
    }
}