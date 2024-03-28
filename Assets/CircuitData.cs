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
    public Vector3Int Pivot { get; }
    [SerializeField] ModuleTypes m_moduleType;
    public ModuleTypes ModuleType { get; }
    [SerializeField] ModuleRotate m_rotate;
    public ModuleRotate Rotate { get; }

    public Vector3Int[] GetCells()
    {
        throw new NotImplementedException();
    }
}



[Serializable]
public class ModuleDictionary : SerializeDictionary<ModuleTypes, ModuleData> { }

[CreateAssetMenu(fileName = "Circuit",menuName = "Circuit")]
public class CircuitData : ScriptableObject
{
    [SerializeField] List<CircuitModule> circuitModules;
    [SerializeField] ModuleDictionary modules = new();

    public bool IsOverlap(Vector3Int[] cells) => throw new NotImplementedException();
    // circuitModules.Any(circuitModule => GetDataByModuleType(circuitModule.ModuleType).GetRotetedCells(circuitModule.Rotate));
    //modules.Any(module => module.LocalCells.Any(moduleCell => cells.Contains(moduleCell)));
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
            }
        };
        moduleCreateElement.Add(createModuleButton);

        return container;
    }
}