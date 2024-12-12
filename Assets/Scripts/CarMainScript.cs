using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMainScript : MonoBehaviour
{
    public float rotationSpeed = 90f; // 回転速度
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // 重力を0にする
        GetComponent<Rigidbody2D>().gravityScale = 0f;

        // SpriteRendererコンポーネントを取得
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRendererが見つかりません！");
        }
        spriteRenderer.color = Color.white;
    }


    void Update()
    {
        // オブジェクトを回転
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 衝突時に色を赤に変更
        spriteRenderer.color = Color.red;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 離れたときに色を白に変更
        spriteRenderer.color = Color.white;
    }
}
