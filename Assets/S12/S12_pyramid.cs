using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class S12_pyramid : MonoBehaviour
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

        if (meshFilter.sharedMesh == null)
        {
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(-1f, 0f, -1f), // 0: 밑면 왼쪽 뒤
                new Vector3( 1f, 0f, -1f), // 1: 밑면 오른쪽 뒤
                new Vector3( 1f, 0f,  1f), // 2: 밑면 오른쪽 앞
                new Vector3(-1f, 0f,  1f), // 3: 밑면 왼쪽 앞
                new Vector3( 0f, 2f,  0f)  // 4: 꼭짓점
            };

            int[] triangles = new int[]
            {
                0, 2, 1, // 밑면
                0, 3, 2, // 밑면
                0, 1, 4, // 뒤쪽 면
                1, 2, 4, // 오른쪽 면
                2, 3, 4, // 앞쪽 면
                3, 0, 4  // 왼쪽 면
            };

            generatedMesh = new Mesh();
            generatedMesh.name = "S12_PyramidMesh";
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
                generatedMaterial.name = "S12_PyramidMaterial";
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
