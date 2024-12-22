using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class MainCarScript : MonoBehaviour
{
    private Rigidbody2D rb;
    private Transform carTransform;
    private Transform sensorTransform;
    private SensorScript sensorScript;
    private string filePath;
    public float timeScale;

    // 乱数で決める変数
    public float obstacleDetectDistance;
    public float carLinearSpeed;
    public float carTurningSpeed;
    public float servoRange;

    private float carLinearSpeedCorrectionValue = 0.0032f;
    private float carLinearSpeedCorrected;
    private float carTurningSpeedCorrectionValue = 104.988364323403f;
    private float carTurningSpeedCorrected;

    private int carStockCount; // 車がストックしたときのカウント
    private Vector3 carTransformPosition; // 車の位置
    private float startTime; // スタートした時間
    private float finishTime; // 終わった時間

    private float servoAngle; // servoの向いてる向き
    private bool first_is; // 何かのフラグ
    private bool isRunning; // 多重のコルーチンを防ぐやつ
    private string servoAim; // 向く向きを一時保存

    /*
    forward
    255:0.85    0.00319548872180451
    100:0.3334  0.003334

    right
    255:456.66  0.558402312442517
    100:143     0.699300699300699
     */

    void Start()
    {
        // フレームレート・カメラ設定関係
        foreach (Camera cam in Camera.allCameras)
        {
            cam.enabled = false; // カメラを無効化
        }
        Time.fixedDeltaTime = 0.01f; // 1フレームを0.01秒とする
        Time.timeScale = timeScale; // 倍速
        Application.targetFrameRate = -1; // フレームレート制限を解除
        QualitySettings.vSyncCount = 0;   // V-Syncを無効化

        // 補正値関係
        carLinearSpeedCorrected = carLinearSpeed * carLinearSpeedCorrectionValue;
        carTurningSpeedCorrected = carTurningSpeed * carTurningSpeedCorrectionValue;

        // 初期設定
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; // 重力を0にする
        setCarMesh(0.115f, 0.0775f); // 車の見た目と当たり判定を定義 縦23cm 横15.5cm
        carTransform = GetComponent<Transform>();

        sensorScript = GetComponentInChildren<SensorScript>();
        sensorScript.setSensorMesh(obstacleDetectDistance);
        sensorTransform = GetComponentInChildren<Transform>();

        filePath = Application.dataPath + "/data.csv";
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "obstacleDetectDistance,carLinearSpeed,carTurningSpeed,servoRange,finishTime\n");
        }

        Init();
    }

    void FixedUpdate()
    {
        /*
        if (!isRunning) {
            StartCoroutine(CarMotionControl("right", 100));
            isRunning = true;
        }
        */
        ///*
        if (!isRunning)
        {
            isRunning = true;
            StartCoroutine(ObstacleAvoidance());
        }
        //*/
        
    }
    private void Init()
    {
        first_is = true;
        isRunning = false;
        carStockCount = 0;
        servoAngle = 0;
        servoAim = "";
        setTransformDefault();
        setValueRandom();
        setPrefabRamdom();
        startTime = Time.time;
    }

    public void SaveLogData()
    {
        string newLine = $"{obstacleDetectDistance},{carLinearSpeed},{carTurningSpeed},{servoRange},{finishTime}\n";
        File.AppendAllText(filePath, newLine); // データを追記
    }

    private void setValueRandom()
    {
        obstacleDetectDistance = Random.Range(0.1f, 1.0f); // 10cmから1mまでの間
        carLinearSpeed = Random.Range(100, 255);
        carTurningSpeed = Random.Range(100, 255);
        servoRange = Random.Range(30, 90); // 30°から90°の間
    }

    private void setTransformDefault()
    {
        transform.position = new Vector3(2.5f, -2.5f, 0);
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0.0f, 90.0f));
    }

    private IEnumerator ObstacleAvoidance()
    {
        if (transform.position.x <= -2 && 2 <= transform.position.y)
        {
            finishTime = Time.time - startTime;
            SaveLogData();
            Init();
            Debug.Log("Goal");
            yield return null;
        }

        if (100 < carStockCount)
        {
            finishTime = Time.time - startTime;
            Init();
            Debug.Log("Stack");
            yield return null;
        }

        if (first_is)
        {
            // Arduinoのコードを良く読むと、本体が50ms回転してから、入力を止めずに450ms、ServoMotorといっしょに回転してる
            // 1. 50ms間本体回転、ループ終了
            // 2. ループ開始、450ms間ServoMotor回転、本体回転
            // 3. ServoMotor止まる 本体が他の動きで入力が上書きされる
            switch (servoAim)
            {
                case "right":
                    yield return StartCoroutine(CarMotionControl("right", 45));
                    servoAim = "";
                    break;
                case "forward":
                    yield return StartCoroutine(CarMotionControl("forward", 45));
                    servoAim = "";
                    break;
                case "left":
                    yield return StartCoroutine(CarMotionControl("left", 45));
                    servoAim = "";
                    break;
                case "":
                    yield return WaitForFixedFrames(45);
                    break;
            }
            servoAngle = 0f;
            sensorScript.setSensorRotation(servoAngle);
            first_is = false;
        }

        float distance = GetDistance(obstacleDetectDistance, servoAngle);
        if (distance <= obstacleDetectDistance) // 近いなら
        {
            for (int i = -1; i <= 1; i++)
            {
                servoAngle = servoRange * i; // -servoRange, 0, servoRange
                sensorScript.setSensorRotation(servoAngle); // sensorを回す
                yield return WaitForFixedFrames(45); // 450ms待つ ServoMotorが回ってる時間

                distance = GetDistance(obstacleDetectDistance, servoAngle);

                if (distance <= obstacleDetectDistance) // 近いなら
                {
                    yield return StartCoroutine(CarMotionControl("stop", 1));
                    if (i == 1)
                    {
                        yield return StartCoroutine(CarMotionControl("backward", 50));
                        yield return StartCoroutine(CarMotionControl("right", 5));
                        servoAim = "right";
                        first_is = true;
                    }
                }
                else
                {
                    // 前回の車の位置と同じなら
                    if(carTransformPosition == carTransform.position)
                    {
                        carStockCount += 10;
                    }
                    else // 位置が違うなら
                    {
                        carTransformPosition = carTransform.position;
                    }

                    if (i == -1)
                    {
                        yield return StartCoroutine(CarMotionControl("right", 5));
                        servoAim = "right";
                    }
                    else if (i == 0)
                    {
                        yield return StartCoroutine(CarMotionControl("forward", 5));
                        servoAim = "forward";
                    }
                    else if (i == 1)
                    {
                        yield return StartCoroutine(CarMotionControl("left", 5));
                        servoAim = "left";
                    }
                    first_is = true;
                    break;
                }
            }
        }
        else // 遠いなら 1フレーム前に進む
        {
            // 前回の車の位置と同じなら
            if (carTransformPosition == carTransform.position)
            {
                carStockCount++;
            }
            else // 位置が違うなら
            {
                carTransformPosition = carTransform.position;
            }
            yield return StartCoroutine(CarMotionControl("forward", 1));
        }
        isRunning = false;
    }

    float GetDistance(float distance, float dir)
    {
        int layerMask = ~LayerMask.GetMask("Ignore Raycast");
        Vector2 rayDirection = Quaternion.Euler(0, 0, dir) * sensorTransform.up;
        RaycastHit2D hit = Physics2D.Raycast(sensorTransform.position, rayDirection, distance, layerMask);
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
                rb.linearVelocity = transform.up * carLinearSpeedCorrected;
                break;
            case "backward":
                rb.linearVelocity = -transform.up * carLinearSpeedCorrected;
                break;
            case "right":
                rb.angularVelocity = -carTurningSpeedCorrected * Mathf.Deg2Rad;
                break;
            case "left":
                rb.angularVelocity = carTurningSpeedCorrected * Mathf.Deg2Rad;
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

    void setCarMesh(float height, float width)
    {
        Mesh mesh = new Mesh();

        // 頂点を設定
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-width, -height, 0),
            new Vector3(width, -height, 0),
            new Vector3(-width, height, 0),
            new Vector3(width, height, 0)
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

        Material carMaterial = new Material(Shader.Find("Unlit/Color"));
        carMaterial.color = Color.white;

        GetComponent<MeshRenderer>().material = carMaterial;
    }

    void generatePrefab(float x, float y)
    {
        GameObject prefabObj = (GameObject)Resources.Load("Object");

        float randomX = Random.Range(x - 1, x + 1);
        float randomY = Random.Range(y - 1, y + 1);
        Vector3 randomPosition = new Vector3(randomX, randomY, 0);

        float randomZRotation = Random.Range(0.0f, 360.0f);
        Quaternion randomRotation = Quaternion.Euler(0, 0, randomZRotation);

        Instantiate(prefabObj, randomPosition, randomRotation);
    }

    void setPrefabRamdom()
    {
        GameObject[] prefabs = GameObject.FindGameObjectsWithTag("Prefab");
        foreach (GameObject prefab in prefabs)
        {
            Destroy(prefab);
        }

        Vector2[] prefabPositions = new Vector2[] {
            new Vector2(0.0f, -2.0f),
            new Vector2(-2.0f, 0.0f),
            new Vector2(0.0f, 0.0f),
            new Vector2(2.0f, 0.0f),
            new Vector2(0.0f, 2.0f),
        };

        foreach (var prefabPosition in prefabPositions)
        {
            generatePrefab(prefabPosition.x, prefabPosition.y);
        }
    }
}