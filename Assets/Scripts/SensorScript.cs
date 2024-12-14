using UnityEngine;

public class SensorScript : MonoBehaviour
{
    private Material sensorMaterial;
    private MainCarScript mainCarScript;

    void Start()
    {
        mainCarScript = GetComponentInParent<MainCarScript>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        mainCarScript.SensorEnter();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        mainCarScript.SensorExit();
    }

    public void setSensorMesh(Vector2[] v2)
    {
        // 引数で当たり判定を設定
        GetComponent<PolygonCollider2D>().points = v2;

        // 引数のVector3版を定義
        Vector3[] v3 = new Vector3[v2.Length];
        for (int i = 0; i < v2.Length; i++)
        {
            v3[i] = new Vector3(v2[i][0], v2[i][1], 0);
        }

        Mesh mesh = new Mesh();

        // 頂点を設定
        mesh.vertices = v3;

        // 面を定義
        int[] triangles = new int[(v2.Length - 2) * 3];
        for (int i = 0; i < v2.Length - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.triangles = triangles;

        // メッシュをMeshFilterに設定
        GetComponent<MeshFilter>().mesh = mesh;

        // マテリアルを作成
        sensorMaterial = new Material(Shader.Find("Unlit/Color"));
        sensorMaterial.color = Color.yellow;

        // 作成したマテリアルをMeshRendererに設定
        GetComponent<MeshRenderer>().material = sensorMaterial;
    }

    void setSensorRotation(float angle)
    {
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
