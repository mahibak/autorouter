using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out hit, 300, 1 << 8 /*Ground*/))
        {
            return new Point((int)hit.point.x, (int)hit.point.z);
        }

        return new Point();
    }
}
