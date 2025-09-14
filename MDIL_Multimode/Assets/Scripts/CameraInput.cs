using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInput : MonoBehaviour
{
    public float rotateSpeed = 2.0f;  // 카메라 회전 속도 (보통 너무 큰 숫자는 조정해야함)
    public float verticalLimit = 80f; // 세로 회전 제한 (각도 범위)
    private float mouseX;
    private float mouseY;

    // Start is called once before the first execution of Update
    void Start()
    {
        // 마우스 커서를 화면 중앙으로 고정하고, 커서 표시를 숨깁니다.
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
        // 마우스 입력값을 받아옵니다.
        float inputX = Input.GetAxis("Mouse X");
        float inputY = Input.GetAxis("Mouse Y");

        // 마우스 이동에 따라 회전값 업데이트
        mouseX += inputX * rotateSpeed;
        mouseY -= inputY * rotateSpeed;

        // 마우스 Y 이동 제한 (세로 각도 제한)
        mouseY = Mathf.Clamp(mouseY, -verticalLimit, verticalLimit);

        // 카메라 회전 적용
        transform.localRotation = Quaternion.Euler(mouseY, mouseX, 0);
    }
}
