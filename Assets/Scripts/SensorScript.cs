using UnityEngine;

public class SensorScript : MonoBehaviour
{    public void SetSensorMesh(float distance)
    {
        Mesh mesh = new Mesh();

        // ���_��ݒ�
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-0.01f, 0, 0), //����
            new Vector3(0.01f, 0, 0), //�E��
            new Vector3(-0.01f, distance, 0), //����
            new Vector3(0.01f, distance, 0) //�E��
        };
        mesh.vertices = vertices;

        // �ʂ�ݒ�
        int[] triangles = new int[6]
        {
            0, 2, 1,
            2, 3, 1
        };
        mesh.triangles = triangles;

        GetComponent<MeshFilter>().mesh = mesh;

        Material sensorMaterial = new Material(Shader.Find("Unlit/Color"));
        sensorMaterial.color = Color.yellow;

        GetComponent<MeshRenderer>().material = sensorMaterial;
    }

    public void SetSensorRotation(float angle)
    {
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
