using UnityEngine;

public class SensorScript : MonoBehaviour
{    public void SetSensorMesh(float distance)
    {
        Mesh mesh = new Mesh();

        // í∏ì_Çê›íË
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(-0.01f, 0, 0), //ç∂â∫
            new Vector3(0.01f, 0, 0), //âEâ∫
            new Vector3(-0.01f, distance, 0), //ç∂è„
            new Vector3(0.01f, distance, 0) //âEè„
        };
        mesh.vertices = vertices;

        // ñ Çê›íË
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
