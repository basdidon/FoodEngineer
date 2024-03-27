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

[CreateAssetMenu(fileName = "Circuit",menuName = "Circuit")]
public class CircuitData : ScriptableObject
{
    [SerializeField] List<CircuitModule> circuitModules;
    [SerializeField] List<ModuleData> modules;

    public bool IsOverlap(Vector3Int[] cells) => throw new NotImplementedException(); 
    // circuitModules.Any(circuitModule => GetDataByModuleType(circuitModule.ModuleType).GetRotetedCells(circuitModule.Rotate));
    //modules.Any(module => module.LocalCells.Any(moduleCell => cells.Contains(moduleCell)));

    public Dictionary<ModuleTypes, ModuleData> ModuleDataDict = new();
    public ModuleData GetDataByModuleType(ModuleTypes moduleType)
    {
        return ModuleDataDict[moduleType];
    }
    

    public Type GetModuleType(ModuleTypes moduleType) => moduleType switch
    {
        ModuleTypes.Input => typeof(InputModule),
        _ => throw new InvalidOperationException()
    };
    
    public void InstantiateModule(ModuleTypes moduleType, Vector3Int position)
    {
        Type type = GetModuleType(moduleType);
        var instance = CreateInstance(type);
        if(instance is ModuleData moduleData)
        {
            Debug.Log("Created");
            modules.Add(moduleData);
            AssetDatabase.AddObjectToAsset(moduleData, AssetDatabase.GetAssetPath(this));

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
            AssetDatabase.Refresh();
        }
    }

    public void InstantiateModule(Type type, Vector3Int position)
    {
        if(type.IsSubclassOf(typeof(ModuleData)))
        {
            var instance = CreateInstance(type.Name);
            if(instance != null)
            {
                Debug.Log("Created");
            }
        }
    }

    public ModuleData CreateModule<T>() where T : ModuleData,new()
    {
        return CreateInstance<T>();
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
                (target as CircuitData).InstantiateModule(moduleType, createPositionField.value);
            }
        };
        moduleCreateElement.Add(createModuleButton);

        return container;
    }
}

public abstract class ModuleData : ScriptableObject
{
    [field: SerializeField]
    public Vector3Int[] LocalCells { get; private set; } = new Vector3Int[] { Vector3Int.zero };

    [field: SerializeField]
    public Sprite Sprite { get; private set; }

    public Vector3Int[] GetRotetedCells(CircuitModule.ModuleRotate rotate)
    {
        // do rotate here
        
        return LocalCells;
    }
}
