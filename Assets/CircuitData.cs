using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public enum ModuleTypes
{
    Input,
    Output,
    Convoyer,
}

[Serializable]
public class CircuitModule
{
    public enum ModuleRotate
    {
        Rotate_0,
        Rotate_90,
        Rotate_180,
        Rotate_270
    }

    [SerializeField] Vector3Int m_pivot;
    [SerializeField] ModuleTypes m_moduleType;
    [SerializeField] ModuleRotate m_rotate;

    public Vector3Int Pivot { get => m_pivot; set => m_pivot = value; }
    public ModuleTypes ModuleType { get => m_moduleType; set => m_moduleType = value; }
    public ModuleRotate Rotate { get => m_rotate; set => m_rotate = value; }

    public CircuitModule(Vector3Int pivot, ModuleTypes moduleType, ModuleRotate rotate)
    {
        Pivot = pivot;
        ModuleType = moduleType;
        Rotate = rotate;
    }
}

[Serializable]
public class ModuleDictionary : SerializeDictionary<ModuleTypes, ModuleData> { }

[CreateAssetMenu(fileName = "Circuit", menuName = "Circuit")]
public class CircuitData : ScriptableObject
{
    [field: SerializeField]
    public BoundsInt Bounds { get; private set; }
    [SerializeField] ModuleDictionary modules= new();
    [SerializeField] List<CircuitModule> circuitModules;

    public Vector3Int[] GetCellsOf(CircuitModule circuitModule)
        => GetDataByModuleType(circuitModule.ModuleType).GetRotetedCells(circuitModule.Rotate, circuitModule.Pivot);
    
    public bool IsOverlapWith(Vector3Int[] cells, CircuitModule circuitModule)
        => GetCellsOf(circuitModule).Any(moduleCell => cells.Any(cell => cell == moduleCell));

    public bool IsOverlap(CircuitModule circuitModule) => IsOverlap(GetCellsOf(circuitModule));
    public bool IsOverlap(Vector3Int[] cells)
        => circuitModules.Any(module=> IsOverlapWith(cells,module));

    public bool IsOutOfBound(CircuitModule circuitModule) => IsOutOfBound(GetCellsOf(circuitModule));
    public bool IsOutOfBound(Vector3Int[] cells)
        => cells.Any(cell => !Bounds.Contains(cell));

    public void AddCircuitModule(Vector3Int pivot,ModuleTypes moduleType,CircuitModule.ModuleRotate moduleRotate = CircuitModule.ModuleRotate.Rotate_0)
    {
        CircuitModule moduleToAdd = new(pivot, moduleType, moduleRotate);

        if (IsOverlap(moduleToAdd))
        {
            Debug.LogWarning("can't add new CircuitModule : <color=yellow>Overlap</color> with exitsing CircuitModule.");
        }
        else if (IsOutOfBound(moduleToAdd))
        {
            Debug.LogWarning("can't add new CircuitModule : <color=yellow>Out Of Bound</color>.");
        }
        else
        {
            circuitModules.Add(moduleToAdd);
        }
    }

    public ModuleData GetDataByModuleType(ModuleTypes moduleType)
    {
        return modules[moduleType];
    }
}


[CustomEditor(typeof(CircuitData))]
public class CircuitDataEditor : Editor
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement container = new();
        InspectorElement.FillDefaultInspector(container, serializedObject, this);

        VisualElement moduleCreateElement = new();
        container.Add(moduleCreateElement);

        EnumField typeSelectField = new("Type To Create",ModuleTypes.Convoyer);
        moduleCreateElement.Add(typeSelectField);

        Vector3IntField createPositionField = new("Postion");
        moduleCreateElement.Add(createPositionField);

        Button createModuleButton = new(){ text = "Create Module"};
        createModuleButton.clicked += () =>
        {
            if(typeSelectField.value is ModuleTypes moduleType)
            {
                // create 
                (target as CircuitData).AddCircuitModule(createPositionField.value, Enum.Parse<ModuleTypes>(typeSelectField.value.ToString()));
            }
        };
        moduleCreateElement.Add(createModuleButton);

        return container;
    }
}