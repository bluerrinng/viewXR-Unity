using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 5f;         // 이동 속도
    public float lookSpeedX = 2f;        // 마우스 X 회전 속도
    public float lookSpeedY = 2f;        // 마우스 Y 회전 속도
    public float verticalMoveSpeed = 3f; // 위아래 이동 속도 (Shift/Ctrl 키 사용)

  

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    void Update()
    {
        if (VRManager.instance.monitorCamera != null && VRManager.instance.monitorCamera.enabled)
        {
            MoveCamera();
        }
    }

    void MoveCamera()
    {
        Camera cam = VRManager.instance.monitorCamera;
        Transform camTransform = cam.transform;

        float moveX = Input.GetAxis("Horizontal"); // A/D
        float moveZ = Input.GetAxis("Vertical");   // W/S

        Vector3 move = camTransform.right * moveX + camTransform.forward * moveZ;
        camTransform.position += move * moveSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            camTransform.position += Vector3.up * verticalMoveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            camTransform.position += Vector3.down * verticalMoveSpeed * Time.deltaTime;
        }

        // (선택 사항) 마우스로 회전 제어도 추가하고 싶다면 아래 주석 해제:
        /*
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationY += Input.GetAxis("Mouse X") * lookSpeedX;
        camTransform.localEulerAngles = new Vector3(rotationX, rotationY, 0f);
        */
    }
}
