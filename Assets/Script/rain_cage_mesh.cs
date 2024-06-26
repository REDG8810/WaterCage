using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class rain_cage_mesh : MonoBehaviour
{
    public bool removeExistingColliders = true;
    private Stopwatch stopwatch;
    private float elapsedTime;
    [SerializeField]
    float
        destroyTime = 20.0f;
    [SerializeField]
    public float
        scale = 3.0f;

    private void Start()
    {
        this.transform.localScale = new Vector3(scale, scale, scale);
        stopwatch = new Stopwatch();
        stopwatch.Start();
        CreateInvertedMeshCollider();
    }

    private void Update()
    {
        elapsedTime = stopwatch.ElapsedMilliseconds / 1000f;
        destroyThisObject();
    }

    public void CreateInvertedMeshCollider()
    {
        if (removeExistingColliders)
            RemoveExistingColliders();

        InvertMesh();

        gameObject.AddComponent<MeshCollider>();
    }

    private void RemoveExistingColliders()
    {
        Collider[] colliders = GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; i++)
            DestroyImmediate(colliders[i]);
    }

    private void InvertMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.triangles = mesh.triangles.Reverse().ToArray();
    }

    private void destroyThisObject()
    {
        if (destroyTime < elapsedTime)
        {
            Destroy(this.gameObject);
        }
    }
}
