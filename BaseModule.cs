using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CircuitBoard : MonoBehaviour
{
    [SerializeField] List<BaseModule> modules;

    private void Awake()
    {
        modules = new();

        modules.Add(new InputModule(Vector3Int.zero)); 
        modules.Add(new Convoyer(new(1,0,0)));
        modules.Add(new Convoyer(new(2,0,0)));
        modules.Add(new Convoyer(new(3,0,0)));
    }

    public bool TryAddModule(BaseModule newModule)
    {
        if (!modules.Any(module => module.BoardCells.Any(cell => newModule.BoardCells.Contains(cell))))
        {
            modules.Add(newModule);
            return true;
        }

        return false;
    }

    public void RemoveModule(BaseModule module)
    {
        modules.Remove(module);
    }
}

public enum ModuleRotation
{
    Rotate_0,
    Rotate_90,
    Ratate_180,
    Ratate_270
}

[System.Serializable]
public abstract class BaseModule
{
    protected BaseModule(Vector3Int pivot, ModuleRotation moduleRotation)
    {
        Pivot = pivot;
        ModuleRotation = moduleRotation;
    }

    public Vector3Int Pivot { get; }
    public ModuleRotation ModuleRotation { get; }

    public abstract Vector3Int[] LocalCells { get; }
    public Vector3Int[] BoardCells => LocalCells.Select(cell => cell + Pivot).ToArray(); // add rotation later
}

public sealed class InputModule : BaseModule
{
    public InputModule(Vector3Int pivot, ModuleRotation moduleRotation = ModuleRotation.Rotate_0) : base(pivot,moduleRotation){}

    public override Vector3Int[] LocalCells { get; } = { Vector3Int.zero };
}

public sealed class Convoyer : BaseModule
{
    public Convoyer(Vector3Int pivot, ModuleRotation moduleRotation = ModuleRotation.Rotate_0) : base(pivot, moduleRotation) { }

    public override Vector3Int[] LocalCells { get; } = { Vector3Int.zero };
}
