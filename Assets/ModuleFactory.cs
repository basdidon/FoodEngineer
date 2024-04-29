using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ModuleType
{
    InputModule,
    Convoyer
}

public class ModuleFactory : MonoBehaviour
{
    public static ModuleFactory Instance { get; private set; }

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
    }
    /*
    public GameObject CreateModule(ModuleType moduleType)
    {
        GameObject GO = new();

    }*/
}
