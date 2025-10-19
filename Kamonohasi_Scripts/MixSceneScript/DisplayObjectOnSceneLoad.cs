using UnityEngine;

public class DisplayObjectOnSceneLoad : MonoBehaviour
{
    public GameObject objectToShow; // 表示したいオブジェクト

    private void Start()
    {
        // ゲームデータの合計カウントをチェック
        int totalCount = GameData.D1Count + GameData.D2Count + GameData.D3Count + GameData.D4Count + GameData.D5Count;

        // 合計が4以下の場合はオブジェクトを表示
        if (totalCount < 4)
        {
            ShowObject();
        }
        else
        {
            HideObject();
        }
    }

    private void ShowObject()
    {
        if (objectToShow != null)
        {
            objectToShow.SetActive(true); // オブジェクトを表示
        }
    }

    private void HideObject()
    {
        if (objectToShow != null)
        {
            objectToShow.SetActive(false); // オブジェクトを非表示
        }
    }
}
