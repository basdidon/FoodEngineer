using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public sealed class BoardController : MonoBehaviour
{
    [Header("Debug")]
    public BoardObjectData input;
    public BoardObjectData convoyer;

    [SerializeField] Grid grid;
    [field:SerializeField] List<BoardObject> BoardObjects { get; set; }
    
    private void Awake()
    {
        grid = GetComponent<Grid>();

        BoardObjects = new();

        CreateBoardObject(input, Vector3Int.zero);
        CreateBoardObject(convoyer, new Vector3Int(1,0,0));
        CreateBoardObject(convoyer, new Vector3Int(2,0,0));
    }

    public bool CreateBoardObject(BoardObjectData data,Vector3Int position)
    {
        BoardObject newBoardObject = new(data,position);
        bool isOverlap = BoardObjects.Any(boardObject => boardObject.Cells.Any(cell => newBoardObject.Cells.Contains(cell)));

        if (isOverlap)
            return false;

        BoardObjects.Add(newBoardObject);

        return true;
    }

    [Serializable]
    private class BoardObject
    {
        [field: SerializeField] public Vector3Int Position { get; set; }
        public IEnumerable<Vector3Int> Cells => BoardObjectData.Cells.Select(cell => cell + Position);

        public BoardObjectData BoardObjectData { get; }

        public BoardObject(BoardObjectData data, Vector3Int position)
        {
            BoardObjectData = data;
            Position = position;
        }
    }
}


