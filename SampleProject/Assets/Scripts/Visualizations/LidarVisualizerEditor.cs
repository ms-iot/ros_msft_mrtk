using System.Collections;
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
    SerializedProperty rosConnectorTimeout;
    SerializedProperty rosBridgeURL;
    SerializedProperty topic;
    SerializedProperty spaceRendererType;
    SerializedProperty ringHeight;



    private void OnEnable()
    {
        renderCallsPerSecond = serializedObject.FindProperty("renderCallsPerSecond");
        lidarResolution = serializedObject.FindProperty("lidarResolution");
        worldScale = serializedObject.FindProperty("worldScale");

        lidarDataProviderType = serializedObject.FindProperty("lidarDataProviderType");
        randomRange = serializedObject.FindProperty("randomRange");
        rosConnectorTimeout = serializedObject.FindProperty("rosConnectorTimeout");
        rosBridgeURL = serializedObject.FindProperty("rosBridgeURL");
        topic = serializedObject.FindProperty("topic");

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
            case LidarDataProviderClass.ROS1:
#if ROS1_MODULE_LIDAR
                EditorGUILayout.PropertyField(rosConnectorTimeout);
                EditorGUILayout.PropertyField(rosBridgeURL);
                EditorGUILayout.PropertyField(topic);
#else
                EditorGUILayout.HelpBox("The Ros1 lidar module has not been loaded. Check the readme for more details.", MessageType.Warning);
#endif
                break;
            case LidarDataProviderClass.ROS2:
#if ROS2_MODULE_LIDAR
#else
                EditorGUILayout.HelpBox("The Ros2 lidar module has not been loaded. Check the readme for more details.", MessageType.Warning);
#endif
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
