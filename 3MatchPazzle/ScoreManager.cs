using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _currentScoreText;
    [SerializeField] private GameObject _addScoreText;
    [SerializeField] private Transform _canvasTransform;
    [SerializeField] private float _addScoreDesplayTime;

    private int _currentScore = 0;
    
    public int Score { get; private set; }

    private void Start()
    {
        UpdateScoreText();
    }

    public void AddScore(int matchCount)
    {
        int points = 0;

        switch (matchCount)
        {
            case 3: points = 100;
                break;
            case 4: points = 200; 
                break;
            case 5: points = 500;
                break;
            default: if (matchCount > 5) points = (500 * (matchCount - 4) * (matchCount - 5)); 
                break; 
        }

        _currentScore += points;
        Score = _currentScore;

        UpdateScoreText();

        StartCoroutine(ShowAddScore(points));
    }

    void UpdateScoreText()
    {
        _currentScoreText.text = _currentScore.ToString();
    }

    IEnumerator ShowAddScore(int points)
    {
        GameObject adtx = Instantiate(_addScoreText, _canvasTransform);
        TMP_Text tmp = adtx.GetComponent<TMP_Text>();
        tmp.text = "+" + points;

        Vector3 startPos = adtx.transform.localPosition;
        adtx.transform.localPosition = startPos;

        yield return new WaitForSeconds(Random.Range(0f, 0.05f));

        adtx.transform.DOLocalMoveY(startPos.y + 50f, _addScoreDesplayTime).SetEase(Ease.OutCubic);
        tmp.DOFade(0f, _addScoreDesplayTime).SetEase(Ease.OutCubic);

        yield return new WaitForSeconds(_addScoreDesplayTime);

        Destroy(adtx);
    }
}