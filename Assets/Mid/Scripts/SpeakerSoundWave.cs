
using UnityEngine;

public class SpeakerSoundWave : MonoBehaviour
{
    [Header("Woofer Objects")]
    public Transform[] woofers;

    [Header("Sound Wave Origins")]
    public Transform[] waveOrigins;

    [Header("Woofer Vibration")]
    public float vibrationSpeed = 12f;
    public float vibrationAmount = 0.06f;

    [Header("Sound Wave")]
    public int waveCount = 3;
    public float waveDuration = 1.5f;
    public float maxRadius = 1.8f;
    public float waveSpeed = 2f;
    public float lineWidth = 0.035f;

    private Vector3[] initialScales;
    private Vector3[] initialPositions;

    private LineRenderer[,] waveLines;
    private Material waveMaterial;

    private const int Segments = 48;

    void Start()
    {
        // 우퍼의 처음 크기와 위치 저장
        initialScales = new Vector3[woofers.Length];
        initialPositions = new Vector3[woofers.Length];

        for (int i = 0; i < woofers.Length; i++)
        {
            if (woofers[i] == null)
                continue;

            initialScales[i] = woofers[i].localScale;
            initialPositions[i] = woofers[i].localPosition;
        }

        // 음파용 재질 생성
        Shader shader = Shader.Find("Sprites/Default");

        if (shader == null)
            shader = Shader.Find("Universal Render Pipeline/Unlit");

        waveMaterial = new Material(shader);
        waveMaterial.color = Color.white;

        // 음파 링 생성
        waveLines = new LineRenderer[waveOrigins.Length, waveCount];

        for (int i = 0; i < waveOrigins.Length; i++)
        {
            if (waveOrigins[i] == null)
                continue;

            for (int j = 0; j < waveCount; j++)
            {
                GameObject ring = new GameObject(
                    "SoundWave_" + i + "_" + j
                );

                ring.transform.SetParent(transform);

                LineRenderer line = ring.AddComponent<LineRenderer>();

                line.useWorldSpace = true;
                line.loop = true;
                line.positionCount = Segments;

                line.startWidth = lineWidth;
                line.endWidth = lineWidth;

                line.material = waveMaterial;
                line.startColor = new Color(1f, 1f, 1f, 0.35f);
                line.endColor = new Color(1f, 1f, 1f, 0.35f);
                line.numCapVertices = 4;
                line.numCornerVertices = 4;

                waveLines[i, j] = line;
            }
        }
    }

    void Update()
    {
        AnimateWoofers();
        AnimateSoundWaves();
    }

    // 우퍼가 소리에 맞춰 앞뒤로 진동
    void AnimateWoofers()
    {
        for (int i = 0; i < woofers.Length; i++)
        {
            if (woofers[i] == null)
                continue;

            float pulse = Mathf.Sin(
                Time.time * vibrationSpeed + i
            );

            Vector3 scale = initialScales[i];

            // 스피커 앞뒤 방향으로 미세하게 팽창
            scale.z *= 1f + Mathf.Abs(pulse) * vibrationAmount;

            woofers[i].localScale = scale;

            Vector3 position = initialPositions[i];
            position.z += pulse * vibrationAmount * 0.5f;

            woofers[i].localPosition = position;
        }
    }

    // 스피커 앞쪽으로 퍼지는 원형 음파    
    void AnimateSoundWaves()
    {
        for (int i = 0; i < waveOrigins.Length; i++)
        {
            Transform origin = waveOrigins[i];

            if (origin == null)
                continue;

            for (int j = 0; j < waveCount; j++)
            {
                LineRenderer line = waveLines[i, j];

                if (line == null)
                    continue;

                float cycle = Mathf.Repeat(
                    Time.time * waveSpeed +
                    (float)j / waveCount,
                    1f
                );

                float radius = Mathf.Lerp(
                    0.15f,
                    maxRadius,
                    cycle
                );

                float alpha = Mathf.Pow(1f - cycle, 1.5f) * 0.35f;

                Color waveColor = new Color(
                    1f, 1f, 1f, alpha
                );

                line.startColor = waveColor;
                line.endColor = waveColor;

                // 퍼질수록 약간 두꺼워지는 효과
                float thickness = Mathf.Lerp(
                    0.08f,
                    0.18f,
                    cycle
                );

                line.startWidth = thickness;
                line.endWidth = thickness;

                Vector3 center =
                    origin.position +
                    origin.forward * radius * 0.35f;

                Vector3 right = origin.right;
                Vector3 up = origin.up;

                for (int k = 0; k < Segments; k++)
                {
                    float angle =
                        2f * Mathf.PI * k / Segments;

                    // 원형을 약간 불규칙하게 만들어 부드러운 느낌
                    float irregularity =
                        1f
                        + Mathf.Sin(angle * 5f + Time.time * 1.5f) * 0.04f
                        + Mathf.Sin(angle * 9f - Time.time) * 0.025f;

                    float r = radius * irregularity;

                    Vector3 offset =
                        right * Mathf.Cos(angle) * r +
                        up * Mathf.Sin(angle) * r;

                    line.SetPosition(k, center + offset);
                }

                line.enabled = true;
            }
        }
    }


    void OnDestroy()
    {
        if (waveMaterial != null)
            Destroy(waveMaterial);
    }
}
