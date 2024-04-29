using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputModule : BaseModule
{
    public override string ModuleDataPath => "Assets/Resources/InputModule.asset";
    public GameObject ItemPrefab;

    private void Start()
    {
        Instantiate(ItemPrefab, transform);
    }
}
