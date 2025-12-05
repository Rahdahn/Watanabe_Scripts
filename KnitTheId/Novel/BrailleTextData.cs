using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBrailleTextData", menuName = "Braille/NumberData")]
public class BrailleTextData : ScriptableObject
{
    [Header("Prefab番号リスト (1-48)")]
    public List<int> prefabNumbers;
}
