using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Box : MonoBehaviour
{
    public Rect shape;

    public void SetShape(Rect s)
    {
        shape = s;
    }

    public Rect GetShape()
    {
        return shape;
    }

    public void GenerateMesh(List<Vector2> vertices)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshCollider meshCollider = GetComponent<MeshCollider>();

        if (meshCollider)
            meshCollider.convex = true;

        //Undo.RecordObject(meshFilter, "Mesh");
        //Undo.RecordObject(meshCollider, "Mesh");


        List<Vector3> vertice3 = new List<Vector3>(vertices.Count * 6);
        Vector3 zOffset = new Vector3(0, 0, 2);
        foreach (Vector3 vertex in vertices)
        {
            vertice3.Add(vertex - zOffset);
        }

        foreach (Vector3 vertex in vertices)
        {
            vertice3.Add(vertex + zOffset);
        }

        vertice3.Add(vertice3[0]); // 8
        vertice3.Add(vertice3[4]); // 9
        vertice3.Add(vertice3[5]); // 10
        vertice3.Add(vertice3[1]); // 11
        vertice3.Add(vertice3[3]); // 12
        vertice3.Add(vertice3[2]); // 13
        vertice3.Add(vertice3[6]); // 14
        vertice3.Add(vertice3[7]); // 15
        vertice3.Add(vertice3[0]); // 16
        vertice3.Add(vertice3[3]); // 17
        vertice3.Add(vertice3[7]); // 18
        vertice3.Add(vertice3[4]); // 19
        vertice3.Add(vertice3[2]); // 20
        vertice3.Add(vertice3[1]); // 21
        vertice3.Add(vertice3[5]); // 22
        vertice3.Add(vertice3[6]); // 23

        List<int> triangles = new List<int>(24);
        triangles.Add(0);
        triangles.Add(2);
        triangles.Add(1);
        triangles.Add(0);
        triangles.Add(3);
        triangles.Add(2);

        triangles.Add(5);
        triangles.Add(7);
        triangles.Add(4);
        triangles.Add(5);
        triangles.Add(6);
        triangles.Add(7);

        triangles.Add(19);
        triangles.Add(17);
        triangles.Add(16);
        triangles.Add(19);
        triangles.Add(18);
        triangles.Add(17);

        triangles.Add(21);
        triangles.Add(23);
        triangles.Add(22);
        triangles.Add(21);
        triangles.Add(20);
        triangles.Add(23);

        triangles.Add(12);
        triangles.Add(14);
        triangles.Add(13);
        triangles.Add(12);
        triangles.Add(15);
        triangles.Add(14);

        triangles.Add(9);
        triangles.Add(11);
        triangles.Add(10);
        triangles.Add(9);
        triangles.Add(8);
        triangles.Add(11);

        Vector2[] uvs = new Vector2[vertice3.Count];
        for (int i = 0; i < vertice3.Count; ++i)
        {
            Vector2 v = vertice3[i];
            v.x /= 1.0f;
            v.y /= 1.0f;
            uvs[i] = v;
        }

        Mesh mesh = new Mesh
        {
            vertices = vertice3.ToArray(),
            triangles = triangles.ToArray(),
            uv = uvs
        };

        meshFilter.sharedMesh = mesh;
        meshFilter.mesh = mesh;
        //Undo.RegisterCreatedObjectUndo(mesh, "Mesh");
        if (meshCollider)
            meshCollider.sharedMesh = mesh;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
