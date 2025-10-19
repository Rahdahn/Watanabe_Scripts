using UnityEngine;
public class MixColorManager : MonoBehaviour
{
    [Header("色を変える先のミルクのイメージ")]
    [SerializeField] private SpriteRenderer _milk;
    [Header("色を変えるトッピング")]
    [SerializeField] private GameObject[] _colorTopping;
    [Header("混ぜた後の色のスプライト")]
    [SerializeField] private Sprite[] _changeColorImage;
    [Header("上に乗せるトッピング")]
    [SerializeField] private GameObject[] _onToping;
}
