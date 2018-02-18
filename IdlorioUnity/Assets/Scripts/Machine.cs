using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Machine
{
    public Point _position;
    public int _sizeX = 1;
    public int _sizeY = 1;

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

    public Properties _addedProperty = Properties.None;
    public Item _itemInStorage = null;
    public Recipe _recipe = null;
    public MachineConnector[] _inputsUsedForRecipe = null;
    public bool _productionEnabled = true;

    public Item ApplyTransformation(Item i)
    {
        Item ret = i.GetCopy();

        if (_addedProperty == Properties.None)
            return ret;

        if (ret._properties.ContainsKey(_addedProperty))
        {
            ret._properties[_addedProperty]++;
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
        GDK.DrawFilledAABB(_position.ToVector3() + new Vector3(0.5f * _sizeX, 0.5f, 0.5f * _sizeY), new Vector3(0.5f * _sizeX, 0.5f, 0.5f * _sizeY), Color.gray);
        
        GDK.DrawText(string.Format("{0},{1},{2}", _itemsPerSecondFromProduction, _itemsPerSecondToStorage, _recipe), _position.ToVector3(), Color.black);

        foreach (MachineConnector m in _inputSlots)
        {
            if (m != null)
            {
                GDK.DrawFilledAABB(_position.ToVector3() + m.GetWorldEdgeOffset(), new Vector3(0.25f, 0.25f, 0.25f), Color.red);
                GDK.DrawText(string.Format("{0}/{1} {2}", m._itemsPerSecond, m._desiredItemsPerSecond, m._item), _position.ToVector3() + Vector3.up * 0.5f + m.GetWorldEdgeOffset(), Color.red);

  
            }
        }

        foreach (MachineConnector m in _outputSlots)
        {
            if (m != null)
            {
                GDK.DrawFilledAABB(_position.ToVector3() + m.GetWorldEdgeOffset(), new Vector3(0.25f, 0.25f, 0.25f), Color.green);
                GDK.DrawText(string.Format("{0}", m._item), _position.ToVector3() + Vector3.up * 0.5f + m.GetWorldEdgeOffset(), Color.red);

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

    public IEnumerable<MachineConnector> GetConnectedInputsThatGiveItems()
    {
        foreach (MachineConnector m in _inputSlots)
            if (m._otherConnector != null && m._item != null)
                yield return m;
    }

    public IEnumerable<MachineConnector> GetConnectedOutputs()
    {
        foreach (MachineConnector m in _outputSlots)
            if (m._otherConnector != null)
                yield return m;
    }

    public MachineConnector[] GetInputSetupForRecipe(Recipe r)
    {
        List<MachineConnector> connectedInputs = GetConnectedInputsThatGiveItems().ToList();

        if (connectedInputs.Count > _inputSlots.Length)
            return null;

        if (r._outputs.Count > _outputSlots.Length && (_storageMode & Machine.StorageModes.In) == 0)
            return null;

        foreach (MachineConnector[] inputPermutation in connectedInputs.GetPermutations().Select(x => x.ToArray()))
        {
            bool goodRecipe = true;

            for (int i = 0; i < inputPermutation.Length; i++)
            {
                if (!ApplyTransformation(inputPermutation[i]._item).Satisfies(r._inputs[i]))
                {
                    goodRecipe = false;
                    break;
                }
            }

            if (goodRecipe)
                return inputPermutation;
        }

        return null;
    }
}
