using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCarScript : MonoBehaviour
{
    private Material carMaterial;
    private SensorScript sensorScript;

    public float moveSpeed = 1f; // 車のスピード
    public float rotationSpeed = 90f; // 回転するスピード

    public float servoAngle = 90f;
    public float obstacleDetectDistance = 1f;  // 20cm
    public Transform sensorTransform;             // sensorの座標
    public float carSpeed = 1f;
    private bool first_is = true;

    void Start()
    {
        Time.fixedDeltaTime = 0.01f; // 1フレームを0.01秒とする

        GetComponent<Rigidbody2D>().gravityScale = 0f;

        setCarMesh();
        
        // sensorの頂点
        Vector2[] sensorVertices = new Vector2[3]
        {
        new Vector2(0, 0),
        new Vector2(-0.1f, obstacleDetectDistance),
        new Vector2(0.1f, obstacleDetectDistance)
        };
        sensorScript = GetComponentInChildren<SensorScript>();
        sensorScript.setSensorMesh(sensorVertices);

        sensorTransform = GetComponentInChildren<Transform>();
    }

    void FixedUpdate()
    {
        if (true)
        {
            StartCoroutine(ObstacleAvoidance()); // コルーチンで実行
        }
        else
        {
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.up * -moveSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(Vector3.forward * -rotationSpeed * Time.deltaTime);
            }
        }
    }

    public void SensorEnter()
    {
        carMaterial.color = Color.red;
    }

    public void SensorExit()
    {
        carMaterial.color = Color.white;
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

    private IEnumerator ObstacleAvoidance()
    {
        if (first_is)
        {
            servoAngle = 0f;
            sensorTransform.localRotation = Quaternion.Euler(0f, 0f, -servoAngle); // 最初だけ向きを90度にする

            first_is = false;
            yield return null; //1フレーム飛ばす
        }

        float distance = GetDistance();
        Debug.Log(distance);

        if (distance <= obstacleDetectDistance)
        {
            StartCoroutine(CarMotionControl("stop", 0));
            for (int i = 1; i <= 5; i += 2)
            {
                servoAngle = 30 * i;
                sensorTransform.localRotation = Quaternion.Euler(0f, 0f, -servoAngle);
                yield return new WaitForSeconds(0.45f);

                yield return new WaitForSeconds(0.1f); // 100ms待つ
                distance = GetDistance();

                if (distance <= obstacleDetectDistance)
                {
                    StartCoroutine(CarMotionControl("stop", 0.01f));
                    if (i == 5)
                    {
                        StartCoroutine(CarMotionControl("backward", 0.5f)); // 前進 0.5秒間実行
                        StartCoroutine(CarMotionControl("right", 0.05f)); // 右に回転 0.05秒間実行
                        first_is = true;
                    }
                }
                else
                {
                    if (i == 1)
                    {
                        StartCoroutine(CarMotionControl("right", 0.05f)); // 右に回転 0.05秒間実行
                    }
                    else if (i == 3)
                    {
                        StartCoroutine(CarMotionControl("forward", 0.05f)); // 前進 0.05秒間実行
                    }
                    else if (i == 5)
                    {
                        StartCoroutine(CarMotionControl("left", 0.05f)); // 左に回転 0.05秒間実行
                    }
                    first_is = true;
                    break;
                }
            }
        }
        else
        {
            StartCoroutine(CarMotionControl("forward", 0.01f)); // 前進 0.05秒間実行
        }
    }

    float GetDistance()
    {
        int layerMask = ~LayerMask.GetMask("Ignore Raycast"); // IgnoreRaycast レイヤーを無視
        RaycastHit2D hit = Physics2D.Raycast(sensorTransform.position, sensorTransform.up, Mathf.Infinity, layerMask);
        Debug.DrawRay(sensorTransform.position, sensorTransform.up * 1f, Color.red); // 1mの長さで赤いレイを描画
        if (hit.collider != null)
        {
            return hit.distance;
        }
        return Mathf.Infinity; // 何もヒットしなかった場合は無限大
    }

    IEnumerator CarMotionControl(string direction, float duration)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        switch (direction)
        {
            case "forward":
                rb.linearVelocity = transform.up * carSpeed;
                break;
            case "backward":
                rb.linearVelocity = -transform.up * carSpeed;
                break;
            case "right":
                rb.angularVelocity = -rotationSpeed * Mathf.Deg2Rad;
                break;
            case "left":
                rb.angularVelocity = rotationSpeed * Mathf.Deg2Rad;
                break;
            case "stop":
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                break;
        }

        // 指定された秒数だけ待機
        yield return new WaitForSeconds(duration);

        // 動きを停止
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}