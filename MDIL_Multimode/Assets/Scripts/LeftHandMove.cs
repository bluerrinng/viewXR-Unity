using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class LeftHandMove : MonoBehaviour
{
    public float verticalMoveSpeed = 1.0f;
    private InputDevice leftHand;
    public Transform xrRigTransform; // XR Origin의 transform

    void Start()
    {
        // 왼손 컨트롤러 찾기
        var desiredCharacteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, devices);
        if (devices.Count > 0)
            leftHand = devices[0];
    }

    void Update()
    {
        if (!leftHand.isValid)
        {
            // 다시 시도 (연결 안 된 경우)
            var desiredCharacteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, devices);
            if (devices.Count > 0)
                leftHand = devices[0];
        }

        // A 버튼 (primaryButton) → 위로
        if (leftHand.TryGetFeatureValue(CommonUsages.primaryButton, out bool aPressed) && aPressed)
        {
            xrRigTransform.position += Vector3.down * verticalMoveSpeed * Time.deltaTime;
        }

        // B 버튼 (secondaryButton) → 아래로
        if (leftHand.TryGetFeatureValue(CommonUsages.secondaryButton, out bool bPressed) && bPressed)
        {
            xrRigTransform.position += Vector3.up * verticalMoveSpeed * Time.deltaTime;
        }
    }
}
