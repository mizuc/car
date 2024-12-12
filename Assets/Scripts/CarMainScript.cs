using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMainScript : MonoBehaviour
{
    public float rotationSpeed = 90f; // ��]���x
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // �d�͂�0�ɂ���
        GetComponent<Rigidbody2D>().gravityScale = 0f;

        // SpriteRenderer�R���|�[�l���g���擾
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer��������܂���I");
        }
        spriteRenderer.color = Color.white;
    }


    void Update()
    {
        // �I�u�W�F�N�g����]
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �Փˎ��ɐF��ԂɕύX
        spriteRenderer.color = Color.red;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // ���ꂽ�Ƃ��ɐF�𔒂ɕύX
        spriteRenderer.color = Color.white;
    }
}
