using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class S12_polygon : MonoBehaviour
{
    private Mesh generatedMesh;
    private Material generatedMaterial;

    void OnEnable()
    {
        GenerateMesh();
    }

    void GenerateMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        if (meshFilter.sharedMesh == null ||
            generatedMesh == null)
        {
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(0f, 0f, 0f),  // 0
                new Vector3(1f, 0f, 0f),  // 1
                new Vector3(1f, 1f, 0f),  // 2
                new Vector3(0f, 1f, 0f),  // 3
                new Vector3(1f, 2f, 0f),  // 4
                new Vector3(-1f, 1f, 0f), // 5
                new Vector3(0f, -1f, 0f), // 6
                new Vector3(2f, 0f, 0f),  // 7
            };

            int[] triangles = new int[]
            {
                3, 2, 0,
                2, 1, 0,
                2, 7, 1,
                4, 2, 3,
                5, 3, 0,
                0, 1, 6
            };

            generatedMesh = new Mesh();
            generatedMesh.name = "S03_CustomPolygonMesh";
            generatedMesh.vertices = vertices;
            generatedMesh.triangles = triangles;
            generatedMesh.RecalculateNormals();

            meshFilter.sharedMesh = generatedMesh;
        }

        if (meshRenderer.sharedMaterial == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");

            if (shader != null)
            {
                generatedMaterial = new Material(shader);
                generatedMaterial.name = "S03_PolygonMaterial";
                meshRenderer.sharedMaterial = generatedMaterial;
            }
            else
            {
                Debug.LogWarning(
                    "URP/Lit Shader를 찾을 수 없습니다.", this);
            }
        }
    }
}
