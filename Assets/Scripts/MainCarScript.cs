using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCarScript : MonoBehaviour
{
    private Material carMaterial;

    public float moveSpeed = 1f; // 移動速度
    public float rotationSpeed = 90f; // 回転速度

    void Start()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0f;

        // メッシュを生成
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

        // 三角形を定義 (面)
        int[] triangles = new int[6]
        {
            0, 2, 1, // 1つ目の三角形
            2, 3, 1  // 2つ目の三角形
        };
        mesh.triangles = triangles;


        // メッシュをMeshFilterに設定
        GetComponent<MeshFilter>().mesh = mesh;

        // マテリアルを作成
        carMaterial = new Material(Shader.Find("Unlit/Color"));
        carMaterial.color = Color.white;

        // 作成したマテリアルをMeshRendererに設定
        GetComponent<MeshRenderer>().material = carMaterial;
    }


    void Update()
    {
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
    }

    public void SensorEnter()
    {
        carMaterial.color = Color.red;
    }
    public void SensorExit()
    {
        carMaterial.color = Color.white;
    }
}
