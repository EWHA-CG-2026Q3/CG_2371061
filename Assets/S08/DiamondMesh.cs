using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DiamondMesh : MonoBehaviour
{
    [Header("다이아몬드 원본 정점 (0=아래 꼭짓점, 1~4=중간 사각형, 5=위 꼭짓점)")]
    [SerializeField]
    Vector3[] baseVertices = new Vector3[]
    {
        new Vector3(0.5f, 0f,   0.5f),
        new Vector3(0f,   0.5f, 0f), 
        new Vector3(1f,   0.5f, 0f), 
        new Vector3(1f,   0.5f, 1f), 
        new Vector3(0f,   0.5f, 1f), 
        new Vector3(0.5f, 1f,   0.5f),
    };

    static readonly int[] triangles = new int[]
    {
        0,2,1, 0,3,2, 0,4,3, 0,1,4,  
        5,1,2, 5,2,3, 5,3,4, 5,4,1,  
    };

    Mesh mesh;

    public Vector3[] BaseVertices => baseVertices;

    void OnEnable()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        SetVertices(baseVertices);
    }

    public void SetVertices(Vector3[] verts)
    {
        mesh.Clear();
        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}