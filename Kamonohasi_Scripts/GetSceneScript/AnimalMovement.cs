using System;
using UnityEngine;

public class AnimalMovement : MonoBehaviour
{
    public float moveSpeed = 2f;           // 動物の移動速度
    public float changeDirectionTime = 2f; // 方向を変えるまでの時間

    private Vector2 targetPosition;        // 動物が移動する目標地点
    private float timer;                   // 方向変更のタイマー
    private bool isMoving = true;          // 移動状態を制御するフラグ

    private Vector2 minBounds;             // 元のカメラの左下のワールド座標
    private Vector2 maxBounds;             // 元のカメラの右上のワールド座標

    private void Start()
    {
        // ズーム前のカメラ範囲を計算して保存
        SetInitialCameraBounds();

        // 初期の目標地点を設定
        SetRandomTargetPosition();
    }

    private void Update()
    {
        if (isMoving)
        {
            // タイマーを更新し、一定時間経過後に方向を変える
            timer += Time.deltaTime;
            if (timer >= changeDirectionTime)
            {
                SetRandomTargetPosition();
                timer = 0;
            }

            // 動物を目標地点に向かって移動させる
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // 目標地点に到達したら、新しい目標地点を設定する
            if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
            {
                SetRandomTargetPosition();
            }
        }
    }

    // 元のカメラ範囲を取得するメソッド
    private void SetInitialCameraBounds()
    {
        Camera cam = Camera.main;

        // ズーム前のカメラのビューポートサイズを取得して保存
        minBounds = cam.ViewportToWorldPoint(new Vector2(0.1f, 0.1f)); // 左下のワールド座標
        maxBounds = cam.ViewportToWorldPoint(new Vector2(0.9f, 0.9f)); // 右上のワールド座標
    }

    // ランダムな目標地点を設定するメソッド
    private void SetRandomTargetPosition()
    {
        // 元のカメラ範囲内でランダムな目標地点を設定
        targetPosition = new Vector2(
            UnityEngine.Random.Range(minBounds.x, maxBounds.x),
            UnityEngine.Random.Range(minBounds.y, maxBounds.y)
        );
    }

    // 移動を停止するメソッド
    public void StopMoving()
    {
        isMoving = false;
    }

    // 移動を再開するメソッド
    public void StartMoving()
    {
        isMoving = true;
    }
}
