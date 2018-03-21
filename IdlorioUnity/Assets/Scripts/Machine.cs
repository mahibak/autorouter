using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Machine
{
    public Point _position;
    public Point _size;

    public Rotation _rotation = Rotation.CW0;

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
            conn._local.X = _size.X - 1;
            conn._local.Y = _size.Y - 1;
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

    public IEnumerable<Point> GetOccupiedPoints()
    {
        switch (_rotation)
        {
            case Rotation.CW90:
                return _position.GetPointsInRectangle(_size.Y, -_size.X);
            case Rotation.CW180:
                return _position.GetPointsInRectangle(-_size.X, -_size.Y);
            case Rotation.CW270:
                return _position.GetPointsInRectangle(-_size.Y, _size.X);
            case Rotation.CW0:
            default:
                return _position.GetPointsInRectangle(_size.X, _size.Y);
        }
    }

    public Point GetOppositeCornerFromPosition()
    {
        return new Point(_size.X - 1, _size.Y - 1).Rotate(_rotation) + _position;
    }

    public Point GetCenterTile()
    {
        return new Point(_size.X * 0.5f, _size.Y * 0.5f).Rotate(_rotation) + _position;
    }

    public Point GetWorldSize()
    {
        return _size.RotateAbsolute(_rotation);
    }

    public bool OccupiesPoint(Point point)
    {
        return GetOccupiedPoints().Contains(point);
    }

    public void DrawDebug()
    {
        int worldX;
        int worldY;

        if (_rotation == Rotation.CW0 || _rotation == Rotation.CW180)
        {
            worldX = _size.X;
            worldY = _size.Y;
        }
        else
        {
            worldX = _size.Y;
            worldY = _size.X;
        }

        if(OccupiesPoint(InputManager.GetPointerTile()))
        {
            GDK.DrawText(GetMachineInfoString(), _position.ToVector3(), Color.black);
        }

        GDK.DrawFilledAABB((_position + GetOppositeCornerFromPosition()).ToVector3() / 2.0f + new Vector3(0.5f, 0.5f, 0.5f), GetWorldSize().ToVector3(1.0f) / 2.0f, Color.gray);

        if (_isSelected)
        {
            Vector3 halfExtents = GetWorldSize().ToVector3(1.0f) / 2.0f + new Vector3(0.1f, 0.1f, 0.1f);
            halfExtents[1] = 0.1f;
            GDK.DrawFilledAABB((_position + GetOppositeCornerFromPosition()).ToVector3() / 2.0f + new Vector3(0.5f, 0f, 0.5f), halfExtents, GDK.FadeColor(Color.cyan, 0.25f));
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
}
