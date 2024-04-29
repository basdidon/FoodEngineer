using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class CircuitBoard : MonoBehaviour
{
    public static CircuitBoard Instance { get; private set; }
    public Grid Grid { get; private set; }

    List<BaseModule> modules;

    public GameObject itemPrefab;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        Grid = GetComponent<Grid>();

        modules = new();

        var module = CreateModule<InputModule>(Vector3Int.zero);
        module.ItemPrefab = itemPrefab;
    }

    public T CreateModule<T>(Vector3Int pivot,ModuleRotation moduleRotation = ModuleRotation.Ratate_0) where T:BaseModule,new()
    {
        GameObject GO = new();
        GO.transform.parent = transform;
        var module = GO.AddComponent<T>();
        module.Initialize(this,pivot,moduleRotation);
        return module;
    }
}
