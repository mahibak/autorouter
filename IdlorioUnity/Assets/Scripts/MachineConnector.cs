﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectorDir
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3,
}

public class MachineConnector
{
    // TODO : This probably doesn't need to be all public
    public Machine _thisMachine;
    public Machine _otherMachine;
    public MachineConnector _otherConnector;
    public Point _local = new Point(0, 0);
    public ConnectorDir _localDir = ConnectorDir.Up;
    public bool _requiredForMachineOperation = false;

    public float _desiredItemsPerSecond;
    public float _itemsPerSecond;
    public float _satisfaction;

    public Vector3 GetWorldEdgeOffset()
    {
        Vector3 offset = new Vector3(_local.X + 0.5f, 0, _local.Y + 0.5f);

        if (_thisMachine != null)
        {
            switch (_thisMachine._rotation)
            {
                case MachineRot.CW90:
                    offset = new Vector3(_local.Y + 0.5f, 0, _thisMachine._sizeX - _local.X - 0.5f);
                    break;
                case MachineRot.CW180:
                    offset = new Vector3(_thisMachine._sizeX - _local.X - 0.5f, 0, _thisMachine._sizeY - _local.Y - 0.5f);
                    break;
                case MachineRot.CW270:
                    offset = new Vector3(_thisMachine._sizeY - _local.Y - 0.5f, 0, _local.X + 0.5f);
                    break;
            }
        }

        switch (GetWorldDir())
        {
            case ConnectorDir.Up:
                offset[2] += 0.5f;
                break;
            case ConnectorDir.Down:
                offset[2] -= 0.5f;
                break;
            case ConnectorDir.Left:
                offset[0] -= 0.5f;
                break;
            case ConnectorDir.Right:
                offset[0] += 0.5f;
                break;
        }

        return offset;
    }

    public ConnectorDir GetWorldDir()
    {
        return (ConnectorDir)(((int)_localDir + (_thisMachine != null ? (int)_thisMachine._rotation : 0)) % 4);
    }
}
