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
        };

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
    }
}