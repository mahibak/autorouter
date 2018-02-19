using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineConnector
{
    // TODO : This probably doesn't need to be all public
    public Machine _thisMachine;
    public Machine _otherMachine;
    public MachineConnector _otherConnector;
    public Point _local = new Point(0, 0);
    public Direction _localDir = Direction.Up;

    public float _desiredItemsPerSecond;
    public float _itemsPerSecond;

    public Item _item;

    public bool IsConnected
    {
        get
        {
            return _otherConnector != null;
        }
    }

    public float Satisfaction
    {
        get
        {
            if (_desiredItemsPerSecond == 0)
                return 1;
            else if (System.Single.IsPositiveInfinity(_desiredItemsPerSecond))
                return 0;
            else
                return _itemsPerSecond / _desiredItemsPerSecond;
        }
    }
    
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

    public Direction GetWorldDir()
    {
        return (Direction)(((int)_localDir + (_thisMachine != null ? (int)_thisMachine._rotation : 0)) % 4);
    }

    public override string ToString()
    {
        return string.Format("{0}, {1:P2} satisfied, Has {2}/{3} desired of {4}", IsConnected ? "Connected" : "Disconnected", Satisfaction, _itemsPerSecond, _desiredItemsPerSecond, _item);
    }
}
