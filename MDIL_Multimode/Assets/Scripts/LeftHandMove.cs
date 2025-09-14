using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class LeftHandMove : MonoBehaviour
{
    public float verticalMoveSpeed = 1.0f;
    private InputDevice leftHand;
    public Transform xrRigTransform; // XR Origin�� transform

    void Start()
    {
        // �޼� ��Ʈ�ѷ� ã��
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
            // �ٽ� �õ� (���� �� �� ���)
            var desiredCharacteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
            var devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, devices);
            if (devices.Count > 0)
                leftHand = devices[0];
        }

        // A ��ư (primaryButton) �� ����
        if (leftHand.TryGetFeatureValue(CommonUsages.primaryButton, out bool aPressed) && aPressed)
        {
            xrRigTransform.position += Vector3.down * verticalMoveSpeed * Time.deltaTime;
        }

        // B ��ư (secondaryButton) �� �Ʒ���
        if (leftHand.TryGetFeatureValue(CommonUsages.secondaryButton, out bool bPressed) && bPressed)
        {
            xrRigTransform.position += Vector3.up * verticalMoveSpeed * Time.deltaTime;
        }
    }
}
