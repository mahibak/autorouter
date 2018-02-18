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
    public Point _position;
    public int _sizeX = 1;
    public int _sizeY = 1;
    public MachineRot _rotation = MachineRot.CW0;

    public MachineConnector[] _inputSlots;
    public MachineConnector[] _outputSlots;

    //Specs
    public float _maximumItemsPerSecondFromStorage = 0;
    public float _maximumItemsPerSecondProduction = 0;
    public float _maximumItemsPerSecondOutput = System.Single.PositiveInfinity;
    public float _storageCapacity = 0;

    //State
    public float _itemsInStorage = 0;

    //Computation results
    public float _desiredItemsPerSecond = 0;
    public float _itemsPerSecondFromProduction = 0;
    public float _itemsPerSecondToOutputs = 0;
    public float _inputSatisfactionRatio = 0;
    public float _desiredItemsPerSecondToStorage = 0; //Can be negative if items are removed from storage
    public float _itemsPerSecondToStorage = 0;
    public float _outputSatisfaction = 0;
    

    [System.Flags]
    public enum StorageModes
    {
        In = 0x1,
        Out = 0x02,
    }
    public StorageModes _storageMode = 0;
    
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
            conn._local.X = _sizeX - 1;
            conn._local.Y = _sizeY - 1;
            _outputSlots[i] = conn;
        }
    }

    public float GetSecondsBeforeRecalculationNeeded()
    {
        if (_itemsPerSecondToStorage > 0)
            return (_storageCapacity - _itemsInStorage) / _itemsPerSecondToStorage;
        else if (_itemsPerSecondToStorage < 0)
            return _itemsInStorage / -_itemsPerSecondToStorage;
        else
            return System.Single.PositiveInfinity;
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

        GDK.DrawFilledAABB(_position.ToVector3() + new Vector3(0.5f * worldX, 0.5f, 0.5f * worldY), new Vector3(0.5f * worldX, 0.5f, 0.5f * worldY), Color.gray);
        
        GDK.DrawText(string.Format("{0},{1}", _itemsPerSecondFromProduction, _itemsPerSecondToStorage), _position.ToVector3(), Color.black);

        foreach (MachineConnector m in _inputSlots)
        {
            if (m != null)
            {
                GDK.DrawFilledAABB(_position.ToVector3() + m.GetWorldEdgeOffset(), new Vector3(0.25f, 0.25f, 0.25f), Color.red);
                if (m._otherMachine != null)
                {
                    GDK.DrawText(string.Format("{0}/{1}", m._itemsPerSecond, m._desiredItemsPerSecond), _position.ToVector3() + Vector3.up * 0.5f + m.GetWorldEdgeOffset(), Color.red);
                }
            }
        }

        foreach (MachineConnector m in _outputSlots)
        {
            if (m != null)
            {
                GDK.DrawFilledAABB(_position.ToVector3() + m.GetWorldEdgeOffset(), new Vector3(0.25f, 0.25f, 0.25f), Color.green);

                if (m._otherMachine != null)
                {
                    GDK.DrawLine(_position.ToVector3() + Vector3.up * 0.5f + m.GetWorldEdgeOffset(), m._otherMachine._position.ToVector3() + Vector3.up * 0.5f + m._otherConnector.GetWorldEdgeOffset(), Color.black);
                }
            }
        }
    }
}
