using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineConnector
{
    // TODO : This probably doesn't need to be all public
    public Machine _otherMachine;
    public MachineConnector _otherConnector;
    public Point _local = new Point(0, 0);
    public Direction _localDir = Direction.Up;

    public float _desiredItemsPerSecond;
    public float _itemsPerSecond;
    public float _satisfaction;

    public Item _item;
    
    public Vector3 GetWorldEdgeOffset()
    {
        Vector3 offset = new Vector3(_local.X + 0.5f, 0, _local.Y + 0.5f);

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
