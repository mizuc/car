using UnityEngine;

public class SensorScript : MonoBehaviour
{
    private Material sensorMaterial;
    private MainCarScript mainCarScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0f;

        mainCarScript = GetComponentInParent<MainCarScript>();

        // メッシュを生成
        Mesh mesh = new Mesh();

        // 頂点を設定
        Vector3[] vertices = new Vector3[3]
        {
        new Vector3(0, 0, 0), //左下
        new Vector3(-0.1f, 1, 0),  //右下
        new Vector3(0.1f, 1, 0),  //左上
        };
        mesh.vertices = vertices;

        // 三角形を定義 (面)
        int[] triangles = new int[3]
        {
            0, 1, 2 // 1つ目の三角形
        };
        mesh.triangles = triangles;

        // メッシュをMeshFilterに設定
        GetComponent<MeshFilter>().mesh = mesh;

        // マテリアルを作成
        sensorMaterial = new Material(Shader.Find("Unlit/Color"));
        sensorMaterial.color = Color.yellow;

        // 作成したマテリアルをMeshRendererに設定
        GetComponent<MeshRenderer>().material = sensorMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        mainCarScript.SensorEnter();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        mainCarScript.SensorExit();
    }
}
