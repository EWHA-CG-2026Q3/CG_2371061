
using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class StraightStairMesh : MonoBehaviour
{
    [Header("Stair Settings")]
    public int stepCount = 4;
    public float width = 1.8f;
    public float treadDepth = 0.45f;
    public float stepHeight = 0.35f;

    private Mesh generatedMesh;

    void OnEnable()
    {
        GenerateMesh();
    }

    void OnValidate()
    {
        GenerateMesh();
    }

    [ContextMenu("Generate Stairs")]
    public void GenerateMesh()
    {
        if (stepCount < 1) return;

        MeshFilter filter = GetComponent<MeshFilter>();

        if (generatedMesh != null)
        {
            if (Application.isPlaying)
                Destroy(generatedMesh);
            else
                DestroyImmediate(generatedMesh);
        }

        generatedMesh = new Mesh();
        generatedMesh.name = "Straight Stair Mesh";

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        float halfWidth = width / 2f;

        // 1. 계단 윗면 생성
        for (int i = 0; i < stepCount; i++)
        {
            float y = (i + 1) * stepHeight;
            float z0 = i * treadDepth;
            float z1 = (i + 1) * treadDepth;

            AddQuad(
                vertices, triangles,
                new Vector3(-halfWidth, y, z0),
                new Vector3(halfWidth, y, z0),
                new Vector3(halfWidth, y, z1),
                new Vector3(-halfWidth, y, z1)
            );
        }

        // 2. 계단 앞면(수직면) 생성
        for (int i = 0; i < stepCount; i++)
        {
            float y0 = i * stepHeight;
            float y1 = (i + 1) * stepHeight;
            float z = i * treadDepth;

            AddQuad(
                vertices, triangles,
                new Vector3(-halfWidth, y0, z),
                new Vector3(halfWidth, y0, z),
                new Vector3(halfWidth, y1, z),
                new Vector3(-halfWidth, y1, z)
            );
        }

        // 3. 계단 바닥 생성
        AddQuad(
            vertices, triangles,
            new Vector3(-halfWidth, 0, 0),
            new Vector3(halfWidth, 0, 0),
            new Vector3(halfWidth, 0, stepCount * treadDepth),
            new Vector3(-halfWidth, 0, stepCount * treadDepth)
        );

        // 4. 양쪽 측면 생성
        List<Vector3> profile = new List<Vector3>();

        profile.Add(new Vector3(0, 0, 0));
        profile.Add(new Vector3(0, 0, stepCount * treadDepth));
        profile.Add(new Vector3(0, stepCount * stepHeight,
                                stepCount * treadDepth));

        for (int i = stepCount - 1; i >= 0; i--)
        {
            profile.Add(new Vector3(
                0, (i + 1) * stepHeight, i * treadDepth));

            profile.Add(new Vector3(
                0, i * stepHeight, i * treadDepth));
        }

        for (int side = -1; side <= 1; side += 2)
        {
            float x = side * halfWidth;

            Vector3 origin = new Vector3(
                x, profile[0].y, profile[0].z);

            for (int i = 1; i < profile.Count - 1; i++)
            {
                Vector3 a = new Vector3(
                    x, profile[i].y, profile[i].z);

                Vector3 b = new Vector3(
                    x, profile[i + 1].y, profile[i + 1].z);

                AddTriangle(vertices, triangles, origin, a, b);
            }
        }

        // 5. 메시 적용
        generatedMesh.SetVertices(vertices);
        generatedMesh.SetTriangles(triangles, 0);
        generatedMesh.RecalculateNormals();
        generatedMesh.RecalculateBounds();

        filter.sharedMesh = generatedMesh;
    }

    // 사각형을 두 개의 삼각형으로 생성
    void AddQuad(
        List<Vector3> vertices,
        List<int> triangles,
        Vector3 a, Vector3 b,
        Vector3 c, Vector3 d)
    {
        AddTriangle(vertices, triangles, a, b, c);
        AddTriangle(vertices, triangles, a, c, d);
    }

    // 삼각형 생성 (양면에서 보이도록 처리)
    
    void AddTriangle(
        List<Vector3> vertices,
        List<int> triangles,
        Vector3 a, Vector3 b, Vector3 c)
    {
        // 앞면
        int start = vertices.Count;

        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);

        triangles.Add(start);
        triangles.Add(start + 1);
        triangles.Add(start + 2);

        // 뒷면은 별도의 정점을 사용
        int back = vertices.Count;

        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);

        triangles.Add(back + 2);
        triangles.Add(back + 1);
        triangles.Add(back);
    }
}
