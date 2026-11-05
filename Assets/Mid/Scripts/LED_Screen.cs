using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
public class LED_Screen : MonoBehaviour
{
    public int textureWidth = 320;
    public int textureHeight = 180;

    private Texture2D screenTexture;
    private MaterialPropertyBlock propertyBlock;

    void OnEnable()
    {
        DrawLED();
    }

    void OnValidate()
    {
        if (isActiveAndEnabled)
            DrawLED();
    }

    void OnDisable()
    {
        if (screenTexture != null)
        {
            if (Application.isPlaying)
                Destroy(screenTexture);
            else
                DestroyImmediate(screenTexture);

            screenTexture = null;
        }
    }

    void DrawLED()
    {
        if (textureWidth < 32 || textureHeight < 32)
            return;

        if (screenTexture == null ||
            screenTexture.width != textureWidth ||
            screenTexture.height != textureHeight)
        {
            if (screenTexture != null)
            {
                if (Application.isPlaying)
                    Destroy(screenTexture);
                else
                    DestroyImmediate(screenTexture);
            }

            screenTexture = new Texture2D(
                textureWidth,
                textureHeight,
                TextureFormat.RGBA32,
                false
            );

            screenTexture.name = "Concert LED Pixels";
            screenTexture.filterMode = FilterMode.Point;
            screenTexture.wrapMode = TextureWrapMode.Clamp;
            screenTexture.hideFlags = HideFlags.HideAndDontSave;
        }

        // 1. 어두운 공연장 배경을 픽셀 단위로 생성
        for (int y = 0; y < textureHeight; y++)
        {
            for (int x = 0; x < textureWidth; x++)
            {
                float t = (float)y / textureHeight;

                Color bg = Color.Lerp(
                    new Color(0.015f, 0.005f, 0.06f),
                    new Color(0.02f, 0.04f, 0.16f),
                    t
                );

                screenTexture.SetPixel(x, y, bg);
            }
        }

        // 2. 공연 조명 형태의 컬러 삼각형
        FillTriangle(
            new Vector2Int(0, 0),
            new Vector2Int(125, 0),
            new Vector2Int(0, 100),
            new Color(0.1f, 0.02f, 0.5f)
        );

        FillTriangle(
            new Vector2Int(textureWidth, 0),
            new Vector2Int(200, 0),
            new Vector2Int(textureWidth, 120),
            new Color(0.5f, 0.02f, 0.3f)
        );

        FillTriangle(
            new Vector2Int(80, textureHeight),
            new Vector2Int(180, textureHeight),
            new Vector2Int(150, 100),
            new Color(0.02f, 0.15f, 0.5f)
        );

        // 3. 이퀄라이저 막대 직접 생성
        int barCount = 30;
        int barWidth = 6;
        int spacing = 4;

        for (int i = 0; i < barCount; i++)
        {
            float wave = Mathf.Abs(
                Mathf.Sin(i * 0.45f) * Mathf.Cos(i * 0.19f)
            );

            int barHeight = Mathf.RoundToInt(25 + wave * 75);

            Color barColor = Color.Lerp(
                new Color(0.0f, 0.8f, 1.0f),
                new Color(1.0f, 0.05f, 0.65f),
                (float)i / barCount
            );

            int x = 15 + i * (barWidth + spacing);

            FillRectangle(
                x,
                15,
                barWidth,
                barHeight,
                barColor
            );
        }

        // 4. 음악 파형을 픽셀 선으로 그리기
        Vector2Int previous = Vector2Int.zero;

        for (int x = 0; x < textureWidth; x++)
        {
            float y = 115
                + Mathf.Sin(x * 0.045f) * 15
                + Mathf.Sin(x * 0.12f) * 7;

            Vector2Int current = new Vector2Int(
                x,
                Mathf.RoundToInt(y)
            );

            if (x > 0)
            {
                DrawPixelLine(
                    previous,
                    current,
                    new Color(0.1f, 1.0f, 0.9f)
                );
            }

            previous = current;
        }

        // 5. 화면에 작은 LED 점 패턴 추가
        for (int y = 5; y < textureHeight; y += 8)
        {
            for (int x = 5; x < textureWidth; x += 8)
            {
                screenTexture.SetPixel(
                    x, y,
                    new Color(0.3f, 0.5f, 1.0f)
                );
            }
        }

        screenTexture.Apply();

        // 6. 전광판 재질에 텍스처 적용
        MeshRenderer meshRenderer =
            GetComponent<MeshRenderer>();

        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        meshRenderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetTexture(
            "_BaseMap",
            screenTexture
        );

        propertyBlock.SetTexture(
            "_MainTex",
            screenTexture
        );

        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    // 외적을 이용해 삼각형 내부 픽셀 판정
    void FillTriangle(
        Vector2Int a,
        Vector2Int b,
        Vector2Int c,
        Color color
    )
    {
        int minX = Mathf.Max(
            0, Mathf.Min(a.x, b.x, c.x)
        );

        int maxX = Mathf.Min(
            textureWidth - 1,
            Mathf.Max(a.x, b.x, c.x)
        );

        int minY = Mathf.Max(
            0, Mathf.Min(a.y, b.y, c.y)
        );

        int maxY = Mathf.Min(
            textureHeight - 1,
            Mathf.Max(a.y, b.y, c.y)
        );

        for (int y = minY; y <= maxY; y++)
        {
            for (int x = minX; x <= maxX; x++)
            {
                Vector2 p = new Vector2(x, y);

                float d1 = Cross(b - a, p - a);
                float d2 = Cross(c - b, p - b);
                float d3 = Cross(a - c, p - c);

                bool negative =
                    d1 < 0 || d2 < 0 || d3 < 0;

                bool positive =
                    d1 > 0 || d2 > 0 || d3 > 0;

                if (!(negative && positive))
                    screenTexture.SetPixel(x, y, color);
            }
        }
    }

    float Cross(Vector2 a, Vector2 b)
    {
        return a.x * b.y - a.y * b.x;
    }

    // 사각형 영역의 픽셀을 직접 채우기
    void FillRectangle(
        int startX,
        int startY,
        int width,
        int height,
        Color color
    )
    {
        for (int y = startY; y < startY + height; y++)
        {
            for (int x = startX; x < startX + width; x++)
            {
                if (x >= 0 && x < textureWidth &&
                    y >= 0 && y < textureHeight)
                {
                    screenTexture.SetPixel(x, y, color);
                }
            }
        }
    }

    // Bresenham 방식으로 픽셀 선 그리기
    void DrawPixelLine(
        Vector2Int a,
        Vector2Int b,
        Color color
    )
    {
        int dx = Mathf.Abs(b.x - a.x);
        int dy = Mathf.Abs(b.y - a.y);

        int sx = a.x < b.x ? 1 : -1;
        int sy = a.y < b.y ? 1 : -1;

        int error = dx - dy;

        while (true)
        {
            if (a.x >= 0 && a.x < textureWidth &&
                a.y >= 0 && a.y < textureHeight)
            {
                screenTexture.SetPixel(a.x, a.y, color);
            }

            if (a.x == b.x && a.y == b.y)
                break;

            int e2 = 2 * error;

            if (e2 > -dy)
            {
                error -= dy;
                a.x += sx;
            }

            if (e2 < dx)
            {
                error += dx;
                a.y += sy;
            }
        }
    }
}
