
using UnityEngine;

public class SeatGenerator : MonoBehaviour
{
    public GameObject chairPrefab;

    public int rows = 5;
    public int seatsPerSide = 6;

    public float seatSpacing = 0.85f;
    public float rowSpacing = 1.15f;

    public float aisleWidth = 1.8f;

    [ContextMenu("Generate Audience")]
    public void GenerateAudience()
    {
        if (chairPrefab == null)
        {
            Debug.LogError("Chair Prefab을 연결해 주세요.");
            return;
        }

        // 기존 객석 제거
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            GameObject oldChair = transform.GetChild(i).gameObject;

            if (Application.isPlaying)
                Destroy(oldChair);
            else
                DestroyImmediate(oldChair);
        }

        // 좌우 객석 생성
        for (int row = 0; row < rows; row++)
        {
            for (int side = -1; side <= 1; side += 2)
            {
                for (int seat = 0; seat < seatsPerSide; seat++)
                {
                    float x = side * (aisleWidth / 2f
                        + seatSpacing / 2f
                        + seat * seatSpacing);

                    float z = -2f - row * rowSpacing;

                    Vector3 position = new Vector3(x, 0, z);

                    GameObject chair = Instantiate(
                        chairPrefab,
                        transform
                    );

                    chair.transform.localPosition = position;
                    chair.transform.localRotation =
                        Quaternion.identity;

                    chair.name =
                        $"Chair_R{row + 1}_S{side}_{seat + 1}";
                }
            }
        }
    }
}
