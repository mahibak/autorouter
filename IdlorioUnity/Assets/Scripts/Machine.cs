using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MachineRot
{
    CW0 = 0,
    CW90 = 1,
    CW180 = 2,
    CW270 = 3,
}

public class Machine
{
    public Vector3 _position; // Position of bottom left tile. TODO : Replace with int Point instead of float Vector.
    public int _sizeX = 1;
    public int _sizeY = 1;
    public MachineRot _rotation = MachineRot.CW0;

    public MachineConnector[] _inputSlots;
    public MachineConnector[] _outputSlots;

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
            conn._localDir = ConnectorDir.Left;
            _inputSlots[i] = conn;
        }

        for (int i = 0; i < _outputSlots.Length; ++i)
        {
            MachineConnector conn = new MachineConnector();
            conn._localDir = ConnectorDir.Right;
            conn._localX = _sizeX - 1;
            conn._localY = _sizeY - 1;
            _outputSlots[i] = conn;
        }
    }

    public void DrawDebug()
    {
        int worldX;
        int worldY;

        if (_rotation == MachineRot.CW0 || _rotation == MachineRot.CW180)
        {
            worldX = _sizeX;
            worldY = _sizeY;
        }
        else
        {
            worldX = _sizeY;
            worldY = _sizeX;
        }

        GDK.DrawFilledAABB(_position + new Vector3(0.5f * worldX, 0.5f, 0.5f * worldY), new Vector3(0.5f * worldX, 0.5f, 0.5f * worldY), Color.gray);

        foreach (MachineConnector m in _inputSlots)
        {
            if (m != null)
            {
                GDK.DrawFilledAABB(_position + m.GetWorldEdgeOffset(), new Vector3(0.25f, 0.25f, 0.25f), Color.red);
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
