using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCarScript : MonoBehaviour
{
    private Material carMaterial;
    private SensorScript sensorScript;

    public float moveSpeed = 1f; // 移動速度
    public float rotationSpeed = 90f; // 回転速度

    enum mode
    {
        forward = 0,
        backward = 1,
        right = 2,
        left = 3,
        stop = 4,
        servo = 5,
        control = 100

    };

    mode m = mode.control;

    void Start()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0f;

        setCarMesh();
        
        // センサーのメッシュと当たり判定を定義
        Vector2[] sensorVertices = new Vector2[3]
        {
        new Vector2(0, 0),          //左下
        new Vector2(-0.1f, 0.2f),   //右下
        new Vector2(0.1f, 0.2f),    //左上
        };
        sensorScript = GetComponentInChildren<SensorScript>();
        sensorScript.setSensorMesh(sensorVertices);
    }

    void Update()
    {
        switch (m)
        {
            case mode.forward:
                break;
            case mode.backward:
                break;
            case mode.right:
                break;
            case mode.left:
                break;
            case mode.stop:
                break;
            case mode.servo:
                break;
            case mode.control:
                if (Input.GetKey(KeyCode.W)) // Wキーで前進
                {
                    transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.S)) // Sキーで後進
                {
                    transform.Translate(Vector3.up * -moveSpeed * Time.deltaTime);
                }

                if (Input.GetKey(KeyCode.A)) // Aキーで左回転
                {
                    transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.D)) // Dキーで右回転
                {
                    transform.Rotate(Vector3.forward * -rotationSpeed * Time.deltaTime);
                }
                break;
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
            new Vector3(-0.1f, -0.15f, 0), //左下
            new Vector3(0.1f, -0.15f, 0),  //右下
            new Vector3(-0.1f, 0.15f, 0),  //左上
            new Vector3(0.1f, 0.15f, 0)    //右上
        };
        mesh.vertices = vertices;

        // 面を定義
        int[] triangles = new int[6]
        {
            0, 2, 1, // 1つ目の三角形
            2, 3, 1  // 2つ目の三角形
        };
        mesh.triangles = triangles;

        GetComponent<MeshFilter>().mesh = mesh;

        carMaterial = new Material(Shader.Find("Unlit/Color"));
        carMaterial.color = Color.white;

        GetComponent<MeshRenderer>().material = carMaterial;
    }
}
