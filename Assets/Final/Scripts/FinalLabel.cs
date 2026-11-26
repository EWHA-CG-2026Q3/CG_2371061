using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif

// 채점 대상 오브젝트 위에 이름표를 띄움
// Game 뷰: Play 중 L 키로 켜고 끔
// Scene 뷰: Scene 뷰 위쪽의 Gizmos 버튼으로 켜고 끔
public class FinalLabel : MonoBehaviour
{
    [SerializeField] string label = "코드 메시: 별 기둥";
    [SerializeField] Vector3 offset = new Vector3(0f, 1.5f, 0f);

    static bool show = true;          // Game 뷰 이름표: 모든 이름표가 함께 씀
    static int lastToggleFrame = -1;  // 한 프레임에 한 번만 바뀌도록

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.lKey.wasPressedThisFrame
            && lastToggleFrame != Time.frameCount)
        {
            show = !show;
            lastToggleFrame = Time.frameCount;
        }
    }

    // Game 뷰 (Play 중)
    void OnGUI()
    {
        if (!show) return;
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 p = cam.WorldToScreenPoint(transform.position + offset);
        if (p.z < 0f) return;  // 카메라 뒤에 있으면 그리지 않음

        GUIStyle style = MakeStyle(GUI.skin.label);
        Vector2 size = style.CalcSize(new GUIContent(label));
        float y = Screen.height - p.y;  // 화면 좌표와 GUI 좌표는 y 방향이 반대
        GUI.Label(new Rect(p.x - size.x / 2f, y - size.y / 2f, size.x, size.y), label, style);
    }

#if UNITY_EDITOR
    // Scene 뷰 (Play 중이 아니어도 보임)
    void OnDrawGizmos()
    {
        GUIStyle style = MakeStyle(EditorStyles.boldLabel);
        Handles.Label(transform.position + offset, label, style);
    }
#endif

    static GUIStyle MakeStyle(GUIStyle baseStyle)
    {
        GUIStyle style = new GUIStyle(baseStyle);
        style.fontSize = 18;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.yellow;
        style.alignment = TextAnchor.MiddleCenter;
        return style;
    }
}