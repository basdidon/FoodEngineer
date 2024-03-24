using UnityEngine;

public abstract class BoardObjectData : ScriptableObject
{
    [SerializeField] protected Vector3Int[] cells = new Vector3Int[] { Vector3Int.zero };
    public Vector3Int[] Cells => cells;

    [field:SerializeField] public Sprite Sprite { get; private set;}
}
