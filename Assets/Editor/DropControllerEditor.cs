using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DropController))]
[CanEditMultipleObjects]
public class DropControllerEditor : Editor
{
    int index = 0;
    ItemDBObject[] itemDBObjects;

    List<Item> items = new List<Item>();

    string[] itemNames;
    string[] itemIDs;

    void OnEnable()
    {
        itemDBObjects = Resources.LoadAll<ItemDBObject>("Database");
        itemNames = new string[0];
        itemIDs = new string[0];
        foreach (var db in itemDBObjects)
        {
            Item[] items = db.GetItems();
            string[] names = new string[items.Length];
            string[] ids = new string[items.Length];
            for (int i=0;i<items.Length;i++)
            {
                names[i] = items[i].DisplayName;
                ids[i] = items[i].ID;
            }
            
        }
    }

    //public override void OnInspectorGUI()
    //{
    //    serializedObject.Update();

    //    index = EditorGUILayout.Popup(index, itemNames);

    //    //EditorGUILayout.PropertyField(lookAtPoint);
    //    serializedObject.ApplyModifiedProperties();
    //}
}