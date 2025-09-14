using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInput : MonoBehaviour
{
    public float rotateSpeed = 2.0f;  // ī�޶� ȸ�� �ӵ� (���� �ʹ� ū ���ڴ� �����ؾ���)
    public float verticalLimit = 80f; // ���� ȸ�� ���� (���� ����)
    private float mouseX;
    private float mouseY;

    // Start is called once before the first execution of Update
    void Start()
    {
        // ���콺 Ŀ���� ȭ�� �߾����� �����ϰ�, Ŀ�� ǥ�ø� ����ϴ�.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        RotateCamera();
    }

    private void RotateCamera()
    {
        // ���콺 �Է°��� �޾ƿɴϴ�.
        float inputX = Input.GetAxis("Mouse X");
        float inputY = Input.GetAxis("Mouse Y");

        // ���콺 �̵��� ���� ȸ���� ������Ʈ
        mouseX += inputX * rotateSpeed;
        mouseY -= inputY * rotateSpeed;

        // ���콺 Y �̵� ���� (���� ���� ����)
        mouseY = Mathf.Clamp(mouseY, -verticalLimit, verticalLimit);

        // ī�޶� ȸ�� ����
        transform.localRotation = Quaternion.Euler(mouseY, mouseX, 0);
    }
}
