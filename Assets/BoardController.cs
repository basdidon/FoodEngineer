using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Tilemaps;

public enum BoardObjectTypes
{
    Input,
    Output,
    Convoyer
}

public class GridData<T>
{
    public int Width { get; }
    public int Height { get; }

    T[,] data;

    public GridData(int width, int height)
    {
        Width = width;
        Height = height;
        data = new T[Width, Height];
    }

    public T this[int i,int j]
    {
        get => data[i,j];
        set => data[i, j] = value;
    }

    public void Clear()
    {
        data = new T[Width, Height];
    }
}

[Serializable]
public class BoardObjectGridData : GridData<BoardObjectData>
{
    public BoardObjectGridData(int width, int height) : base(width, height)
    {

    }
}

[RequireComponent(typeof(Grid))]
public sealed class BoardController : MonoBehaviour
{
    public GameObject boardObjectPrefab;

    public Vector2Int size;
    public Sprite Sprite;
    public BoardObjectGridData BoardObjectGridData;


    public Grid Grid { get; private set; }

    [field:SerializeField] List<BoardObject> BoardObjects { get; set; }
    [SerializeField] Dictionary<BoardObjectTypes, BoardObjectData> BoardObjectDataDictionary;

    private void Awake()
    {
        Grid = GetComponent<Grid>();

        BoardObjects = new();
        BoardObjectDataDictionary = new();

        BoardObjectGridData = new(size.x, size.y);
        
        foreach(var boardTypeName in Enum.GetNames(typeof(BoardObjectTypes)))
        {
            var path = $"Assets/Resources/{boardTypeName}.Asset";
            BoardObjectData boardObjectData = AssetDatabase.LoadAssetAtPath<BoardObjectData>(path);

            if (boardObjectData == null)
                Debug.Log($"{boardTypeName} = null");

            BoardObjectDataDictionary.Add(Enum.Parse<BoardObjectTypes>(boardTypeName), boardObjectData);
        }
        
        for(int i = 0; i < BoardObjectGridData.Width; i++)
        {
            for(int j=0;j<BoardObjectGridData.Height; j++)
            {
                if(i != j)
                {
                    BoardObjectGridData[i, j] = BoardObjectDataDictionary[BoardObjectTypes.Convoyer];
                }    

                if(BoardObjectGridData[i,j] is BoardObjectData boardObjectData)
                {
                    var gameObject = BoardObject.CreateBoardObject(this, boardObjectData, new(i, j, 0));
                    gameObject.name = $"BoardObject({i},{j})";
                    /*
                    var clone = Instantiate(boardObjectPrefab, transform);
                    clone.name = $"BoardObject({i},{j})";
                    BoardObject boardObject = clone.GetComponent<BoardObject>();
                    boardObject.Initialize(this, boardObjectData, new(i, j, 0));
                    */
                }
            }
        }
    }

    public bool CreateBoardObject(BoardObjectTypes moduleType, Vector3Int position)
    {
        throw new System.NotImplementedException();
    }

    public bool CreateBoardObject(BoardObjectData data,Vector3Int position)
    {
        bool isOverlap = BoardObjects.Any(boardObject => boardObject.Cells.Any(cell => data.Cells.Select(cell => cell + position).Contains(cell)));

        if (isOverlap)
            return false;

        var clone = Instantiate(boardObjectPrefab, transform);
        BoardObject boardObject = clone.GetComponent<BoardObject>();
        boardObject.Initialize(this,data,position);
        BoardObjects.Add(boardObject);
        return true;
    }

}

[CustomEditor(typeof(BoardController))]
public class BoardControllerEditor : Editor 
{
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement container = new(); 

        InspectorElement.FillDefaultInspector(container, serializedObject, this);

        ObjectField boardObjectDataField = new() { objectType = typeof(BoardObjectData)};
        container.Add(boardObjectDataField);

        EnumField boardObjectTypeSelector = new(BoardObjectTypes.Convoyer);
        container.Add(boardObjectTypeSelector);

        Vector3IntField positionField = new();
        container.Add(positionField);

        Button gridSelectionBtn = new() { text = "select cell" };
        container.Add(gridSelectionBtn);

        gridSelectionBtn.clicked += () =>
        {
            GameObject gameObject = (target as BoardController).gameObject;
            GridSelection.Select(gameObject,new(Vector3Int.zero,Vector3Int.one));
        };

        Button createBoardObjectBtn = new() { text = "Create boardObject" };
        container.Add(createBoardObjectBtn);

        createBoardObjectBtn.clicked += () =>
        {
            if(boardObjectDataField.value is BoardObjectData objectData)
            {
                if((target as BoardController).CreateBoardObject(objectData, positionField.value))
                {
                    Debug.Log("created boardObject");
                }
                else
                {
                    Debug.Log("look like boardObject overlap with the other.");
                }
                
            }

                Debug.Log("boardObjectData is null");
        };

        return container;
    }
}