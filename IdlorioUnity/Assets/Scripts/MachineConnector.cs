using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineConnector
{
    // TODO : This probably doesn't need to be all public
    public Machine _thisMachine;
    public Conveyor _conveyor;
    public Point _local = new Point(0, 0);
    public Direction _localDir = Direction.Up;

    public bool _input;

    public bool IsConnected
    {
        get
        {
            return _conveyor != null;
        }
    }

    public Point GetWorldPosition()
    {
        if (_thisMachine == null)
            return _local;
        else
            return _local.Rotate(_thisMachine._rotation) + _thisMachine._position;
    }

    public Point GetWorldPositionOneTileOut()
    {
        if (_thisMachine == null)
            return _local + new Point(_localDir);
        else
            return (_local + new Point(_localDir)).Rotate(_thisMachine._rotation) + _thisMachine._position;
    }

    public Vector3 GetWorldEdgeOffset()
    {
        Vector3 offset = new Vector3(_local.X + 0.5f, 0, _local.Y + 0.5f);

        if (_thisMachine != null)
        {
            switch (_thisMachine._rotation)
            {
                case Rotation.CW90:
                    offset = new Vector3(_local.Y + 0.5f, 0, _thisMachine._size.X - _local.X - 0.5f);
                    break;
                case Rotation.CW180:
                    offset = new Vector3(_thisMachine._size.X - _local.X - 0.5f, 0, _thisMachine._size.Y - _local.Y - 0.5f);
                    break;
                case Rotation.CW270:
                    offset = new Vector3(_thisMachine._size.Y - _local.Y - 0.5f, 0, _local.X + 0.5f);
                    break;
            }
        }

        switch (GetWorldDirection())
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

    public Direction GetWorldDirection()
    {
        return (Direction)(((int)_localDir + (_thisMachine != null ? (int)_thisMachine._rotation : 0)) % 4);
    }

    public override string ToString()
    {
        if(IsConnected)
            return string.Format("Connected, {0:P2} satisfied, Has {1}/{2} desired of {3}", _conveyor.Satisfaction, _conveyor._itemsPerSecond, _conveyor._desiredItemsPerSecond, _conveyor._item);
        else
            return "Disconnected";
    }

    public void DebugDraw()
    {
        GDK.DrawFilledAABB(GetWorldPosition().ToVector3() + new Vector3(0.5f, 0.5f, 0.5f) + new Point(GetWorldDirection()).ToVector3() * 0.5f, new Vector3(0.25f, 0.25f, 0.25f), _input ? Color.red : Color.green);

        if(_input && IsConnected)
        {
            //GDK.DrawLine(GetWorldPosition().ToVector3() + new Vector3(0.5f, 0.5f, 0.5f) + Vector3.up * 0.5f + new Point(GetWorldDirection()).ToVector3() * 0.5f, _otherConnector.GetWorldPosition().ToVector3() + new Vector3(0.5f, 0.5f, 0.5f) + new Point(_otherConnector.GetWorldDirection()).ToVector3() * 0.5f, Color.black);
        }
    }
}
