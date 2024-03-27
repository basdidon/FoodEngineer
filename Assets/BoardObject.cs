using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[RequireComponent(typeof(SpriteRenderer))]
public class BoardObject : MonoBehaviour
{
    Vector3Int gridPosition;
    public Vector3Int GridPosition
    {
        get => gridPosition;
        set
        {
            gridPosition = value;
            OnSetGridPosition();
        }
    }
    public IEnumerable<Vector3Int> Cells => BoardObjectData.Cells.Select(cell => cell + GridPosition);

    public BoardController BoardController { get; private set; }
    public SpriteRenderer SpriteRenderer { get; private set; }
    public BoardObjectData BoardObjectData { get; private set; }

    public void Initialize(BoardController boardController,BoardObjectData data,Vector3Int gridPosition)
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        BoardController = boardController;
        BoardObjectData = data;
        GridPosition = gridPosition;

        SpriteRenderer.sprite = BoardObjectData.Sprite;
    }

    public static GameObject CreateBoardObject(BoardController boardController, BoardObjectData data, Vector3Int gridPosition)
    {
        GameObject gameObject = new();
        gameObject.transform.parent = boardController.transform;

        BoardObject boardObject = gameObject.AddComponent<BoardObject>();

        boardObject.Initialize(boardController,data,gridPosition);

        return gameObject;
    }

    void OnSetGridPosition()
    {
        transform.position = BoardController.Grid.GetCellCenterWorld(GridPosition);
    }
}