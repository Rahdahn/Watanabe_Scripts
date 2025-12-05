using UnityEngine;

public class ColoredObject : MonoBehaviour
{
    public enum ObjectColor
    {
        White,
        Blue,
        Yellow,
        Black
    }

    [Header("このオブジェクトの色タイプ")]
    public ObjectColor colorType;
}
