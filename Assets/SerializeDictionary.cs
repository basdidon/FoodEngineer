using System;
using System.Reflection;
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
    protected SerializeDictionaryItem itemToAdd;

    public ICollection<TKey> Keys => items.Select(item => item.Key).ToArray();
    public ICollection<TValue> Values => items.Select(item => item.Value).ToArray();

    public int Count => items.Count;
    public bool IsReadOnly => true;

    public TValue this[TKey key] {
        get
        {
            if (TryGetValue(key,out TValue value))  
                return value;

            throw new KeyNotFoundException();
        } 
        set => throw new NotImplementedException();
    }

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
        var item = items.FirstOrDefault(i => Equals(i.Key, key));
        if (item == null)
        {
            value = default;
            return false;
        }

        value = item.Value;
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
        VisualElement container = new();

        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Resources/Editor/Styles/SerializeDictionaryStyle.uss");
        container.styleSheets.Add(styleSheet);

        var arrayProperty = property.FindPropertyRelative("items");

        Foldout foldout = new() { text = $"{property.propertyPath}"};
        container.Add(foldout);

        items = new();
        for (int i = 0;i< arrayProperty.arraySize; i++)
        {
            items.Add(i);
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

        foldout.Add(listView);

        VisualElement addItemContainer = new(){
            name = "add-item-container"
        };
        foldout.Add(addItemContainer);

        VisualElement addItemFieldsContainer = new() {
            name = "add-item-fields-container"
        };
        addItemContainer.Add(addItemFieldsContainer);

        SerializedProperty itemToAddSP = property.FindPropertyRelative("itemToAdd");

        SerializedProperty itemKeyToAddSP = itemToAddSP.FindPropertyRelative("<Key>k__BackingField");
        PropertyField keyPropertyField = new(itemKeyToAddSP, string.Empty) {
            name = "item-to-add-key-field"
        };
        addItemFieldsContainer.Add(keyPropertyField);

        SerializedProperty itemValueToAddSP = itemToAddSP.FindPropertyRelative("<Value>k__BackingField");
        PropertyField valuePropertyField = new(itemValueToAddSP, string.Empty) { 
            name = "item-to-add-value-field" 
        };
        addItemFieldsContainer.Add(valuePropertyField);

        Button addItemButton = new() { 
            name ="add-item-btn", 
            text = "Add"
        };
        addItemButton.AddToClassList("add-item-btn");
        addItemContainer.Add(addItemButton);

        addItemButton.clicked += () => OnAddButtonClicked(property);

        return container;
    }

    void OnAddButtonClicked(SerializedProperty property)
    {
        var targetObject = property.serializedObject.targetObject;
        FieldInfo dictFieldInfo = targetObject.GetType().GetField(property.propertyPath, BindingFlags.Instance | BindingFlags.NonPublic);
        if (dictFieldInfo == null)
            throw new Exception($"can't find field {targetObject.GetType()}.{property.propertyPath}");
            
        var dictObject = dictFieldInfo.GetValue(targetObject);

        var itemToAddFieldInfo = dictFieldInfo.FieldType.GetField("itemToAdd", BindingFlags.Instance | BindingFlags.NonPublic);
        var itemToAddObject = itemToAddFieldInfo.GetValue(dictObject);

        var keyInfo = itemToAddFieldInfo.FieldType.GetProperty("Key");
        var valueInfo = itemToAddFieldInfo.FieldType.GetProperty("Value");
        Type[] addMethodParamsType = new Type[] { keyInfo.PropertyType, valueInfo.PropertyType };

        // values
        var keyObject = keyInfo.GetValue(itemToAddObject);
        var valueObject = valueInfo.GetValue(itemToAddObject);

        var addMethod = dictFieldInfo.FieldType.GetMethod("Add", addMethodParamsType);
        if (addMethod != null)
        {
            addMethod.Invoke(dictObject, new[] { keyObject, valueObject });
            Debug.Log("found");
        }
    }
}
