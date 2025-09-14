using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class SwitchMode : MonoBehaviour
{
    private bool isVRMode = false;

    private Vector3 lastNormalCamPos;
    private Quaternion lastNormalCamRot;

    private Vector3 lastVRCamPos;
    private Quaternion lastVRCamRot;

    private UnityEngine.XR.InputDevice rightHand;

    void Start()
    {
        // 초기 위치 저장
        lastNormalCamPos = VRManager.instance.monitorCamera.transform.position;
        lastNormalCamRot = VRManager.instance.monitorCamera.transform.rotation;

        lastVRCamPos = VRManager.instance.xrOrigin.transform.position;
        lastVRCamRot = VRManager.instance.xrOrigin.transform.rotation;

        // 일반 모드로 시작: 일반 카메라 켜고 XR 비활성화
        VRManager.instance.monitorCamera.gameObject.SetActive(true);
        VRManager.instance.xrOrigin.SetActive(false);

        // XR 카메라도 꺼두기
        Camera xrCam = VRManager.instance.xrOrigin.GetComponentInChildren<Camera>();
        if (xrCam != null)
            xrCam.enabled = false;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isVRMode)
            {
                // VR → 일반 모드로 전환
                lastVRCamPos = VRManager.instance.xrOrigin.transform.position;
                lastVRCamRot = VRManager.instance.xrOrigin.transform.rotation;

                StartCoroutine(StopXRAndRestorePosition());
            }
            else
            {
                // 일반 → VR 모드로 전환
                lastNormalCamPos = VRManager.instance.monitorCamera.transform.position;
                lastNormalCamRot = VRManager.instance.monitorCamera.transform.rotation;

                StartCoroutine(StartXR());
            }

            isVRMode = !isVRMode;
        }
    }

    IEnumerator StartXR()
    {
        Debug.Log("Starting XR...");

        XRGeneralSettings.Instance.Manager.InitializeLoaderSync();

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("Initializing XR Failed.");
            yield break;
        }

        XRGeneralSettings.Instance.Manager.StartSubsystems();

        // 일반 카메라 비활성화
        VRManager.instance.monitorCamera.gameObject.SetActive(false);

        // XR Origin 활성화
        VRManager.instance.xrOrigin.SetActive(true);

        // XR 카메라 활성화
        Camera xrCam = VRManager.instance.xrOrigin.GetComponentInChildren<Camera>();
        if (xrCam != null)
            xrCam.enabled = true;

        // 위치 복원
        VRManager.instance.xrOrigin.transform.position = lastNormalCamPos;
        VRManager.instance.xrOrigin.transform.rotation = lastNormalCamRot;
    }

    IEnumerator StopXRAndRestorePosition()
    {
        Debug.Log("Stopping XR...");

        XRGeneralSettings.Instance.Manager.StopSubsystems();
        XRGeneralSettings.Instance.Manager.DeinitializeLoader();

        // XR 카메라 비활성화
        Camera xrCam = VRManager.instance.xrOrigin.GetComponentInChildren<Camera>();
        if (xrCam != null)
            xrCam.enabled = false;

        VRManager.instance.xrOrigin.SetActive(false);

        // 한 프레임 대기: XR 시스템 완전 종료 보장
        yield return null;

        // 일반 카메라 위치 복원
        VRManager.instance.monitorCamera.transform.position = lastVRCamPos;
        VRManager.instance.monitorCamera.transform.rotation = lastVRCamRot;

        VRManager.instance.monitorCamera.gameObject.SetActive(true);
    }
}
