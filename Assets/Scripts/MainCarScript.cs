using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCarScript : MonoBehaviour
{
    private Material carMaterial;

    public float moveSpeed = 1f; // �ړ����x
    public float rotationSpeed = 90f; // ��]���x

    void Start()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0f;

        // ���b�V���𐶐�
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

        // �O�p�`���` (��)
        int[] triangles = new int[6]
        {
            0, 2, 1, // 1�ڂ̎O�p�`
            2, 3, 1  // 2�ڂ̎O�p�`
        };
        mesh.triangles = triangles;


        // ���b�V����MeshFilter�ɐݒ�
        GetComponent<MeshFilter>().mesh = mesh;

        // �}�e���A�����쐬
        carMaterial = new Material(Shader.Find("Unlit/Color"));
        carMaterial.color = Color.white;

        // �쐬�����}�e���A����MeshRenderer�ɐݒ�
        GetComponent<MeshRenderer>().material = carMaterial;
    }


    void Update()
    {
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
