using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public float _maximumItemsPerSecond = 0;
    public float _storageCapacity = 0;

    //State
    public float _itemsInStorage = 0;

    //Computation results
    public float _desiredItemsPerSecondToOutputs = 0;
    public float _itemsPerSecondToOutputs = 0;
    public float _desiredItemsPerSecondToStorage = 0; //Can be negative if items are removed from storage
    public float _itemsPerSecondToStorage = 0;
    public float _outputSatisfaction = 0;
    public float _inputSatisfaction = 0;
    public float _itemsPerSecondFromProduction = 0;

    public Properties _addedProperty = Properties.None;
    public Item _itemInStorage = null;
    public Recipe _recipe = null;
    public MachineConnector[] _inputsUsedForRecipe = null;
    
    public bool IsStorage
    {
        get
        {
            return _storageCapacity != 0;
        }
    }

    public float StorageLeft
    {
        get
        {
            return _storageCapacity - _itemsInStorage;
        }
    }

    public float DesiredItemsPerSecond
    {
        get
        {
            return _desiredItemsPerSecondToOutputs + _desiredItemsPerSecondToStorage;
        }
    }

    public float PossibleDesiredItemsPerSecond
    {
        get
        {
            return Mathf.Min(DesiredItemsPerSecond, _maximumItemsPerSecond);
        }
    }
    
    public Item ApplyTransformation(Item i)
    {
        Item ret = i.GetCopy();

        if (_addedProperty == Properties.None)
            return ret;

        if (ret._properties.ContainsKey(_addedProperty))
        {
            ret._properties[_addedProperty]++;
        }
        else
        {
            ret._properties.Add(_addedProperty, 1);
        }

        return ret;
    }

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
            conn._localDir = Direction.Left;
            _inputSlots[i] = conn;
        }

        for (int i = 0; i < _outputSlots.Length; ++i)
        {
            MachineConnector conn = new MachineConnector();
            conn._localDir = Direction.Right;
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

        if(_position.GetPointsInRectangle(_sizeX, _sizeY).Contains(InputManager.GetPointerTile()))
        {
            StringBuilder sb = new StringBuilder();

            if (_recipe != null)
                sb.AppendLine(_recipe.ToString());
            else
                sb.AppendLine("No Recipe");

            if (IsStorage)
                sb.AppendLine(string.Format("Storage {3} is {0}/{1}, {2} left", _itemsInStorage, _storageCapacity, StorageLeft, _storageMode));
            if (_addedProperty != Properties.None)
                sb.AppendLine("Adds " + _addedProperty);

            sb.AppendLine(string.Format("Produces {0}/{1}, {2}/{3} to outputs, {4}/{5} to storage", _itemsPerSecondFromProduction, DesiredItemsPerSecond, _itemsPerSecondToOutputs, _desiredItemsPerSecondToOutputs, _itemsPerSecondToStorage, _desiredItemsPerSecondToStorage));

            for (int i = 0; i < _inputSlots.Length; i++)
            {
                sb.AppendLine(string.Format("In {0}: {1}, {2:P2}, Has {3}/{4} of {5}", i, _inputSlots[i].IsConnected ? "Connected" : "Disconnected", _inputSlots[i].Satisfaction, _inputSlots[i]._itemsPerSecond, _inputSlots[i]._desiredItemsPerSecond, _inputSlots[i]._item));
            }

            for (int i = 0; i < _outputSlots.Length; i++)
            {
                sb.AppendLine(string.Format("Out {0}: {1}, {2:P2}, Has {3}/{4} of {5}", i, _outputSlots[i].IsConnected ? "Connected" : "Disconnected", _outputSlots[i].Satisfaction, _outputSlots[i]._itemsPerSecond, _outputSlots[i]._desiredItemsPerSecond, _outputSlots[i]._item));
            }

            GDK.DrawText(sb.ToString(), _position.ToVector3(), Color.black);
        }

        GDK.DrawFilledAABB(_position.ToVector3() + new Vector3(0.5f * worldX, 0.5f, 0.5f * worldY), new Vector3(0.5f * worldX, 0.5f, 0.5f * worldY), Color.gray);
        
        foreach (MachineConnector m in _inputSlots)
        {
            if (m != null)
            {
                GDK.DrawFilledAABB(_position.ToVector3() + m.GetWorldEdgeOffset(), new Vector3(0.25f, 0.25f, 0.25f), Color.red);  
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

    public IEnumerable<MachineConnector> GetConnectedInputs()
    {
        foreach (MachineConnector m in _inputSlots)
            if (m._otherConnector != null)
                yield return m;
    }
    
    public IEnumerable<MachineConnector> GetConnectedOutputs()
    {
        foreach (MachineConnector m in _outputSlots)
            if (m._otherConnector != null)
                yield return m;
    }
}
