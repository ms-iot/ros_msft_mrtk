﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LidarVisualizer))]
public class LidarVisualizerEditor : Editor
{
    // global settings
    SerializedProperty renderCallsPerSecond;
    SerializedProperty lidarResolution;
    SerializedProperty worldScale;

    SerializedProperty lidarDataProviderType;
    SerializedProperty randomRange;
    SerializedProperty spaceRendererType;
    SerializedProperty ringHeight;



    private void OnEnable()
    {
        renderCallsPerSecond = serializedObject.FindProperty("renderCallsPerSecond");
        lidarResolution = serializedObject.FindProperty("lidarResolution");
        worldScale = serializedObject.FindProperty("worldScale");
        lidarDataProviderType = serializedObject.FindProperty("lidarDataProviderType");
        randomRange = serializedObject.FindProperty("randomRange");
        spaceRendererType = serializedObject.FindProperty("spaceRendererType");
        ringHeight = serializedObject.FindProperty("ringHeight");

        
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(renderCallsPerSecond);
        EditorGUILayout.PropertyField(lidarResolution);
        EditorGUILayout.PropertyField(worldScale);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(lidarDataProviderType);
        switch((LidarDataProviderClass)lidarDataProviderType.enumValueIndex)
        {
            case LidarDataProviderClass.SIMPLE_RANDOM:
                EditorGUILayout.PropertyField(randomRange);
                break;
        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(spaceRendererType);
        switch((SpaceRendererClass)spaceRendererType.enumValueIndex) {
            case SpaceRendererClass.RING_MESH:
                EditorGUILayout.PropertyField(ringHeight);
                break;
        }

        serializedObject.ApplyModifiedProperties();
        
    }
}
