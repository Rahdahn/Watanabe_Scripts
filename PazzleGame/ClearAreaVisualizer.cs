using UnityEngine;

public class ClearAreaVisualizer : MonoBehaviour
{
    [SerializeField] private float cubeSize = 6f;
    [SerializeField] private Color gizmoColor = new Color(0f, 1f, 0f, 0.25f);

    private void OnDrawGizmos()
    {
        // YÇ+3ÇµÇƒÅAâ∫ï”Ç™ínñ Ç∆àÍívÇ∑ÇÈÇÊÇ§Ç…í≤êÆ
        Vector3 center = new Vector3(0f, cubeSize / 2f, 0f);

        Gizmos.color = gizmoColor;
        Vector3 size = Vector3.one * cubeSize;

        Gizmos.DrawWireCube(center, size);
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
        Gizmos.DrawCube(center, size);
    }
}
