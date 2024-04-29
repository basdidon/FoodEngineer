using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum ModuleRotation
{
    Ratate_0,
    Ratate_90,
    Ratate_180,
    Ratate_270,
}

public enum PortDirection
{
    Input,
    Output
}

public class ModulePort
{
    public PortDirection Direction { get; }
    public Vector3Int Position { get; }

    public ModulePort(PortDirection direction, Vector3Int position)
    {
        Direction = direction;
        Position = position;
    }
}

[RequireComponent(typeof(SpriteRenderer))]
public abstract class BaseModule : MonoBehaviour
{
    public abstract string ModuleDataPath { get; }

    public List<Vector3Int> LocalCells { get; private set; }

    List<Item> items = new();

    public CircuitBoard CircuitBoard { get; private set; }
    Vector3Int pivot;
    public Vector3Int Pivot {
        get => pivot; 
        private set
        {
            pivot = value;
            transform.position = CircuitBoard.Grid.GetCellCenterWorld(pivot);
        }
    }
    public ModuleRotation ModuleRotation { get; private set; }

    public virtual void Initialize(CircuitBoard circuitBoard,Vector3Int pivot, ModuleRotation moduleRotation)
    {
        CircuitBoard = circuitBoard;
        Pivot = pivot;
        ModuleRotation = moduleRotation;

        LoadData();
    }

    private void LoadData()
    {
        var moduleData = AssetDatabase.LoadAssetAtPath<ModuleData>(ModuleDataPath);
        LocalCells = new List<Vector3Int>(moduleData.LocalCells);
        GetComponent<SpriteRenderer>().sprite = moduleData.Sprite;
    }
}
