using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemySpawn))]
[CanEditMultipleObjects]
public class EnemySpawnEditor : Editor
{
    int _index;
    List<EnemyFamilyData> _familyData = new List<EnemyFamilyData>();

    void OnEnable()
    {
        _familyData = new List<EnemyFamilyData>();
        EnemySpawn spawn = serializedObject.targetObject as EnemySpawn;

        MasterDBObject t = (MasterDBObject)AssetDatabase.LoadAssetAtPath("Assets/Master.asset", typeof(MasterDBObject));
        for(int i=0;i<t.Enemies.Count;i++)
        {
            _familyData.Add(t.Enemies[i].Family);
            if (t.Enemies[i].Family.ID == spawn.EnemyFamilyID)
                _index = i;
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.Space();

        Object[] objs = serializedObject.targetObjects;
        List<EnemySpawn> spawns = new List<EnemySpawn>();
        foreach(var s in objs)
            spawns.Add(s as EnemySpawn);
        EnemySpawn spawn = serializedObject.targetObject as EnemySpawn;

        float oldRange = spawn.Range;
        spawn.Range = EditorGUILayout.FloatField("Range", spawn.Range);
        if (oldRange != spawn.Range)
        {
            foreach (var s in spawns)
                s.Range = spawn.Range;
        }

        string[] enemyIDList = new string[_familyData.Count];
        for (int i = 0; i < _familyData.Count; i++)
            enemyIDList[i] = _familyData[i].ID;

        int oldIndex = _index;
        _index = EditorGUILayout.Popup("Enemy Family", _index, enemyIDList);
        spawn.EnemyFamilyID = enemyIDList[_index];
        if(oldIndex != _index)
        {
            foreach (var s in spawns)
                s.EnemyFamilyID = spawn.EnemyFamilyID;
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}