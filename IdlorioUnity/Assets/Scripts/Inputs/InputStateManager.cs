using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonType
{
    Invalid,
    Confirm,
    Cancel,
    Move,
}

public class InputStateManager : MonoBehaviour
{
    private static InputStateManager _instance;

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

    BaseInputState _mainInputState = null;

    private void OnEnable()
    {
        _mainInputState = new InputStateMain();
        _mainInputState.OnEnter();
    }

    private void OnDisable()
    {
        _mainInputState.OnExit();
        _mainInputState = null;
    }

    private void Update()
    {
        _instance._mainInputState.Update();
    }

    public static void OnCursorClick(Point tile)
    {
        _instance._mainInputState.OnCursorClick(tile);
    }

    public static void OnCursorHold(Point tile)
    {
        _instance._mainInputState.OnCursorHold(tile);
    }

    public static void OnCursorPress(Point tile)
    {
        _instance._mainInputState.OnCursorPress(tile);
    }

    public static void OnCursorRelease(Point tile)
    {
        _instance._mainInputState.OnCursorRelease(tile);
    }

    public static void OnDrag(Point tile, Vector3 dragAmountWorld)
    {
        _instance._mainInputState.OnDrag(tile, dragAmountWorld);
    }

    public void OnButtonMove()
    {
        _mainInputState.OnButtonClick(ButtonType.Move);
    }

    public void OnButtonConfirm()
    {
        _mainInputState.OnButtonClick(ButtonType.Confirm);
    }

    public void OnButtonCancel()
    {
        _mainInputState.OnButtonClick(ButtonType.Cancel);
    }
}
