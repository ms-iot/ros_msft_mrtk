using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RingMeshRenderer : MonoBehaviour, ISpaceRenderer
{
    private readonly int PREDICTED_LIDAR_RESOLUTION = 1000;
    private readonly float RING_HEIGHT = 1.5f;

    private GameObject _meshHolder;
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private Mesh _mesh;

    /// <summary>
    /// Represents the vertices of the rendered mesh. Format: series of two-length columns. 
    /// </summary>
    /// 1 - 3 - 5 - 7 - 9 ....
    /// | / | / | / | / | ....
    /// 0 - 2 - 4 - 6 - 8 ....
    private Vector3[] _verts;
    private int _logicalVertsCount;
    private BitArray _fakeVertFlags;
    private int[] _triangles;


    // Invariant: _logicalVertsCount < (_verts.Length / 2)
    // Invariant: _verts.Length is even
    // Invariant: _fakeVertFlags.Length == (_verts.Length / 2)
    // Invariant: _triangles.Length == (_verts.Length / 2) * 12
    private bool CheckInvariants()
    {
        bool _vertsState = _logicalVertsCount <= (_verts.Length / 2) && _verts.Length % 2 == 0;
        if (!_vertsState)
        {
            Debug.LogError("RingMeshRenderer: counts");
        }
        bool flagsLen = _fakeVertFlags.Length == (_verts.Length / 2);
        if (!flagsLen)
        {
            Debug.LogError("RingMeshRenderer: flagsLen");
        }
        bool trisLen = _triangles.Length == (_verts.Length / 2) * 12;
        if (!trisLen)
        {
            Debug.LogError("RingMeshRenderer: trisLen");
        }


        bool output = _vertsState && flagsLen && trisLen;
        if (!output)
        {
            Debug.LogError("Invariants failed to hold in RingMeshRenderer!");
        }
        return output;
    }

    public void Render(float[] lidarData, Transform origin)
    {
        if (_meshHolder == null)
        {
            Init(origin);
        }

        ResizeMesh(lidarData.Length);

        for (int vInd = 0, tInd = 0; vInd < (_verts.Length); vInd += 2, tInd += 3)
        {
            // vInd = index for column in the ladder; 
            //   vInd+1 = second ring/top of column which
            //   should vary from vInd only by y displacement
            float rad = ((float)vInd / (float)lidarData.Length) * (2 * Mathf.PI);
            // offset by 90 degrees so that first data point corresponds to x axis/straight ahead
            Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * lidarData[vInd / 2];
            _verts[vInd] = offset;
            _verts[vInd + 1] = offset + Vector3.up * RING_HEIGHT;
        }

        _mesh.vertices = _verts;
        //debug
        for (int i = 0; i < _verts.Length; i++)
        {
            if (_verts[i] == Vector3.zero)
            {
                Debug.LogWarning(i);
            }
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
        // Building a ladder-shaped mesh with two identical rings of vertices
        _verts = new Vector3[PREDICTED_LIDAR_RESOLUTION * 2];
        _logicalVertsCount = _verts.Length / 2;
        _triangles = new int[PREDICTED_LIDAR_RESOLUTION * 12];
        _fakeVertFlags = new BitArray(PREDICTED_LIDAR_RESOLUTION, false);
        _mesh.vertices = _verts;
        _mesh.triangles = _triangles;
        _meshFilter.mesh = _mesh;

        WeaveTriangles();
        
    }

    private void ResizeMesh(int size)
    {
        CheckInvariants();
        return;
        /*
        if (size < _logicalVertsCount)
        {
            int fakeVerts = (_verts.Length / 2) - size;
            for (int i = size; i < _ballCacheSize; i++)
            {
                _ballCache[i].SetActive(false);
            }
            _ballCacheSize = size;

        }
        else if (size > _logicalVertsCount)
        {
            // only rebuild the entire array if the new size exceeds the PHYSICAL size of the cache
            if (size > _ballCache.Length)
            {
                GameObject[] newCache = new GameObject[size];
                for (int i = 0; i < _ballCacheSize; i++)
                {
                    newCache[i] = _ballCache[i];
                }
                _ballCache = newCache;
            }

            _ballCacheSize = size;
        } */
        CheckInvariants();
    }

    private void WeaveTriangles()
    {
        CheckInvariants();

        // edge case: start
        _triangles[0] = 3;
        _triangles[1] = 1;
        _triangles[2] = 0;
        _triangles[3] = 0;
        _triangles[4] = 1;
        _triangles[5] = 3;

        // edge case: end
        _triangles[6] = _verts.Length - 2;
        _triangles[7] = _verts.Length - 1;
        _triangles[8] = _verts.Length - 4;
        _triangles[9] = _verts.Length - 4;
        _triangles[10] = _verts.Length - 1;
        _triangles[11] = _verts.Length - 2;

        // edge case: loop upper (where the ladder jumps from last vertice back to first)
        _triangles[12] = 1;
        _triangles[13] = _verts.Length - 1;
        _triangles[14] = _verts.Length - 2;
        _triangles[15] = _verts.Length - 2;
        _triangles[16] = _verts.Length - 1;
        _triangles[17] = 1;

        // edge case: loop lower
        _triangles[18] = 0;
        _triangles[19] = 1;
        _triangles[20] = _verts.Length - 2;
        _triangles[21] = _verts.Length - 2;
        _triangles[22] = 1;
        _triangles[23] = 0;

        for (int tInd = 24, vInd = 2; tInd < _triangles.Length; tInd += 12, vInd += 2)
        {
            // tInd = index for the start of a triangle;
            //   tInd+1 = second vertice in triangle;
            //   tInd+2 = third vertice in triangle;
            //   triangle vertice ordering must be clockwise 
            //   for the polygon to be seen
            // vInd = index for column in the ladder; 
            //   vInd+1 = second ring/top of column 

            // lower triangle
            _triangles[tInd] = vInd;
            _triangles[tInd + 1] = vInd + 1;
            _triangles[tInd + 2] = vInd - 2;
            _triangles[tInd + 3] = vInd - 2;
            _triangles[tInd + 4] = vInd + 1;
            _triangles[tInd + 5] = vInd;

            // upper triangle
            _triangles[tInd + 6] = vInd + 3;
            _triangles[tInd + 7] = vInd + 1;
            _triangles[tInd + 8] = vInd;
            _triangles[tInd + 9] = vInd;
            _triangles[tInd + 10] = vInd + 1;
            _triangles[tInd + 11] = vInd + 3;
        }

        _mesh.triangles = _triangles;

        
    }

    
}
