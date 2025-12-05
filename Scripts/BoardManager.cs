using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private GameObject[] _tilePrefab;
    [SerializeField] private float _spacing = 0.2f;

    [SerializeField] private float _fallTime = 0.25f;

    public GameObject[,] _board;
    public ScoreManager _scoreManager;
    public GameTimer _gameTimer;

    private bool _isFalingTiles = false;

    Tile _firstSelected;
    Tile _secondSelected;

    private void Start()
    {
        _board = new GameObject[_width, _height];
        GenerateBoard();
    }

    void GenerateBoard()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                SpawnTile(x, y);
            }
        }
    }

    void SpawnTile(int x, int y)
    {
        List<GameObject> possibleTiles = new List<GameObject>(_tilePrefab);

        if (x >= 2)
        {
            GameObject left1 = _board[x - 1, y];
            GameObject left2 = _board[x - 2, y];

            if (left1 != null && left2 != null)
            {
                if (left1.tag == left2.tag)
                {
                    possibleTiles.RemoveAll(p => p.tag == left1.tag);
                }
            }
        }

        if (y >= 2)
        {
            GameObject down1 = _board[x, y - 1];
            GameObject down2 = _board[x, y - 2];

            if (down1 != null && down2 != null)
            {
                if (down1.tag == down2.tag)
                {
                    possibleTiles.RemoveAll(p => p.tag == down1.tag);
                }
            }
        }

        if (possibleTiles.Count == 0)
        {
            possibleTiles = new List<GameObject>(_tilePrefab);
        }

        int randomIndex = Random.Range(0, possibleTiles.Count);
        GameObject spawnTile = possibleTiles[randomIndex];

        var spawnPos = GetWorldPos(x, y);

        GameObject tile = Instantiate(spawnTile, spawnPos, Quaternion.identity);

        tile.transform.parent = this.transform;

        Tile t = tile.AddComponent<Tile>();
        t.x = x;
        t.y = y;
        t._boardM = this;

        _board[x, y] = tile;
    }

    bool CheckMaches(out List<int> matchLength, out List<GameObject> matchedTiles)
    {
        matchLength = new List<int>();
        HashSet<GameObject> matchedSet = new HashSet<GameObject>();

        for (int y = 0; y < _height; y++)
        {
            int count = 1;
            for (int x = 1; x < _width; x++)
            {
                if (_board[x, y] != null &&
                    _board[x - 1, y] != null &&
                    _board[x, y].tag == _board[x - 1, y].tag)
                {
                    count++;
                }
                else
                {
                    if (count >= 3)
                    {
                        matchLength.Add(count);
                        for (int i = x - count; i < x; i++)
                        {
                            matchedSet.Add(_board[i, y]);
                        }
                    }
                    count = 1;
                }
            }

            if (count >= 3)
            {
                matchLength.Add(count);
                for (int i = _width - count; i < _width; i++)
                {
                    matchedSet.Add(_board[i, y]);
                }
            }
        }

        for (int x = 0; x < _width; x++)
        {
            int count = 1;
            for (int y = 1; y < _height; y++)
            {
                if (_board[x, y] != null &&
                    _board[x, y - 1] != null &&
                    _board[x, y].tag == _board[x, y - 1].tag)
                {
                    count++;
                }
                else
                {
                    if (count >= 3)
                    {
                        matchLength.Add(count);
                        for (int i = y - count; i < y; i++)
                        {
                            matchedSet.Add(_board[x, i]);
                        }
                    }
                    count = 1;
                }
            }
            if (count >= 3)
            {
                matchLength.Add(count);
                for (int i = _height - count; i < _height; i++)
                {
                    matchedSet.Add(_board[x, i]);
                }
            }
        }

        matchedTiles = new List<GameObject>(matchedSet);
        return matchedTiles.Count > 0;
    }

    public void SelectTile(Tile tile)
    {
        if (_firstSelected == null)
        {
            Debug.Log("一つ目のオブジェクトを選択");
            _firstSelected = tile;
            tile.HighlightTile(true);
            return;
        }
        if (_secondSelected == null)
        {
            Debug.Log("二つめのオブジェクトを選択");
            _secondSelected = tile;
            tile.HighlightTile(true);

            if (Nighbors(_firstSelected, _secondSelected))
            {
                StartCoroutine(SwapAndCheck(_firstSelected, _secondSelected));
            }

            _firstSelected.HighlightTile(false);
            _secondSelected.HighlightTile(false);

            _firstSelected = null;
            _secondSelected = null;
        }
    }

    bool Nighbors(Tile a, Tile b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);

        return (dx + dy == 1);
    }

    void SwapTilesData(Tile a, Tile b)
    {
        _board[a.x, a.y] = b.gameObject;
        _board[b.x, b.y] = a.gameObject;

        int tempX = a.x;
        int tempY = a.y;

        a.x = b.x;
        a.y = b.y;

        b.x = tempX;
        b.y = tempY;
    }

    IEnumerator SwapAndCheck(Tile a, Tile b)
    {
        if (_isFalingTiles) yield break;

        _gameTimer?.StartTimer();

        Vector3 posA = a.transform.position;
        Vector3 posB = b.transform.position;

        a.transform.DOMove(posB, 0.25f);
        b.transform.DOMove(posA, 0.25f);

        yield return new WaitForSeconds(0.25f);

        SwapTilesData(a, b);

        if (CheckMaches(out List<int> matchLength, out List<GameObject> machedTiles))
        {
            yield return StartCoroutine(ProcessMatches());
        }

        else
        {
            a.transform.DOMove(posA, 0.25f);
            b.transform.DOMove(posB, 0.25f);
            SwapTilesData(a, b);
        }
    }

    Vector2Int GetTileIndex(GameObject tile)
    {
        Tile t = tile.GetComponent<Tile>();
        return new Vector2Int(t.x, t.y);
    }

    #region 空いている部分を補完する
    IEnumerator CollapseBoard()
    {
        for (int x = 0; x < _width;  x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_board[x, y] == null)
                {
                    for (int k = y + 1;  k < _height; k++)
                    {
                          if (_board[x, k] != null)
                          {
                                GameObject tile = _board[x, k];
                                Tile t = tile.GetComponent<Tile>();

                                _board[x, y] = tile;
                                _board[x, k] = null;

                                t.y = y;

                                tile.transform.DOMove(GetWorldPos(x, y), _fallTime);

                                break;
                          }
                    }
                }
            }
        }

        yield return new WaitForSeconds(_fallTime);
    }

    IEnumerator RefillBoard()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_board[x, y] == null)
                {
                    GameObject prefab = _tilePrefab[Random.Range(0, _tilePrefab.Length)];
                    Vector3 spawnPos = GetWorldPos(x, _height) + Vector3.up * 1f;
                    GameObject tile = Instantiate(prefab, spawnPos, Quaternion.identity);

                    Tile t = tile.GetComponent<Tile>();
                    if (t == null)
                        t = tile.AddComponent<Tile>();
                    t.x = x; 
                    t.y = y;
                    t._boardM = this;

                    _board[x, y] = tile;

                    tile.transform.DOMove(GetWorldPos(x, y), _fallTime);
                   
                }
            }
        }
        yield return new WaitForSeconds(_fallTime + 0.2f);
    }

    IEnumerator ProcessMatches()
    {
        _isFalingTiles = true;

        while (true)
        {
            if (CheckMaches(out List<int> matchLength, out List<GameObject> matchedTiles) == false)
            {
                break;
            }

            if (_scoreManager != null)
            {
                foreach (int len in matchLength)
                {
                    _scoreManager.AddScore(len);
                }
            }

            foreach (var tile in matchedTiles)
            {
                Vector2Int index = GetTileIndex(tile);
                _board[index.x, index.y] = null;
                Destroy(tile);
            }

            yield return CollapseBoard();
            yield return RefillBoard();
        }

        _isFalingTiles = false;
    }
    #endregion

    Vector3 GetWorldPos(int x, int y)
    {
        float tileSize = 1f;
        float cellSize = tileSize + _spacing;

        float boardOffsetX = (_width - 1) / 2f;
        float boardOffsetY = _height / 2f;

        float worldX = (x - boardOffsetX) * cellSize;
        float worldY = (y - boardOffsetY) * cellSize;

        return new Vector3(worldX, worldY, 0);
    }
}