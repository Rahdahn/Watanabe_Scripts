using UnityEngine;

public class ToppingDrag : MonoBehaviour
{
    [Header("�g�b�s���OID�i0-�j")]
    public int toppingIndex;

    [Header("�����g�b�s���O���H�ifalse = �F��ς���g�b�s���O�j")]
    public bool isDecoration = false;

    private Vector3 _startPos;
    private bool _isDragging;

    void Start()
    {
        _startPos = transform.position;
    }

    void OnMouseDown()
    {
        _isDragging = true;
    }

    void OnMouseDrag()
    {
        if (!_isDragging) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        transform.position = mousePos;
    }

    void OnMouseUp()
    {
        if (!_isDragging) return;
        _isDragging = false;

        Vector2 dropPos = transform.position;
        bool overMilk = MixColorManager.Instance.IsOverMilk(dropPos);

        if (overMilk)
        {
            bool added = MixColorManager.Instance.TryAddTopping(toppingIndex, isDecoration);
            Debug.Log(added ? $"�g�b�s���O {toppingIndex} �ǉ�����" : $"�g�b�s���O {toppingIndex} ���s");
        }

        // �����E���s�Ɋւ�炸���̈ʒu�ɖ߂�
        transform.position = _startPos;
    }
}
