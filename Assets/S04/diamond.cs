using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class S04_CustomCubeMesh : MonoBehaviour
{
    void Start()
    {
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-1f, 0f, -1f), // 0
            new Vector3(1f, 0f, -1f), // 1
            new Vector3(1f, 0f, 1f), // 2
            new Vector3(-1f, 0f, 1f), // 3
            new Vector3(0f, 1f, 0f), // 4
            new Vector3(0f, -1f, 0f), // 5
        };

        int[] triangles = new int[]
        {
            3, 0, 1,
            3, 1, 2,
            4, 3, 0,
            4, 0, 1,
            4, 1, 2,
            4, 2, 3,
            3, 5, 0,
            0, 5, 1,
            1, 5, 2,
            2, 5, 3

        };

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
    }
}