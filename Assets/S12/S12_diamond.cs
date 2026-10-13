using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class S12_diamond : MonoBehaviour
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
                new Vector3(-1f, 0f, -1f), // 0
                new Vector3(1f, 0f, -1f),  // 1
                new Vector3(1f, 0f, 1f),   // 2
                new Vector3(-1f, 0f, 1f),  // 3
                new Vector3(0f, 1f, 0f),   // 4
                new Vector3(0f, -1f, 0f),  // 5
            };

            int[] triangles = new int[]
            {
                4, 0, 3,
                4, 1, 0,
                4, 2, 1,
                4, 3, 2,
                0, 5, 3,
                1, 5, 0,
                2, 5, 1,
                3, 5, 2
            };

            generatedMesh = new Mesh();
            generatedMesh.name = "S04_CustomDiamondMesh";
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
                generatedMaterial.name = "S04_DiamondMaterial";
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
