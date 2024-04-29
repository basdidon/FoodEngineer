using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Circuit/Module")]
public class ModuleData : ScriptableObject
{
    [field: SerializeField]
    public Sprite Sprite { get; private set; }

    [SerializeField] List<Vector3Int> localCells = new() { Vector3Int.zero};
    public IReadOnlyList<Vector3Int> LocalCells => localCells;
}
