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

        // ���b�V���𐶐�
        Mesh mesh = new Mesh();

        // ���_��ݒ�
        Vector3[] vertices = new Vector3[3]
        {
        new Vector3(0, 0, 0), //����
        new Vector3(-0.1f, 1, 0),  //�E��
        new Vector3(0.1f, 1, 0),  //����
        };
        mesh.vertices = vertices;

        // �O�p�`���` (��)
        int[] triangles = new int[3]
        {
            0, 1, 2 // 1�ڂ̎O�p�`
        };
        mesh.triangles = triangles;

        // ���b�V����MeshFilter�ɐݒ�
        GetComponent<MeshFilter>().mesh = mesh;

        // �}�e���A�����쐬
        sensorMaterial = new Material(Shader.Find("Unlit/Color"));
        sensorMaterial.color = Color.yellow;

        // �쐬�����}�e���A����MeshRenderer�ɐݒ�
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
