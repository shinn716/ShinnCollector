using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using Shinn;

[CustomEditor(typeof(SingleValueController))]
public class SingleValueControllerEditor : Editor{

    bool showItweenSettings = true;

    public override void OnInspectorGUI()
    {

        SingleValueController script = (SingleValueController)target;
        
        EditorGUILayout.Space();
        script.valuerange = EditorGUILayout.Vector2Field("Range", script.valuerange);


        EditorGUILayout.Space();
        SerializedProperty onCheck = serializedObject.FindProperty("floatevents");
        EditorGUILayout.PropertyField(onCheck);


        showItweenSettings = EditorGUILayout.Foldout(showItweenSettings, "Itween");

        if (showItweenSettings) {

            //EditorGUILayout.Space();

            EditorGUILayout.LabelField("General setting.");

            EditorGUILayout.Space();
            script.autoStart = EditorGUILayout.Toggle("AutoStart", script.autoStart);

            EditorGUILayout.Space();
            script.target = (GameObject)EditorGUILayout.ObjectField("Target", script.target, typeof(GameObject), true);

            EditorGUILayout.Space();
            script.time = EditorGUILayout.FloatField("Time", script.time);
            script.delay = EditorGUILayout.Slider("Delay", script.delay, 0, 60);

            EditorGUILayout.Space();
            script.ease = (iTween.EaseType)EditorGUILayout.EnumPopup("EaseType", script.ease);
            script.loop = (iTween.LoopType)EditorGUILayout.EnumPopup("LoopType", script.loop);

            EditorGUILayout.Space();
            script.ignoreTimeScalest = EditorGUILayout.Toggle("Ignore time scale", script.ignoreTimeScalest);

            EditorGUILayout.Space();
            script.complete = EditorGUILayout.Toggle("Complete event", script.complete);


            if (script.complete)
            {
                SerializedProperty onCheck2 = serializedObject.FindProperty("unityevent");
                EditorGUILayout.PropertyField(onCheck2);
            }

        }


        if (GUI.changed)
            serializedObject.ApplyModifiedProperties();
    }
}
