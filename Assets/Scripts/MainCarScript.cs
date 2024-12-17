using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCarScript : MonoBehaviour
{
    Rigidbody2D rb;
    private Material carMaterial;
    private Transform sensorTransform; // sensorの座標

    public float obstacleDetectDistance = 1f; // 100cm
    public float carSpeed = 1f;
    public float rotationSpeed = 50000f; // 回転するスピード

    private float servoAngle = 0;
    private bool first_is = true;
    private bool isRunning = false;
    

    void Start()
    {
        Time.fixedDeltaTime = 0.01f; // 1フレームを0.01秒とする
        //Time.timeScale = 100f; // 100倍速で実行する
        Application.targetFrameRate = -1; // フレームレート制限を解除
        QualitySettings.vSyncCount = 0;   // V-Syncを無効化
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; //重力を0にする
        setCarMesh(); //車の見た目と当たり判定を定義
        sensorTransform = GetComponentInChildren<Transform>(); //センサーの位置を取得
    }

    void FixedUpdate()
    {
        if (!isRunning)
        {
            isRunning = true;
            StartCoroutine(ObstacleAvoidance());
        }
    }

    private IEnumerator ObstacleAvoidance()
    {
        if (first_is)
        {
            servoAngle = 0f;
            first_is = false;
            //yield return null; //1フレーム飛ばす
            yield return WaitForFixedFrames(1); // 1フレーム飛ばす
        }

        float distance = GetDistance(servoAngle, obstacleDetectDistance);

        if (distance <= obstacleDetectDistance) // 近いなら
        {
            yield return StartCoroutine(CarMotionControl("stop", 0)); //0frame待つ 意味ある?
            for (int i = -2; i <= 2; i += 2)
            {
                servoAngle = 30 * i; // -60, 0, 60
                yield return WaitForFixedFrames(45); //45frames, 450ms待つ

                yield return WaitForFixedFrames(10); //10frames, 100ms待つ

                distance = GetDistance(servoAngle, obstacleDetectDistance);

                if (distance <= obstacleDetectDistance) // 近いなら
                {
                    yield return StartCoroutine(CarMotionControl("stop", 1));
                    if (i == 2)
                    {
                        yield return StartCoroutine(CarMotionControl("backward", 50)); // 前進 0.5秒間実行
                        yield return StartCoroutine(CarMotionControl("right", 5)); // 右に回転 0.05秒間実行
                        first_is = true;
                    }
                }
                else
                {
                    if (i == -2)
                    {
                        yield return StartCoroutine(CarMotionControl("right", 5)); // 右に回転 0.05秒間実行
                    }
                    else if (i == 0)
                    {
                        yield return StartCoroutine(CarMotionControl("forward", 5)); // 前進 0.05秒間実行
                    }
                    else if (i == 2)
                    {
                        yield return StartCoroutine(CarMotionControl("left", 5)); // 左に回転 0.05秒間実行
                    }
                    first_is = true;
                    break;
                }
            }
        }
        else // 遠いなら
        {
            yield return StartCoroutine(CarMotionControl("forward", 1)); // 前進 0.01秒間実行
        }
        isRunning = false;
    }

    float GetDistance(float length, float dir)
    {
        int layerMask = ~LayerMask.GetMask("Ignore Raycast");

        // dirを基準に方向を計算（0度を正面として角度を回転）
        Vector2 rayDirection = Quaternion.Euler(0, 0, dir) * sensorTransform.up;
        RaycastHit2D hit = Physics2D.Raycast(sensorTransform.position, rayDirection, Mathf.Infinity, layerMask);
        // Rayを視覚化
        Debug.DrawRay(sensorTransform.position, rayDirection * length, Color.red);

        if (hit.collider != null)
        {
            return hit.distance;
        }
        return Mathf.Infinity;
    }

    IEnumerator CarMotionControl(string direction, int frameCount)
    {
        // 物理的に動き続ける処理と、frameCountの時間待つ処理を並列で実行
        switch (direction)
        {
            case "forward":
                rb.linearVelocity = transform.up * carSpeed;
                break;
            case "backward":
                rb.linearVelocity = -transform.up * carSpeed;
                break;
            case "right":
                rb.angularVelocity = rotationSpeed * Mathf.Deg2Rad;
                break;
            case "left":
                rb.angularVelocity = -rotationSpeed * Mathf.Deg2Rad;
                break;
            case "stop":
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                break;
        }

        // 指定されたframeCountだけ待機
        yield return WaitForFixedFrames(frameCount); //1frame = 10ms

        // 動きを停止
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    IEnumerator WaitForFixedFrames(int frameCount)
    {
        for (int i = 0; i < frameCount; i++)
        {
            yield return new WaitForFixedUpdate();
        }
    }

    void setCarMesh()
    {
        Mesh mesh = new Mesh();

        // 頂点を設定
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-0.1f, -0.15f, 0),
            new Vector3(0.1f, -0.15f, 0),
            new Vector3(-0.1f, 0.15f, 0),
            new Vector3(0.1f, 0.15f, 0)
        };
        mesh.vertices = vertices;

        // 面を設定
        int[] triangles = new int[6]
        {
            0, 2, 1,
            2, 3, 1
        };
        mesh.triangles = triangles;

        GetComponent<MeshFilter>().mesh = mesh;

        carMaterial = new Material(Shader.Find("Unlit/Color"));
        carMaterial.color = Color.white;

        GetComponent<MeshRenderer>().material = carMaterial;
    }
}