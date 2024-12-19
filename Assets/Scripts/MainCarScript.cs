using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCarScript : MonoBehaviour
{
    Rigidbody2D rb;
    private Transform sensorTransform; // sensorの座標
    private SensorScript sensorScript;

    public float timeScale = 100f;

    // 乱数で決める変数
    public float obstacleDetectDistance = 1; // 1m
    public float carSpeed = 1; //1秒で1m
    public float rotationSpeed = 60 * 1147.6333f; // 回転するスピード この値だとちょうど60度回る rotationSpeed:servoAngleDiff = 68858:60
    public float servoAngleDiff = 60;

    /*
    forward
    255:85
    100:33.34

    right
    255:456.66
    100:143
     */

    private float servoAngle = 0;
    private bool first_is = true;
    private bool isRunning = false;
    

    void Start()
    {
        Time.fixedDeltaTime = 0.01f; // 1フレームを0.01秒とする
        Time.timeScale = timeScale; // 倍速
        Application.targetFrameRate = -1; // フレームレート制限を解除
        QualitySettings.vSyncCount = 0;   // V-Syncを無効化

        //setRandom();

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f; //重力を0にする
        setCarMesh(0.23f/2, 0.155f/2); //車の見た目と当たり判定を定義

        sensorScript = GetComponentInChildren<SensorScript>();
        sensorScript.setSensorMesh(obstacleDetectDistance);
        sensorTransform = GetComponentInChildren<Transform>();
    }

    void FixedUpdate()
    {
        if (!isRunning) {
            StartCoroutine(CarMotionControl("right", 100));
            isRunning = true;
        }
        /*
        if (!isRunning)
        {
            isRunning = true;
            StartCoroutine(ObstacleAvoidance());
        }
        */
    }

    private void setRandom()
    {
        obstacleDetectDistance = Random.Range(0.1f, 1); // 10cmから1mまでの間
        carSpeed = Random.Range(0.5f, 2); // あとで計測
        rotationSpeed = Random.Range(20, 90) * 1147.6333f; // 20°から90°の間
        servoAngleDiff = Random.Range(20, 90); // 20°から90°の間
    }

    private IEnumerator ObstacleAvoidance()
    {
        if (first_is)
        {
            servoAngle = 0f;
            sensorScript.setSensorRotation(servoAngle);
            yield return WaitForFixedFrames(45); // 450ms待つ ServoMotorが回ってる時間
            first_is = false;
        }

        float distance = GetDistance(servoAngle, obstacleDetectDistance);
        if (distance <= obstacleDetectDistance) // 近いなら
        {
            for (int i = -1; i <= 1; i++)
            {
                servoAngle = servoAngleDiff * i; // -servoAngleDiff, 0, servoAngleDiff
                sensorScript.setSensorRotation(servoAngle); // sensorを回す
                yield return WaitForFixedFrames(45); // 450ms待つ ServoMotorが回ってる時間

                distance = GetDistance(servoAngle, obstacleDetectDistance);

                if (distance <= obstacleDetectDistance) // 近いなら
                {
                    yield return StartCoroutine(CarMotionControl("stop", 1));
                    if (i == 1)
                    {
                        yield return StartCoroutine(CarMotionControl("backward", 50));
                        yield return StartCoroutine(CarMotionControl("right", 5));
                        first_is = true;
                    }
                }
                else
                {
                    if (i == -1)
                    {
                        yield return StartCoroutine(CarMotionControl("right", 5));
                        // 1/10くらいの確率でランダムな角度で曲がるのはどうか
                    }
                    else if (i == 0)
                    {
                        yield return StartCoroutine(CarMotionControl("forward", 5));
                    }
                    else if (i == 1)
                    {
                        yield return StartCoroutine(CarMotionControl("left", 5));
                    }
                    first_is = true;
                    break;
                }
            }
        }
        else // 遠いなら 1フレーム前に進む
        {
            yield return StartCoroutine(CarMotionControl("forward", 1));
        }
        isRunning = false;
    }

    float GetDistance(float distance, float dir)
    {
        int layerMask = ~LayerMask.GetMask("Ignore Raycast");
        Vector2 rayDirection = Quaternion.Euler(0, 0, dir) * sensorTransform.up;
        RaycastHit2D hit = Physics2D.Raycast(sensorTransform.position, rayDirection, Mathf.Infinity, layerMask);

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
}