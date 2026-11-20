
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DrumRimMesh : MonoBehaviour
{
    [Header("Rim Settings")]
    public float radius = 0.8f;
    public float tubeRadius = 0.025f;

    [Range(16, 96)]
    public int segments = 48;

    [Range(6, 24)]
    public int sides = 12;

    private Mesh generatedMesh;

    void OnEnable()
    {
        GenerateMesh();
    }

    void OnValidate()
    {
        GenerateMesh();
    }

    [ContextMenu("Generate Rim")]
    public void GenerateMesh()
    {
        MeshFilter filter = GetComponent<MeshFilter>();

        if (filter == null) return;

        Vector3[] vertices =
            new Vector3[(segments + 1) * (sides + 1)];

        Vector2[] uvs = new Vector2[vertices.Length];

        int[] triangles = new int[segments * sides * 6];

        for (int i = 0; i <= segments; i++)
        {
            float u = 2f * Mathf.PI * i / segments;

            for (int j = 0; j <= sides; j++)
            {
                float v = 2f * Mathf.PI * j / sides;

                int index = i * (sides + 1) + j;

                float ringRadius =
                    radius + tubeRadius * Mathf.Cos(v);

                float x = ringRadius * Mathf.Cos(u);
                float y = tubeRadius * Mathf.Sin(v);
                float z = ringRadius * Mathf.Sin(u);

                vertices[index] = new Vector3(x, y, z);

                uvs[index] = new Vector2(
                    (float)i / segments,
                    (float)j / sides
                );
            }
        }

        int tri = 0;

        for (int i = 0; i < segments; i++)
        {
            for (int j = 0; j < sides; j++)
            {
                int a = i * (sides + 1) + j;
                int b = a + sides + 1;
                int c = a + 1;
                int d = b + 1;

                triangles[tri++] = a;
                triangles[tri++] = b;
                triangles[tri++] = c;

                triangles[tri++] = c;
                triangles[tri++] = b;
                triangles[tri++] = d;
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "Generated_Drum_Rim";

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        Mesh oldMesh = generatedMesh;

        generatedMesh = mesh;
        filter.sharedMesh = mesh;

        if (oldMesh != null)
        {
            if (Application.isPlaying)
                Destroy(oldMesh);
            else
                DestroyImmediate(oldMesh);
        }
    }
}
