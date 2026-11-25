using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class StarPrismMesh : MonoBehaviour
{
    [Header("Star Shape")]
    [Range(3, 32)]
    public int pointCount = 8;

    [Min(0.1f)]
    public float outerRadius = 1f;

    [Range(0.1f, 0.9f)]
    public float innerRadius = 0.45f;

    [Min(0.05f)]
    public float depth = 0.8f;

    [Range(0.1f, 0.9f)]
    public float centerInset = 0.35f;

    private Mesh generatedMesh;

    private void OnEnable()
    {
        GenerateMesh();
    }

    private void OnValidate()
    {
        GenerateMesh();
    }

    [ContextMenu("Generate Star Mesh")]
    public void GenerateMesh()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter == null) return;

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        int count = pointCount * 2;

        Vector3[] front = new Vector3[count];
        Vector3[] back = new Vector3[count];

        float halfDepth = depth * 0.5f;

        for (int i = 0; i < count; i++)
        {
            float angle = Mathf.PI * 2f * i / count
                          - Mathf.PI * 0.5f;

            float radius = (i % 2 == 0)
                ? outerRadius
                : outerRadius * innerRadius;

            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            front[i] = new Vector3(x, y, halfDepth);
            back[i] = new Vector3(x, y, -halfDepth);
        }

        Vector3 frontCenter = new Vector3(
            0, 0, halfDepth - depth * centerInset
        );

        Vector3 backCenter = new Vector3(
            0, 0, -halfDepth + depth * centerInset
        );

        // Front star surface
        for (int i = 0; i < count; i++)
        {
            int next = (i + 1) % count;

            AddTriangle(
                frontCenter, front[i], front[next],
                vertices, uvs, triangles
            );
        }

        // Back star surface
        for (int i = 0; i < count; i++)
        {
            int next = (i + 1) % count;

            AddTriangle(
                backCenter, back[next], back[i],
                vertices, uvs, triangles
            );
        }

        // Side surfaces
        for (int i = 0; i < count; i++)
        {
            int next = (i + 1) % count;

            AddTriangle(
                front[i], back[i], back[next],
                vertices, uvs, triangles
            );

            AddTriangle(
                front[i], back[next], front[next],
                vertices, uvs, triangles
            );
        }

        Mesh mesh = new Mesh();
        mesh.name = "Generated Star Mesh";
        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetTriangles(triangles, 0);

        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();

        if (generatedMesh != null)
        {
            if (Application.isPlaying)
                Destroy(generatedMesh);
            else
                DestroyImmediate(generatedMesh);
        }

        generatedMesh = mesh;
        filter.sharedMesh = generatedMesh;
    }

    private void AddTriangle(
        Vector3 a, Vector3 b, Vector3 c,
        List<Vector3> vertices,
        List<Vector2> uvs,
        List<int> triangles)
    {
        int index = vertices.Count;

        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);

        uvs.Add(new Vector2(0.5f, 0.5f));
        uvs.Add(new Vector2(1f, 0f));
        uvs.Add(new Vector2(0f, 1f));

        triangles.Add(index);
        triangles.Add(index + 1);
        triangles.Add(index + 2);
    }
}