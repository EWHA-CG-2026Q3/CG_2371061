
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CymbalMesh : MonoBehaviour
{
    [Header("Cymbal Shape")]
    [Range(16, 128)]
    public int segments = 64;

    [Range(4, 64)]
    public int rings = 32;

    public float radius = 0.65f;
    public float bellHeight = 0.35f;
    public float bellRadius = 0.22f;

    private Mesh generatedMesh;

    void OnEnable()
    {
        GenerateMesh();
    }

    void OnValidate()
    {
        segments = Mathf.Max(3, segments);
        rings = Mathf.Max(2, rings);
        radius = Mathf.Max(0.01f, radius);
        bellRadius = Mathf.Clamp(
            bellRadius, 0.01f, radius - 0.001f
        );

        GenerateMesh();
    }

    [ContextMenu("Generate Cymbal Mesh")]
    public void GenerateMesh()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter == null) return;

        Vector3[] vertices =
            new Vector3[(rings + 1) * (segments + 1)];

        Vector2[] uvs = new Vector2[vertices.Length];

        int[] triangles = new int[rings * segments * 6];

        // 1. Generate curved surface vertices
        for (int ring = 0; ring <= rings; ring++)
        {
            float t = (float)ring / rings;
            float r = radius * t;

            float y;

            if (r <= bellRadius)
            {
                float bellT = r / bellRadius;

                // Raised central bell
                y = bellHeight *
                    (1f - 0.5f * bellT * bellT);
            }
            else
            {
                float outerT =
                    (r - bellRadius) /
                    (radius - bellRadius);

                // Gradually descending outer surface
                y = bellHeight * 0.5f *
                    (1f - outerT * outerT);
            }

            for (int i = 0; i <= segments; i++)
            {
                float angle =
                    2f * Mathf.PI * i / segments;

                int index =
                    ring * (segments + 1) + i;

                float x = Mathf.Cos(angle) * r;
                float z = Mathf.Sin(angle) * r;

                vertices[index] = new Vector3(x, y, z);

                // UV mapping
                uvs[index] = new Vector2(
                    0.5f + x / (2f * radius),
                    0.5f + z / (2f * radius)
                );
            }
        }

        // 2. Generate triangles
        int tri = 0;

        for (int ring = 0; ring < rings; ring++)
        {
            for (int i = 0; i < segments; i++)
            {
                int a = ring * (segments + 1) + i;
                int b = a + 1;
                int c = a + segments + 1;
                int d = c + 1;

                // Upward-facing triangle winding
                triangles[tri++] = a;
                triangles[tri++] = b;
                triangles[tri++] = c;

                triangles[tri++] = b;
                triangles[tri++] = d;
                triangles[tri++] = c;
            }
        }

        // 3. Create mesh
        Mesh mesh = new Mesh();
        mesh.name = "Generated_Cymbal_Curved";

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        // 4. Calculate normals and bounds
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // 5. Replace previous generated mesh
        Mesh oldMesh = generatedMesh;

        generatedMesh = mesh;
        filter.sharedMesh = generatedMesh;

        if (oldMesh != null)
        {
            if (Application.isPlaying)
                Destroy(oldMesh);
            else
                DestroyImmediate(oldMesh);
        }
    }

    void OnDisable()
    {
    }
}
