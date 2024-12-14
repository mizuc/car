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
        // �����œ����蔻���ݒ�
        GetComponent<PolygonCollider2D>().points = v2;

        // ������Vector3�ł��`
        Vector3[] v3 = new Vector3[v2.Length];
        for (int i = 0; i < v2.Length; i++)
        {
            v3[i] = new Vector3(v2[i][0], v2[i][1], 0);
        }

        Mesh mesh = new Mesh();

        // ���_��ݒ�
        mesh.vertices = v3;

        // �ʂ��`
        int[] triangles = new int[(v2.Length - 2) * 3];
        for (int i = 0; i < v2.Length - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        mesh.triangles = triangles;

        // ���b�V����MeshFilter�ɐݒ�
        GetComponent<MeshFilter>().mesh = mesh;

        // �}�e���A�����쐬
        sensorMaterial = new Material(Shader.Find("Unlit/Color"));
        sensorMaterial.color = Color.yellow;

        // �쐬�����}�e���A����MeshRenderer�ɐݒ�
        GetComponent<MeshRenderer>().material = sensorMaterial;
    }

    void setSensorRotation(float angle)
    {
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
