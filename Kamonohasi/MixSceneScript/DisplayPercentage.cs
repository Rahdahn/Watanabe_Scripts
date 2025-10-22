using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class DisplayPercentage : MonoBehaviour
{
    // TextMeshProコンポーネントへの参照
    public TextMeshProUGUI percentageText;

    public UnityEngine.UI.Image[] images;

    void Start()
    {
        // PlayerPrefsからパーセンテージを読み込む
        float percentage = PlayerPrefs.GetFloat("DropOrderPercentage", 0.0f);
        string order = PlayerPrefs.GetString("DropOrder", "");

        // パーセンテージをテキストに変換し、TextMeshProコンポーネントに設定
        percentageText.text = $"{percentage} ％\nカモノハシ！";

        // イメージを表示
        ShowImageByOrder(order);
    }

    void ShowImageByOrder(string order)
    {
        // 全てのイメージを非表示にする
        foreach (var image in images)
        {
            image.gameObject.SetActive(false);
        }

        // 表示されたイメージを保存するためのリスト
        List<string> displayedImages = new List<string>();

        // 順番に基づいて対応するイメージを表示
        switch (order)
        {
            case "D1,D5,D4,D3": images[0].gameObject.SetActive(true); displayedImages.Add("Image0"); break;
            case "D1,D5,D3,D4": images[1].gameObject.SetActive(true); displayedImages.Add("Image1"); break;
            case "D1,D3,D4,D5": images[2].gameObject.SetActive(true); displayedImages.Add("Image2"); break;
            case "D1,D3,D5,D4": images[3].gameObject.SetActive(true); displayedImages.Add("Image3"); break;
            case "D1,D4,D5,D3": images[4].gameObject.SetActive(true); displayedImages.Add("Image4"); break;
            case "D1,D4,D3,D5": images[5].gameObject.SetActive(true); displayedImages.Add("Image5"); break;
            case "D5,D1,D4,D3": images[6].gameObject.SetActive(true); displayedImages.Add("Image6"); break;
            case "D5,D1,D3,D4": images[7].gameObject.SetActive(true); displayedImages.Add("Image7"); break;
            case "D5,D4,D1,D3": images[8].gameObject.SetActive(true); displayedImages.Add("Image8"); break;
            case "D5,D4,D3,D1": images[9].gameObject.SetActive(true); displayedImages.Add("Image9"); break;
            case "D5,D3,D1,D4": images[10].gameObject.SetActive(true); displayedImages.Add("Image10"); break;
            case "D5,D3,D4,D1": images[11].gameObject.SetActive(true); displayedImages.Add("Image11"); break;
            case "D4,D5,D3,D1": images[12].gameObject.SetActive(true); displayedImages.Add("Image12"); break;
            case "D4,D5,D1,D3": images[13].gameObject.SetActive(true); displayedImages.Add("Image13"); break;
            case "D4,D1,D3,D5": images[14].gameObject.SetActive(true); displayedImages.Add("Image14"); break;
            case "D4,D1,D5,D3": images[15].gameObject.SetActive(true); displayedImages.Add("Image15"); break;
            case "D4,D3,D1,D5": images[16].gameObject.SetActive(true); displayedImages.Add("Image16"); break;
            case "D4,D3,D5,D1": images[17].gameObject.SetActive(true); displayedImages.Add("Image17"); break;
            case "D3,D1,D4,D5": images[18].gameObject.SetActive(true); displayedImages.Add("Image18"); break;
            case "D3,D1,D5,D4": images[19].gameObject.SetActive(true); displayedImages.Add("Image19"); break;
            case "D3,D4,D1,D5": images[20].gameObject.SetActive(true); displayedImages.Add("Image20"); break;
            case "D3,D4,D5,D1": images[21].gameObject.SetActive(true); displayedImages.Add("Image21"); break;
            case "D3,D5,D4,D1": images[22].gameObject.SetActive(true); displayedImages.Add("Image22"); break;
            case "D3,D5,D1,D4": images[23].gameObject.SetActive(true); displayedImages.Add("Image23"); break;
            case "D4,D1,D2,D5": images[24].gameObject.SetActive(true); displayedImages.Add("Image24"); break;
            case "D2,D1,D5,D4": images[25].gameObject.SetActive(true); displayedImages.Add("Image25"); break;
            case "D1,D2,D5,D4": images[26].gameObject.SetActive(true); displayedImages.Add("Image26"); break;
            case "D1,D5,D2,D4": images[27].gameObject.SetActive(true); displayedImages.Add("Image27"); break;
            case "D5,D4,D1,D2": images[28].gameObject.SetActive(true); displayedImages.Add("Image28"); break;
            case "D5,D2,D4,D1": images[29].gameObject.SetActive(true); displayedImages.Add("Image29"); break;
            default: images[30].gameObject.SetActive(true); displayedImages.Add("ImageDefault"); break;
        }

    }
}
