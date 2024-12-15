using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCarScript : MonoBehaviour
{
    private Material carMaterial;
    private SensorScript sensorScript;

    public float moveSpeed = 1f; // �ړ����x
    public float rotationSpeed = 90f; // ��]���x

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
        
        // �Z���T�[�̃��b�V���Ɠ����蔻����`
        Vector2[] sensorVertices = new Vector2[3]
        {
        new Vector2(0, 0),          //����
        new Vector2(-0.1f, 0.2f),   //�E��
        new Vector2(0.1f, 0.2f),    //����
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
                if (Input.GetKey(KeyCode.W)) // W�L�[�őO�i
                {
                    transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.S)) // S�L�[�Ō�i
                {
                    transform.Translate(Vector3.up * -moveSpeed * Time.deltaTime);
                }

                if (Input.GetKey(KeyCode.A)) // A�L�[�ō���]
                {
                    transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.D)) // D�L�[�ŉE��]
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

        // ���_��ݒ�
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-0.1f, -0.15f, 0), //����
            new Vector3(0.1f, -0.15f, 0),  //�E��
            new Vector3(-0.1f, 0.15f, 0),  //����
            new Vector3(0.1f, 0.15f, 0)    //�E��
        };
        mesh.vertices = vertices;

        // �ʂ��`
        int[] triangles = new int[6]
        {
            0, 2, 1, // 1�ڂ̎O�p�`
            2, 3, 1  // 2�ڂ̎O�p�`
        };
        mesh.triangles = triangles;

        GetComponent<MeshFilter>().mesh = mesh;

        carMaterial = new Material(Shader.Find("Unlit/Color"));
        carMaterial.color = Color.white;

        GetComponent<MeshRenderer>().material = carMaterial;
    }
}
