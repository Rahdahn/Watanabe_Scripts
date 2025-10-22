using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetGameManager : MonoBehaviour
{
    public GameObject[] animalPrefabs;
    public int numberOfAnimals = 5;
    public Vector2 spawnAreaMin = new Vector2(-5, -5);
    public Vector2 spawnAreaMax = new Vector2(5, 5);

    public ClickZoom clickZoomScript;
    public GaugeMiniGame gaugeMiniGameScript;
    public GameObject backgroundOverlay;
    public TextMeshProUGUI successText; // 捕獲成功のテキスト
    public TextMeshProUGUI failText;    // 捕獲失敗のテキスト
    public TextMeshProUGUI timerText;   // タイマー表示用のTextMeshPro
    public Canvas gameOverCanvas;       // 時間切れ時に表示するCanvas

    // 動物の種類ごとのカウントを表示するTextMeshPro
    public TextMeshProUGUI d1CountText;
    public TextMeshProUGUI d2CountText;
    public TextMeshProUGUI d3CountText;
    public TextMeshProUGUI d4CountText;
    public TextMeshProUGUI d5CountText;

    private List<Animal> capturedAnimals = new List<Animal>();
    private Animal currentAnimal;
    private int originalAnimalSortingOrder; // 動物の元のOrder in Layerを保持する変数
    private float timeRemaining = 60f;      // タイマーの初期値
    private bool timerIsRunning = true;     // タイマーが動作中かどうかのフラグ
    private bool isClickable = true;         // クリック可能かどうかのフラグ

    private List<GameObject> spawnedAnimals = new List<GameObject>(); // スポーンした動物を追跡するリスト

    private void Start()
    {
        SpawnAnimals(numberOfAnimals); // 初期スポーンの数に変更
        if (backgroundOverlay != null)
        {
            backgroundOverlay.SetActive(false); // 初期状態でオーバーレイを非表示
        }
        if (successText != null)
        {
            successText.gameObject.SetActive(false); // 初期状態で成功テキストを非表示
        }
        if (failText != null)
        {
            failText.gameObject.SetActive(false); // 初期状態で失敗テキストを非表示
        }
        if (gameOverCanvas != null)
        {
            gameOverCanvas.gameObject.SetActive(false); // 初期状態でCanvasを非表示
        }

        // 捕獲した動物のカウントを初期化
        UpdateAnimalCountDisplay();
    }

    private void Update()
    {
        if (timerIsRunning && isClickable) // クリック可能な場合のみ処理
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerDisplay(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                timerIsRunning = false;
                OnTimeExpired();
            }
            if (Input.GetMouseButtonDown(0) && currentAnimal == null)
            {
                Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(clickPos, Vector2.zero);

                if (hit.collider != null)
                {
                    Animal animal = hit.collider.GetComponent<Animal>();
                    if (animal != null)
                    {
                        currentAnimal = animal;
                        currentAnimal.StopMoving();  // 動物の動きを止める
                        clickZoomScript.StartZoom(animal.transform.position);

                        // オーバーレイと動物のOrder in Layerを調整
                        var spriteRenderer = backgroundOverlay.GetComponent<SpriteRenderer>();
                        if (spriteRenderer != null)
                        {
                            spriteRenderer.sortingOrder = 90;
                        }
                        var animalRenderer = animal.GetComponent<SpriteRenderer>();
                        if (animalRenderer != null)
                        {
                            originalAnimalSortingOrder = animalRenderer.sortingOrder; // 元のOrder in Layerを保存
                            animalRenderer.sortingOrder = 100;
                        }

                        if (backgroundOverlay != null)
                        {
                            backgroundOverlay.SetActive(true); // オーバーレイを表示
                        }
                    }
                }
            }
        }
    }

    private void UpdateTimerDisplay(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    private void OnTimeExpired()
    {
        // タイマーが0になった時の処理
        UnityEngine.Debug.Log("Time's up!");

        // 自身のスクリプトを停止する
        this.enabled = false;

        // タイマーを非表示
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }

        // ゲームオーバーCanvasを表示
        if (gameOverCanvas != null)
        {
            gameOverCanvas.gameObject.SetActive(true);
        }

        // 全ての動物を消去
        foreach (var animal in spawnedAnimals)
        {
            if (animal != null)
            {
                Destroy(animal);
            }
        }
        spawnedAnimals.Clear();

    }



    public void OnZoomComplete()
    {
        isClickable = false; // ミニゲーム中はクリックを無効化
        gaugeMiniGameScript.StartMiniGame(OnMiniGameComplete);
    }

    private void OnMiniGameComplete(bool isSuccess)
    {
        StartCoroutine(HandleMiniGameResult(isSuccess));
    }

    private IEnumerator HandleMiniGameResult(bool isSuccess)
    {
        if (isSuccess)
        {
            if (successText != null)
            {
                successText.gameObject.SetActive(true); // 成功テキストを表示
            }
            if (currentAnimal != null)
            {
                CaptureAnimal(currentAnimal);
            }
        }
        else
        {
            if (failText != null)
            {
                failText.gameObject.SetActive(true); // 失敗テキストを表示
            }
            // 動物が動き出すように設定し、Order in Layerを元に戻す
            if (currentAnimal != null)
            {
                var animalRenderer = currentAnimal.GetComponent<SpriteRenderer>();
                if (animalRenderer != null)
                {
                    animalRenderer.sortingOrder = originalAnimalSortingOrder; // 元のOrder in Layerに戻す
                }
                currentAnimal.StartMoving();
            }
        }

        yield return new WaitForSeconds(1f); // 1秒間待つ

        // オーバーレイとテキストを非表示にする
        if (backgroundOverlay != null)
        {
            backgroundOverlay.SetActive(false); // オーバーレイを非表示
        }
        if (successText != null)
        {
            successText.gameObject.SetActive(false); // 成功テキストを非表示
        }
        if (failText != null)
        {
            failText.gameObject.SetActive(false); // 失敗テキストを非表示
        }

        clickZoomScript.ResetZoom();
        currentAnimal = null;

        isClickable = true; // クリックを再度有効化
        yield return new WaitForSeconds(5f);
        SpawnAnimals(1); // 新しい動物を1体スポーン
    }

    private void CaptureAnimal(Animal animal)
    {
        capturedAnimals.Add(animal);
        UnityEngine.Debug.Log($"Captured Animal: {animal.type}");
        Destroy(animal.gameObject);
        spawnedAnimals.Remove(animal.gameObject); // リストから削除

        // 捕獲した動物の種類ごとのカウントを更新
        switch (animal.type)
        {
            case AnimalType.D1:
                GameData.D1Count++;
                break;
            case AnimalType.D2:
                GameData.D2Count++;
                break;
            case AnimalType.D3:
                GameData.D3Count++;
                break;
            case AnimalType.D4:
                GameData.D4Count++;
                break;
            case AnimalType.D5:
                GameData.D5Count++;
                break;
        }
        UpdateAnimalCountDisplay();
    }

    private void UpdateAnimalCountDisplay()
    {
        // 各種類のカウントをTextMeshProに反映
        d1CountText.text = $"X {GameData.D1Count}";
        d2CountText.text = $"X {GameData.D2Count}";
        d3CountText.text = $"X {GameData.D3Count}";
        d4CountText.text = $"X {GameData.D4Count}";
        d5CountText.text = $"X {GameData.D5Count}";
    }

    private void SpawnAnimals(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int prefabIndex = UnityEngine.Random.Range(0, animalPrefabs.Length);
            GameObject prefab = animalPrefabs[prefabIndex];
            Vector2 spawnPos = new Vector2(
                UnityEngine.Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                UnityEngine.Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            GameObject animal = Instantiate(prefab, spawnPos, Quaternion.identity);
            spawnedAnimals.Add(animal); // スポーンした動物をリストに追加
        }
    }
}
