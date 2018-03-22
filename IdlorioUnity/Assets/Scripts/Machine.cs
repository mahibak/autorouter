using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Machine
{
    public GridTransform _gridTransform = new GridTransform();

    public MachineConnector[] _inputs;
    public MachineConnector[] _outputs;

    //Specs
    public float _maximumItemsPerSecond = 0;
    public float _storageCapacity = 0;

    //State
    public float _itemsInStorage = 0;
    public bool _isSelected = false;

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
    public Conveyor[] _inputConveyorsUsedForRecipe = null;
    
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

    public Point GetTile()
    {
        return _gridTransform.GetTile();
    }

    public void SetTile(Point tile)
    {
        _gridTransform.SetTile(tile);
    }

    public Point GetSize()
    {
        return _gridTransform.GetBaseSize();
    }

    public void SetSize(Point size)
    {
        _gridTransform.SetBaseSize(size);
    }

    public Rotation GetRotation()
    {
        return _gridTransform.GetRotation();
    }

    public void SetRotation(Rotation rot)
    {
        _gridTransform.SetRotation(rot);
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
        _inputs = new MachineConnector[1];
        _outputs = new MachineConnector[1];

        for (int i = 0; i < _inputs.Length; ++i)
        {
            MachineConnector conn = new MachineConnector();
            conn._localDir = Direction.Left;
            _inputs[i] = conn;
        }

        for (int i = 0; i < _outputs.Length; ++i)
        {
            MachineConnector conn = new MachineConnector();
            conn._localDir = Direction.Right;
            conn._local.X = GetSize().X - 1;
            conn._local.Y = GetSize().Y - 1;
            _outputs[i] = conn;
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

    public IEnumerable<Point> GetOccupiedTiles()
    {
        return _gridTransform.GetOccupiedTiles();
    }

    public Point GetOppositeCornerTile()
    {
        return _gridTransform.GetOppositeTile();
    }

    public Point GetCenterTile()
    {
        return _gridTransform.GetCenterTile();
    }

    public Point GetWorldSize()
    {
        return _gridTransform.GetCurrentSize();
    }

    public bool OccupiesTile(Point point)
    {
        return GetOccupiedTiles().Contains(point);
    }

    public void DrawDebug()
    {
        if(OccupiesTile(InputManager.GetPointerTile()))
        {
            GDK.DrawText(GetMachineInfoString(), _gridTransform.GetPos(), Color.black);
        }

        _gridTransform.DrawInWorld(Color.gray);

        if (_isSelected)
        {
            _gridTransform.DrawInWorld(GDK.FadeColor(Color.cyan, 0.25f), 1f, 0.1f);
        }
        
        foreach (MachineConnector m in _inputs)
        {
            if (m != null)
            {
                m.DebugDraw();
            }
        }

        foreach (MachineConnector m in _outputs)
        {
            if (m != null)
            {
                m.DebugDraw();
            }
        }
    }

    public string GetMachineInfoString()
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

        sb.AppendLine(string.Format("Produces {0} of a desired {1}, max is  {6}, {2}/{3} to outputs, {4}/{5} to storage", _itemsPerSecondFromProduction, DesiredItemsPerSecond, _itemsPerSecondToOutputs, _desiredItemsPerSecondToOutputs, _itemsPerSecondToStorage, _desiredItemsPerSecondToStorage, _maximumItemsPerSecond));

        for (int i = 0; i < _inputs.Length; i++)
        {
            sb.AppendLine(string.Format("In {0}: {1}", i, _inputs[i].ToString()));
        }

        for (int i = 0; i < _outputs.Length; i++)
        {
            sb.AppendLine(string.Format("Out {0}: {1}", i, _outputs[i].ToString()));
        }

        return sb.ToString();
    }

    public IEnumerable<Conveyor> GetConnectedInputs()
    {
        foreach (MachineConnector m in _inputs)
            if (m._conveyor != null)
                yield return m._conveyor;
    }

    public IEnumerable<Conveyor> GetConnectedOutputs()
    {
        foreach (MachineConnector m in _outputs)
            if (m._conveyor != null)
                yield return m._conveyor;
    }

    public void TryRefreshConnectors()
    {
        List<Tuple<MachineConnector, MachineConnector>> lostConnections = new List<Tuple<MachineConnector, MachineConnector>>();
        foreach (MachineConnector m in _outputs)
        {
            if (m._conveyor != null)
            {
                lostConnections.Add(new Tuple<MachineConnector, MachineConnector>(m._conveyor._output, m._conveyor._input));
                MachineManager.DisconnectMachines(m._conveyor);
            }
        }

        foreach (MachineConnector m in _inputs)
        {
            if (m._conveyor != null)
            {
                lostConnections.Add(new Tuple<MachineConnector, MachineConnector>(m._conveyor._output, m._conveyor._input));
                MachineManager.DisconnectMachines(m._conveyor);
            }
        }
        
        foreach (Tuple<MachineConnector, MachineConnector> c in lostConnections)
        {
            MachineManager.ConnectMachines(c.Item1, c.Item2);
        }

        MachineManager.UpdateRecipes();
    }
}
