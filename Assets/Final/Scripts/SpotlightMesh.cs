using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SpotlighttMesh : MonoBehaviour
{
    public enum PartType
    {
        Base,
        Yoke,
        Head,
        Lens
    }

    [Header("Part Settings")]
    public PartType partType;

    [Header("Mesh Settings")]
    [Range(8, 48)]
    public int segments = 24;

    private Mesh generatedMesh;

    void OnEnable()
    {
        Generate();
    }

    void OnValidate()
    {
        if (isActiveAndEnabled)
            Generate();
    }

    [ContextMenu("Generate Mesh")]
    public void Generate()
    {
        if (segments < 3) return;

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> triangles = new List<int>();

        // 1. Base
        if (partType == PartType.Base)
        {
            AddCylinder(
                vertices, normals, uvs, triangles,
                new Vector3(0, 0, 0),
                new Vector3(0, 0.12f, 0),
                0.22f
            );
        }

        // 2. Yoke
        else if (partType == PartType.Yoke)
        {
            AddCylinder(
                vertices, normals, uvs, triangles,
                new Vector3(-0.14f, 0, 0),
                new Vector3(-0.14f, 0.48f, 0),
                0.035f
            );

            AddCylinder(
                vertices, normals, uvs, triangles,
                new Vector3(0.14f, 0, 0),
                new Vector3(0.14f, 0.48f, 0),
                0.035f
            );

            // Upper connecting bar
            AddCylinder(
                vertices, normals, uvs, triangles,
                new Vector3(-0.14f, 0.44f, 0),
                new Vector3(0.14f, 0.44f, 0),
                0.035f
            );
        }

        // 3. Head
        else if (partType == PartType.Head)
        {
            AddCylinder(
                vertices, normals, uvs, triangles,
                new Vector3(0, 0, -0.24f),
                new Vector3(0, 0, 0.24f),
                0.18f
            );
        }

        // 4. Lens
        else if (partType == PartType.Lens)
        {
            AddCylinder(
                vertices, normals, uvs, triangles,
                new Vector3(0, 0, -0.04f),
                new Vector3(0, 0, 0),
                0.145f
            );
        }

        // Create or reuse mesh
        MeshFilter filter = GetComponent<MeshFilter>();

        if (generatedMesh == null)
        {
            generatedMesh = new Mesh();
            generatedMesh.name = "Spotlight_" + partType;
        }

        generatedMesh.Clear();

        generatedMesh.SetVertices(vertices);
        generatedMesh.SetNormals(normals);
        generatedMesh.SetUVs(0, uvs);
        generatedMesh.SetTriangles(triangles, 0);

        generatedMesh.RecalculateBounds();

        filter.sharedMesh = generatedMesh;
    }

    void AddCylinder(
        List<Vector3> vertices,
        List<Vector3> normals,
        List<Vector2> uvs,
        List<int> triangles,
        Vector3 start,
        Vector3 end,
        float radius)
    {
        Vector3 direction = (end - start).normalized;

        Vector3 axisX = Vector3.Cross(direction, Vector3.up);

        if (axisX.sqrMagnitude < 0.001f)
        {
            axisX = Vector3.Cross(direction, Vector3.right);
        }

        axisX.Normalize();

        Vector3 axisZ = Vector3.Cross(direction, axisX).normalized;

        int offset = vertices.Count;

        // Side vertices
        // Duplicate vertices at the UV seam.
        for (int ring = 0; ring < 2; ring++)
        {
            Vector3 center = ring == 0 ? start : end;

            for (int i = 0; i <= segments; i++)
            {
                float angle = 2f * Mathf.PI * i / segments;

                Vector3 radial =
                    axisX * Mathf.Cos(angle) +
                    axisZ * Mathf.Sin(angle);

                vertices.Add(center + radial * radius);
                normals.Add(radial);

                uvs.Add(new Vector2(
                    (float)i / segments,
                    ring
                ));
            }
        }

        // Side triangles
        for (int i = 0; i < segments; i++)
        {
            int a = offset + i;
            int b = offset + i + 1;
            int c = offset + segments + 1 + i;
            int d = offset + segments + 2 + i;

            triangles.Add(a);
            triangles.Add(c);
            triangles.Add(b);

            triangles.Add(b);
            triangles.Add(c);
            triangles.Add(d);
        }

        // Separate end-cap vertices.
        AddCap(
            vertices, normals, uvs, triangles,
            start, -direction, radius, axisX, axisZ
        );

        AddCap(
            vertices, normals, uvs, triangles,
            end, direction, radius, axisX, axisZ
        );
    }

    void AddCap(
        List<Vector3> vertices,
        List<Vector3> normals,
        List<Vector2> uvs,
        List<int> triangles,
        Vector3 center,
        Vector3 normal,
        float radius,
        Vector3 axisX,
        Vector3 axisZ)
    {
        int centerIndex = vertices.Count;

        vertices.Add(center);
        normals.Add(normal);
        uvs.Add(new Vector2(0.5f, 0.5f));

        int ringStart = vertices.Count;

        for (int i = 0; i <= segments; i++)
        {
            float angle = 2f * Mathf.PI * i / segments;

            float x = Mathf.Cos(angle);
            float z = Mathf.Sin(angle);

            Vector3 radial = axisX * x + axisZ * z;

            vertices.Add(center + radial * radius);
            normals.Add(normal);

            uvs.Add(new Vector2(
                0.5f + 0.5f * x,
                0.5f + 0.5f * z
            ));
        }

        // Cap triangles with correct winding.
        for (int i = 0; i < segments; i++)
        {
            int a = centerIndex;
            int b = ringStart + i;
            int c = ringStart + i + 1;

            if (Vector3.Dot(
                Vector3.Cross(
                    vertices[b] - vertices[a],
                    vertices[c] - vertices[a]
                ),
                normal
            ) > 0)
            {
                triangles.Add(a);
                triangles.Add(b);
                triangles.Add(c);
            }
            else
            {
                triangles.Add(a);
                triangles.Add(c);
                triangles.Add(b);
            }
        }
    }
}
