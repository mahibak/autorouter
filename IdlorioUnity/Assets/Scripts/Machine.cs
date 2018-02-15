using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine
{
    public Vector3 _position; // Position of bottom left tile. TODO : Replace with int Point instead of float Vector.
    public int _sizeX = 1;
    public int _sizeY = 1;

    public MachineConnector[] _inputSlots;
    public MachineConnector[] _outputSlots;

    public float _maximumItemsPerSecond = 0;
    public float _desiredItemsPerSecond = 0;
    public float _itemsPerSecond = 0;

    public Machine()
    {
        InitializeTestSlots();
    }

    public void InitializeTestSlots()
    {
        _inputSlots = new MachineConnector[1];
        _outputSlots = new MachineConnector[1];

        for (int i = 0; i < _inputSlots.Length; ++i)
        {
            MachineConnector conn = new MachineConnector();
            conn._localDir = Direction.Left;
            _inputSlots[i] = conn;
        }

        for (int i = 0; i < _outputSlots.Length; ++i)
        {
            MachineConnector conn = new MachineConnector();
            conn._localDir = Direction.Right;
            conn._localX = _sizeX - 1;
            conn._localY = _sizeY - 1;
            _outputSlots[i] = conn;
        }
    }

    public void DrawDebug()
    {
        GDK.DrawFilledAABB(_position + new Vector3(0.5f * _sizeX, 0.5f, 0.5f * _sizeY), new Vector3(0.5f * _sizeX, 0.5f, 0.5f * _sizeY), Color.gray);

        foreach (MachineConnector m in _inputSlots)
        {
            if (m != null)
            {
                GDK.DrawFilledAABB(_position + m.GetWorldEdgeOffset(), new Vector3(0.25f, 0.25f, 0.25f), Color.red);
                if (m._otherMachine != null)
                {
                    //GDK.DrawText(string.Format("Wants {0}", _desiredItemsPerSecond), _position + Vector3.up * 0.5f + m.GetWorldEdgeOffset(), Color.red);
                    //GDK.DrawText(string.Format("Outs {0}", _itemsPerSecond), m._otherMachine._position + Vector3.up * 0.5f + m._otherConnector.GetWorldEdgeOffset(), Color.green);
                }
            }
        }

        foreach (MachineConnector m in _outputSlots)
        {
            if (m != null)
            {
                GDK.DrawFilledAABB(_position + m.GetWorldEdgeOffset(), new Vector3(0.25f, 0.25f, 0.25f), Color.green);

                if (m._otherMachine != null)
                {
                    GDK.DrawLine(_position + Vector3.up * 0.5f + m.GetWorldEdgeOffset(), m._otherMachine._position + Vector3.up * 0.5f + m._otherConnector.GetWorldEdgeOffset(), Color.black);
                }
            }
        }
    }
}
