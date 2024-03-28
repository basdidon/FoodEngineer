using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections;

[Serializable]
public abstract class SerializeDictionary<TKey,TValue> : IDictionary<TKey,TValue>
{
    [Serializable]
    public record SerializeDictionaryItem
    {
        [field: SerializeField]
        public TKey Key { get; set; }
        [field: SerializeField]
        public TValue Value { get; set; }
    }

    [SerializeField]
    List<SerializeDictionaryItem> items;

    [SerializeField]
    SerializeDictionaryItem itemToAdd;

    [SerializeField]
    TKey keyToAdd;
    [SerializeField]
    TValue valueToAdd;

    public ICollection<TKey> Keys => items.Select(item => item.Key).ToArray();
    public ICollection<TValue> Values => items.Select(item => item.Value).ToArray();

    public int Count => items.Count;
    public bool IsReadOnly => true;

    public TValue this[TKey key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public SerializeDictionary(){}

    public void Add(TKey key, TValue value)
    {
        if (ContainsKey(key))
            throw new InvalidOperationException();

        items.Add(new SerializeDictionaryItem { Key = key, Value = value });
    }

    public bool ContainsKey(TKey key) => Keys.Contains(key);

    public bool Remove(TKey key)
    {
        var itemToRemove = items.FirstOrDefault(item => Equals(item.Key, key));
        
        if(itemToRemove != null)
        {
            items.Remove(itemToRemove);
            return true;
        }

        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (!ContainsKey(key))
        {
            value = default;
            return false;
        }

        value = this[key];
        return true;
    }

    public void Clear() => items.Clear();

    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key,item.Value);
    public bool Contains(KeyValuePair<TKey, TValue> item) => items.Any(i => Equals(i.Key, item.Key) &&  Equals(i.Value ,item.Value));
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }
    public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => items.Select(item => new KeyValuePair<TKey, TValue>(item.Key,item.Value)).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator()=> GetEnumerator();
}

[CustomPropertyDrawer(typeof(SerializeDictionary<,>),true)]
public class SerializeDictionaryPropertyDrawer : PropertyDrawer
{
    List<int> items;

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Resources/Editor/Styles/SerializeDictionaryStyle.uss");
        VisualElement container = new();

        container.styleSheets.Add(styleSheet);

        var arrayProperty = property.FindPropertyRelative("items");


        items = new();
        for (int i = 0;i< arrayProperty.arraySize; i++)
        {
            items.Add(i);
            Debug.Log(i);
        }

        Column keyColumn = new() { 
            title = "Key",
            name = "Key",
            stretchable = true, 
            makeCell = OnMakeEmptyPropertyField,
            bindCell = (e,i)=> e.Q<PropertyField>().BindProperty(arrayProperty.GetArrayElementAtIndex(i).FindPropertyRelative("<Key>k__BackingField"))
        };

        Column valueColumn = new()
        {
            title = "Value",
            name = "Value",
            stretchable = true,
            makeCell = OnMakeEmptyPropertyField,
            bindCell = (e, i) => e.Q<PropertyField>().BindProperty(arrayProperty.GetArrayElementAtIndex(i).FindPropertyRelative("<Value>k__BackingField"))
        };

        Column deleteBtnColumn = new()
        {
            makeCell = () => new Button() { text = "-" },
            bindCell = OnBindDeleteButton
        };

        VisualElement OnMakeEmptyPropertyField()
        {
            PropertyField propertyField = new() { label = string.Empty };
            propertyField.style.marginRight = new(4);
            return propertyField;
        }

        void OnBindDeleteButton(VisualElement e, int i)
        {
            e.Q<Button>().clicked += () =>
            {
                Debug.Log(i);
                arrayProperty.DeleteArrayElementAtIndex(i);
                arrayProperty.serializedObject.ApplyModifiedProperties();
            };
        }

        Columns columns = new();
        columns.Add(keyColumn);
        columns.Add(valueColumn);
        columns.Add(deleteBtnColumn);

        MultiColumnListView listView = new(columns);
        listView.headerTitle = "Dictionary";
        listView.showFoldoutHeader = true;
        listView.showBorder = true;
        listView.itemsSource = items;
        listView.TrackPropertyValue(arrayProperty,callback=> {
            Debug.Log("Changed");
            items.Clear();
            for(int i = 0;i< callback.arraySize; i++)
            {
                items.Add(i);
            }
            listView.Rebuild();
        });
        listView.BindProperty(property.FindPropertyRelative("items"));
        listView.Rebuild();

        container.Add(listView);

        VisualElement addItemContainer = new();
        addItemContainer.name = "add-item-container";
        float borderWidth = .5f;
        addItemContainer.style.borderBottomWidth = borderWidth;
        addItemContainer.style.borderTopWidth = borderWidth;
        addItemContainer.style.borderLeftWidth = borderWidth;
        addItemContainer.style.borderRightWidth = borderWidth;
        Color borderColor = Color.cyan;
        addItemContainer.style.borderBottomColor = borderColor;
        addItemContainer.style.borderTopColor = borderColor;
        addItemContainer.style.borderRightColor = borderColor;
        addItemContainer.style.borderLeftColor = borderColor;
        container.Add(addItemContainer);

        VisualElement addItemFieldContainer = new();
        addItemFieldContainer.style.flexDirection = FlexDirection.Row;
        addItemContainer.Add(addItemFieldContainer);

        SerializedProperty itemToAddSP = property.FindPropertyRelative("itemToAdd");
        PropertyField keyPropertyField = new(itemToAddSP.FindPropertyRelative("<Key>k__BackingField"),string.Empty);
        keyPropertyField.style.flexGrow = 1;
        keyPropertyField.style.minWidth = new(120);
        addItemFieldContainer.Add(keyPropertyField);

        PropertyField valuePropertyField = new(itemToAddSP.FindPropertyRelative("<Value>k__BackingField"),string.Empty);
        valuePropertyField.style.flexGrow = 1;
        valuePropertyField.style.minWidth = new(180);
        addItemFieldContainer.Add(valuePropertyField);

        Button addItemButton = new() { text = "Add"};
        addItemContainer.Add(addItemButton);

        return container;
    }


}
