using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class cage_mesh : MonoBehaviour
{
    // Use this for initialization
    public bool removeExistingColliders = true;

    private void Start()
    {
        CreateInvertedMeshCollider();
    }

    public void CreateInvertedMeshCollider()
    {
        if (removeExistingColliders)
            RemoveExistingColliders();

        InvertMesh();

        gameObject.AddComponent<MeshCollider>();
    }

    //既存コライダーを消去
    private void RemoveExistingColliders()
    {
        Collider[] colliders = GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; i++)
            DestroyImmediate(colliders[i]);
    }

    //内側にメッシュコライダーを作成
    private void InvertMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.triangles = mesh.triangles.Reverse().ToArray();
    }
}
