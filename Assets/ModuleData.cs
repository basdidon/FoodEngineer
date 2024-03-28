using UnityEngine;

[CreateAssetMenu(menuName = "Module")]
public class ModuleData : ScriptableObject
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
