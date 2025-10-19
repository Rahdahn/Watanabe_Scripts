using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LibraryManager : MonoBehaviour
{
    [System.Serializable]
    public class LibraryObject
    {
        public string Name;
        public string Par;
        public Sprite Sprite;
        public string Memo;
    }

    [SerializeField]
    private Image _image;

    [SerializeField]
    private TMPro.TextMeshProUGUI _textMeshName;

    [SerializeField]
    private TMPro.TextMeshProUGUI _textMeshPar;

    [SerializeField]
    private TMPro.TextMeshProUGUI _textMeshMemo;

    [SerializeField]
    List<LibraryObject> _listlibraryObjects = new List<LibraryObject>();

    private int _index = 0;

    public void Start()
    {
        if (_listlibraryObjects.Count > 0)
        {
            _textMeshName.text = _listlibraryObjects[_index].Name;
            _textMeshPar.text = _listlibraryObjects[_index].Par;
            _textMeshMemo.text = _listlibraryObjects[_index].Memo;
            _image.sprite = _listlibraryObjects[_index].Sprite;
        }
    }

    public void Left()
    {
        _index = (_index + _listlibraryObjects.Count - 1) % _listlibraryObjects.Count;
        ChangeObject(_index);
    }

    public void Right()
    {
        _index = (_index + _listlibraryObjects.Count + 1) % _listlibraryObjects.Count;
        ChangeObject(_index);
    }

    private void ChangeObject(int index)
    {
        _textMeshName.text = _listlibraryObjects[index].Name;
        _textMeshPar.text = _listlibraryObjects[index].Par;
        _textMeshMemo.text = _listlibraryObjects[index].Memo;
        _image.sprite = _listlibraryObjects[index].Sprite;
    }
}
