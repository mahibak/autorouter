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
    [SerializeField] private Button _moveConfirmButton = null;

    public static void SetMachineSelectedPanelEnabled(bool enable)
    {
        if (_instance._machineSelectedPanel != null)
        {
            _instance._machineSelectedPanel.SetActive(enable);
        }
    }

    public static void SetMachineSelectedInfoText(string machineInfo)
    {
        if (_instance._machineSelectedPanelInfo != null)
        {
            _instance._machineSelectedPanelInfo.text = machineInfo;
        }
    }

    public static void SetSelectionOptionsPanelEnabled(bool enable)
    {
        if (_instance._selectionOptionsPanel != null)
        {
            _instance._selectionOptionsPanel.SetActive(enable);
        }
    }

    public static void SetMoveOptionsPanelEnabled(bool enable)
    {
        if (_instance._moveOptionsPanel != null)
        {
            _instance._moveOptionsPanel.SetActive(enable);
        }
    }

    public static void SetMoveConfirmButtonEnabled(bool enable)
    {
        if (_instance._moveConfirmButton != null)
        {
            _instance._moveConfirmButton.interactable = enable;
        }
    }
}