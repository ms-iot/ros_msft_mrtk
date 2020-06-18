using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RingMeshRenderer : MonoBehaviour, ISpaceRenderer
{

    private GameObject _meshHolder;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private Mesh _mesh;

    public void Render(float[] lidarData, Transform origin)
    {
        //throw new System.NotImplementedException();
        if (_meshHolder == null)
        {
            Init(origin);
        }
    }

    private void Init(Transform origin)
    {
        _meshHolder = new GameObject("RingMesh");
        _meshHolder.transform.parent = origin;
        _meshHolder.transform.localPosition = Vector3.zero;

        _meshFilter = _meshHolder.AddComponent<MeshFilter>();

        _meshRenderer = _meshHolder.AddComponent<MeshRenderer>();
        _meshRenderer.material = Resources.Load<Material>("Shaders/MRTK_WireframeBlue");
        if (_meshRenderer.material == null)
        {
            Debug.LogError("Failed to load ringmesh material!");
        }

        _mesh = new Mesh();
        _mesh.name = "Lidar Data";
        _meshFilter.mesh = _mesh;
    }
}
