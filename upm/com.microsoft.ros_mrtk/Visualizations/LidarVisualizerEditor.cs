using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(LidarVisualizer))]
public class LidarVisualizerEditor : Editor
{
    // global settings
    SerializedProperty renderCallsPerSecond;
    SerializedProperty lidarResolution;

    SerializedProperty lidarDataProviderType;
    SerializedProperty randomRange;
    SerializedProperty topic;
    SerializedProperty spaceRendererType;
    SerializedProperty ringHeight;
    SerializedProperty ballPrefab;
    SerializedProperty ballLineMaterial;



    private void OnEnable()
    {
        renderCallsPerSecond = serializedObject.FindProperty("renderCallsPerSecond");
        lidarResolution = serializedObject.FindProperty("lidarResolution");

        lidarDataProviderType = serializedObject.FindProperty("lidarDataProviderType");
        randomRange = serializedObject.FindProperty("randomRange");
        topic = serializedObject.FindProperty("topic");

        spaceRendererType = serializedObject.FindProperty("spaceRendererType");
        ringHeight = serializedObject.FindProperty("ringHeight");

        ballPrefab = serializedObject.FindProperty("ballPrefab");
        ballLineMaterial = serializedObject.FindProperty("ballLineMaterial");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(topic);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(lidarDataProviderType);
        switch((LidarDataProviderClass)lidarDataProviderType.enumValueIndex)
        {
            case LidarDataProviderClass.SIMPLE_RANDOM:
                EditorGUILayout.PropertyField(randomRange);
                EditorGUILayout.PropertyField(renderCallsPerSecond);
                EditorGUILayout.PropertyField(lidarResolution);
                break;
            case LidarDataProviderClass.ROS2:
                break;
        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(spaceRendererType);
        switch((SpaceRendererClass)spaceRendererType.enumValueIndex) {
            case SpaceRendererClass.RING_MESH:
                EditorGUILayout.PropertyField(ringHeight);
                break;
            case SpaceRendererClass.BALL:
                EditorGUILayout.PropertyField(ballPrefab);
            break;
            case SpaceRendererClass.BALL_LINE:
                EditorGUILayout.PropertyField(ballPrefab);
                EditorGUILayout.PropertyField(ballLineMaterial);
            break;
        }

        serializedObject.ApplyModifiedProperties();
        
    }
}
#endif
