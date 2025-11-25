// UDictionaryPropertyDrawer.cs
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(UDictionary), true)] // fallback; actual generic is UDictionary<,>
public class UDictionaryPropertyDrawer : PropertyDrawer
{
    const string KEYS_NAME = "keys";
    const string VALUES_NAME = "values";

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Root container
        var root = new VisualElement();
        root.style.paddingTop = 4;
        root.style.paddingBottom = 4;
        root.style.paddingLeft = 4;
        root.style.paddingRight = 4;

        // Find child arrays
        var keysProp = property.FindPropertyRelative(KEYS_NAME);
        var valuesProp = property.FindPropertyRelative(VALUES_NAME);

        if (keysProp == null || valuesProp == null)
        {
            root.Add(new Label("UDictionary drawer: missing 'keys' or 'values' lists"));
            return root;
        }

        // Foldout for the dictionary
        var foldout = new Foldout { text = ObjectNames.NicifyVariableName(property.name) };
        foldout.value = property.isExpanded;
        foldout.RegisterValueChangedCallback(evt => property.isExpanded = evt.newValue);
        root.Add(foldout);

        // Controls row (Add / Auto-Fix alignment)
        var controlsRow = new VisualElement { style = { flexDirection = FlexDirection.Row, alignItems = Align.Center } };
        controlsRow.style.marginBottom = 4;
        foldout.Add(controlsRow);

        // Container for list items
        var itemsContainer = new VisualElement();
        itemsContainer.style.flexDirection = FlexDirection.Column;
        foldout.Add(itemsContainer);

        var addButton = new Button(() =>
        {
            property.serializedObject.Update();
            keysProp.arraySize++;
            valuesProp.arraySize++;

            // Initialize new entries (try to set default for primitive types)
            var newKey = keysProp.GetArrayElementAtIndex(keysProp.arraySize - 1);
            var newValue = valuesProp.GetArrayElementAtIndex(valuesProp.arraySize - 1);
            InitializeProperty(newKey);
            InitializeProperty(newValue);

            property.serializedObject.ApplyModifiedProperties();
            RebuildList(itemsContainer, keysProp, valuesProp, property);
        })
        { text = "Add" };
        controlsRow.Add(addButton);

        var fixButton = new Button(() =>
        {
            property.serializedObject.Update();
            AlignArrays(keysProp, valuesProp);
            property.serializedObject.ApplyModifiedProperties();
            RebuildList(itemsContainer, keysProp, valuesProp, property);
        })
        { text = "Fix Alignment" };
        fixButton.style.marginLeft = 6;
        controlsRow.Add(fixButton);

        // Help / Duplicate warning area
        var helpBox = new Label();
        helpBox.style.unityTextAlign = TextAnchor.MiddleLeft;
        helpBox.style.marginTop = 4;
        helpBox.style.marginBottom = 6;
        foldout.Add(helpBox);

        // Build UI initially
        RebuildList(itemsContainer, keysProp, valuesProp, property);

        // Validation update callback - when anything changes, refresh
        property.serializedObject.Update(); // ensure sync
        property.serializedObject.ApplyModifiedProperties();

        // Register a callback to repaint when the serialized object changes externally
        property.serializedObject.ApplyModifiedProperties();
        property.serializedObject.Update();

        // Local helper to validate duplicates and set help text
        void UpdateValidation()
        {
            property.serializedObject.Update();
            var dupMessages = GetDuplicateKeyMessages(keysProp);
            if (dupMessages.Count > 0)
            {
                helpBox.text = "Duplicate keys detected:\n" + string.Join("\n", dupMessages);
                helpBox.style.color = new StyleColor(Color.red);
            }
            else
            {
                helpBox.text = $"Entries: {keysProp.arraySize}";
                helpBox.style.color = new StyleColor(Color.black);
            }
        }

        // Initial validation
        UpdateValidation();

        return root;
    }

    // Ensures keys/values arrays are same size by trimming the larger one
    static void AlignArrays(SerializedProperty keysProp, SerializedProperty valuesProp)
    {
        if (keysProp.arraySize > valuesProp.arraySize)
        {
            var diff = keysProp.arraySize - valuesProp.arraySize;
            for (int i = 0; i < diff; i++)
                keysProp.DeleteArrayElementAtIndex(keysProp.arraySize - 1);
        }
        else if (valuesProp.arraySize > keysProp.arraySize)
        {
            var diff = valuesProp.arraySize - keysProp.arraySize;
            for (int i = 0; i < diff; i++)
                valuesProp.DeleteArrayElementAtIndex(valuesProp.arraySize - 1);
        }
    }

    // Create all the rows anew
    static void RebuildList(VisualElement container, SerializedProperty keysProp, SerializedProperty valuesProp, SerializedProperty parentProperty)
    {
        container.Clear();

        parentProperty.serializedObject.Update();

        AlignArrays(keysProp, valuesProp);

        for (int i = 0; i < keysProp.arraySize; i++)
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.FlexStart;
            row.style.marginBottom = 6;

            // Left: key field (expand)
            var keyFieldContainer = new VisualElement { style = { flexGrow = 1, marginRight = 6 } };
            var keyProp = keysProp.GetArrayElementAtIndex(i);
            var keyField = new PropertyField(keyProp);
            keyField.style.flexShrink = 1;
            keyFieldContainer.Add(keyField);
            row.Add(keyFieldContainer);

            // Middle: value field (expand)
            var valueFieldContainer = new VisualElement { style = { flexGrow = 1, marginRight = 6 } };
            var valueProp = valuesProp.GetArrayElementAtIndex(i);
            var valueField = new PropertyField(valueProp);
            valueField.style.flexShrink = 1;
            valueFieldContainer.Add(valueField);
            row.Add(valueFieldContainer);

            // Right: controls column
            var controls = new VisualElement { style = { flexDirection = FlexDirection.Column, width = 90 } };

            // Up button
            var upBtn = new Button(() =>
            {
                SwapElements(parentProperty, keysProp, valuesProp, i, i - 1);
                RebuildList(container, keysProp, valuesProp, parentProperty);
            })
            { text = "?", tooltip = "Move up" };
            upBtn.SetEnabled(i > 0);
            controls.Add(upBtn);

            // Down button
            var downBtn = new Button(() =>
            {
                SwapElements(parentProperty, keysProp, valuesProp, i, i + 1);
                RebuildList(container, keysProp, valuesProp, parentProperty);
            })
            { text = "?", tooltip = "Move down" };
            downBtn.SetEnabled(i < keysProp.arraySize - 1);
            controls.Add(downBtn);

            // Remove button
            var removeBtn = new Button(() =>
            {
                parentProperty.serializedObject.Update();
                keysProp.DeleteArrayElementAtIndex(i);
                valuesProp.DeleteArrayElementAtIndex(i);
                parentProperty.serializedObject.ApplyModifiedProperties();
                RebuildList(container, keysProp, valuesProp, parentProperty);
            })
            { text = "Remove", tooltip = "Remove entry" };
            removeBtn.style.marginTop = 6;
            controls.Add(removeBtn);

            row.Add(controls);

            // Make the property fields bindable so they show foldouts for complex types
            keyField.Bind(parentProperty.serializedObject);
            valueField.Bind(parentProperty.serializedObject);

            // When either field changes, revalidate
            keyField.RegisterValueChangeCallback(evt =>
            {
                parentProperty.serializedObject.ApplyModifiedProperties();
                ValidateAndMark(parentProperty, keysProp);
                RebuildList(container, keysProp, valuesProp, parentProperty);
            });

            valueField.RegisterValueChangeCallback(evt =>
            {
                parentProperty.serializedObject.ApplyModifiedProperties();
            });

            container.Add(row);
        }

        parentProperty.serializedObject.ApplyModifiedProperties();
    }

    // Swap keys and values at two indices
    static void SwapElements(SerializedProperty parentProperty, SerializedProperty keysProp, SerializedProperty valuesProp, int a, int b)
    {
        parentProperty.serializedObject.Update();
        if (a < 0 || b < 0 || a >= keysProp.arraySize || b >= keysProp.arraySize) return;

        keysProp.MoveArrayElement(a, b);
        valuesProp.MoveArrayElement(a, b);

        parentProperty.serializedObject.ApplyModifiedProperties();
    }

    // Initialize newly added property to default value
    static void InitializeProperty(SerializedProperty prop)
    {
        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer:
                prop.intValue = 0;
                break;
            case SerializedPropertyType.Boolean:
                prop.boolValue = false;
                break;
            case SerializedPropertyType.Float:
                prop.floatValue = 0f;
                break;
            case SerializedPropertyType.String:
                prop.stringValue = string.Empty;
                break;
            case SerializedPropertyType.ObjectReference:
                prop.objectReferenceValue = null;
                break;
            case SerializedPropertyType.Enum:
                prop.enumValueIndex = 0;
                break;
            // For Generic/Complex fields, leave as-is; Unity will default them
            default:
                break;
        }
    }

    // Find duplicate keys (by serialized equality). Returns descriptive messages.
    static List<string> GetDuplicateKeyMessages(SerializedProperty keysProp)
    {
        var messages = new List<string>();
        var seen = new Dictionary<string, int>();

        for (int i = 0; i < keysProp.arraySize; i++)
        {
            var keyProp = keysProp.GetArrayElementAtIndex(i);
            string keyRepr = SerializedPropertyToString(keyProp);

            if (seen.TryGetValue(keyRepr, out int firstIdx))
            {
                messages.Add($"Index {firstIdx} and {i} => {keyRepr}");
            }
            else
            {
                seen[keyRepr] = i;
            }
        }

        return messages;
    }

    // Try to produce a human-readable representation of a SerializedProperty value for duplicates check.
    static string SerializedPropertyToString(SerializedProperty prop)
    {
        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer: return prop.intValue.ToString();
            case SerializedPropertyType.Boolean: return prop.boolValue.ToString();
            case SerializedPropertyType.Float: return prop.floatValue.ToString();
            case SerializedPropertyType.String: return prop.stringValue;
            case SerializedPropertyType.Enum: return prop.enumNames[prop.enumValueIndex];
            case SerializedPropertyType.ObjectReference: return prop.objectReferenceValue ? prop.objectReferenceValue.GetInstanceID().ToString() : "null";
            default:
                // For complex types, use propertyPath + serialized object to identify unique entry
                return $"{prop.propertyPath}:{prop.serializedObject.targetObject.GetInstanceID()}:{prop.serializedObject.targetObject.name}";
        }
    }

    // When a key changes, validate duplicates and mark the asset dirty if needed
    static void ValidateAndMark(SerializedProperty parentProperty, SerializedProperty keysProp)
    {
        parentProperty.serializedObject.Update();
        var dups = GetDuplicateKeyMessages(keysProp);
        if (dups.Count > 0)
        {
            // Optionally show console warning
            // Debug.LogWarning($"Duplicate keys in {parentProperty.displayName}: {string.Join(", ", dups)}");
        }

        parentProperty.serializedObject.ApplyModifiedProperties();
    }
}
#endif
