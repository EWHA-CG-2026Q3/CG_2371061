
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class DrumCylinderMesh : MonoBehaviour
{
    public int segments = 24;
    public float radius = 1f;
    public float height = 1f;

    private Mesh generatedMesh;

    // 오브젝트가 활성화될 때 메시 생성
    void OnEnable()
    {
        CreateCylinder();
    }

    // Inspector 값이 변경될 때 메시 갱신
    void OnValidate()
    {
        segments = Mathf.Max(3, segments);
        radius = Mathf.Max(0.01f, radius);
        height = Mathf.Max(0.01f, height);

        CreateCylinder();
    }

    void CreateCylinder()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        if (meshFilter == null)
            return;

        // 기존 메시 정리
        if (generatedMesh != null)
        {
            if (Application.isPlaying)
                Destroy(generatedMesh);
            else
                DestroyImmediate(generatedMesh);
        }

        Mesh mesh = new Mesh();
        mesh.name = "Generated Drum Cylinder";

        generatedMesh = mesh;

        Vector3[] vertices = new Vector3[segments * 2 + 2];
        int[] triangles = new int[segments * 12];

        float halfHeight = height / 2f;

        // 아래쪽과 위쪽 원의 정점 생성
        for (int i = 0; i < segments; i++)
        {
            float angle = 2f * Mathf.PI * i / segments;

            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);

            vertices[i] = new Vector3(x, -halfHeight, z);

            vertices[i + segments] =
                new Vector3(x, halfHeight, z);
        }

        // 원판 중심 정점
        int bottomCenter = segments * 2;
        int topCenter = segments * 2 + 1;

        vertices[bottomCenter] =
            new Vector3(0, -halfHeight, 0);

        vertices[topCenter] =
            new Vector3(0, halfHeight, 0);

        int t = 0;

        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;

            int b1 = i;
            int b2 = next;
            int u1 = i + segments;
            int u2 = next + segments;

            // 원기둥 옆면
            triangles[t++] = b1;
            triangles[t++] = u1;
            triangles[t++] = b2;

            triangles[t++] = b2;
            triangles[t++] = u1;
            triangles[t++] = u2;

            // 아래쪽 원판
            triangles[t++] = bottomCenter;
            triangles[t++] = b1;
            triangles[t++] = b2;

            // 위쪽 원판
            triangles[t++] = topCenter;
            triangles[t++] = u2;
            triangles[t++] = u1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;
    }

    void OnDisable()
    {
        if (generatedMesh != null)
        {
            if (Application.isPlaying)
                Destroy(generatedMesh);
            else
                DestroyImmediate(generatedMesh);

            generatedMesh = null;
        }
    }
}
