using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public enum BoardObjectTypes
{
    Input,
    Output,
    Convoyer
}

[Serializable]
public class BoardObjectTypeDataDictionary : UnitySerializedDictionary<BoardObjectTypes, BoardObjectData> { }

[RequireComponent(typeof(Grid))]
public sealed class BoardController : MonoBehaviour
{
    public GameObject boardObjectPrefab;

    public Grid Grid { get; private set; }

    [field:SerializeField] List<BoardObject> BoardObjects { get; set; }
    [SerializeField] Dictionary<BoardObjectTypes, BoardObjectData> BoardObjectDataDictionary;
    [SerializeField] BoardObjectTypeDataDictionary Dict;

    private void Awake()
    {
        Grid = GetComponent<Grid>();

        BoardObjects = new();
        BoardObjectDataDictionary = new();

        foreach(var boardTypeName in Enum.GetNames(typeof(BoardObjectTypes)))
        {
            var path = $"{boardTypeName}";
            BoardObjectData boardObjectData = AssetDatabase.LoadAssetAtPath<BoardObjectData>(path);
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