using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class MachineConnector
{
    // TODO : This probably doesn't need to be all public
    public Machine _otherMachine;
    public MachineConnector _otherConnector;
    public int _localX = 0;
    public int _localY = 0;
    public Direction _localDir = Direction.Up;
    public bool _requiredForMachineOperation = false;

    public Vector3 GetWorldEdgeOffset()
    {
        Vector3 offset = new Vector3(_localX + 0.5f, 0, _localY + 0.5f);

        switch (_localDir)
        {
            case Direction.Up:
                offset[2] += 0.5f;
                break;
            case Direction.Down:
                offset[2] -= 0.5f;
                break;
            case Direction.Left:
                offset[0] -= 0.5f;
                break;
            case Direction.Right:
                offset[0] += 0.5f;
                break;
        }

        return offset;
    }
}
