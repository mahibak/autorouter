using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    private static InputManager _instance;
    public static void CreateInstance()
    {
        if (_instance == null)
        {
            _instance = new InputManager();
        }
    }
    public static InputManager GetInstance()
    {
        return _instance;
    }

    private InputManager() { }

    public static Point GetPointerTile()
    {
        Vector3 groundPos = ScreenPosToGround(Input.mousePosition);
        return new Point((int)groundPos.x, (int)groundPos.z);
    }

    public static Vector3 GetCursorGroundPos()
    {
        return ScreenPosToGround(Input.mousePosition);
    }

    public static Vector3 ScreenPosToGround(Vector3 screenPos)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(screenPos);
        
        if (Physics.Raycast(ray, out hit, 300, 1 << 8 /*Ground*/))
        {
            return hit.point;
        }

        return Vector3.zero;
    }

    float _timeMouseDown = 0f;
    Vector3 _mouseDownScreenPos = new Vector3();
    Vector3 _lastDragEventWorldPos = new Vector3();
    bool _isMouseDownValid = false;
    bool _isDragging = false;
    bool _isHolding = false;

    const float hold_duration = 0.25f;
    const float drag_distance = 0.025f; // In ratio of screen width

    public void Update()
    {
        bool isPointerOverGameObject = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        if (Input.GetMouseButton(0))
        {
            if (_timeMouseDown == 0f && !isPointerOverGameObject)
            {
                _mouseDownScreenPos = Input.mousePosition;
                _isDragging = false;
                _isHolding = false;
                _isMouseDownValid = true;
                OnCursorPress();
            }

            _timeMouseDown += Time.deltaTime;

            if (_isDragging)
            {
                Vector3 groundPos = GetCursorGroundPos();
                OnDrag(groundPos - _lastDragEventWorldPos);
                _lastDragEventWorldPos = groundPos;
            }
            else if (_isMouseDownValid)
            {
                Vector2 mouseTravel = Input.mousePosition - _mouseDownScreenPos;

                if (mouseTravel.GetSquareLength() > Math.Square(drag_distance * Screen.width))
                {
                    _isDragging = true;
                    Vector3 groundPos = GetCursorGroundPos();
                    OnDrag(groundPos - ScreenPosToGround(_mouseDownScreenPos));
                    _lastDragEventWorldPos = groundPos;
                }
            }

            if (!_isHolding && !_isDragging && _timeMouseDown > hold_duration && !isPointerOverGameObject)
            {
                _isHolding = true;
                OnCursorHold();
            }
        }
        else
        {
            if (_timeMouseDown > 0)
            {
                if (_timeMouseDown < hold_duration && !_isDragging && !isPointerOverGameObject)
                {
                    OnCursorClick();
                }
                
                OnCursorRelease();
                _isMouseDownValid = false;
                _isDragging = false;
                _isHolding = false;
            }

            _timeMouseDown = 0f;
        }
    }

    public void OnCursorClick()
    {
        InputStateManager.OnCursorClick(GetPointerTile());
    }

    public void OnCursorHold()
    {
        InputStateManager.OnCursorHold(GetPointerTile());
    }

    public void OnCursorPress()
    {
        InputStateManager.OnCursorPress(GetPointerTile());
    }

    public void OnCursorRelease()
    {
        InputStateManager.OnCursorRelease(GetPointerTile());
    }

    public void OnDrag(Vector3 dragAmountWorld)
    {
        InputStateManager.OnDrag(GetPointerTile(), dragAmountWorld);
    }
}
