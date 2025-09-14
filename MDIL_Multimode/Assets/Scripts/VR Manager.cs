using UnityEngine;

public class VRManager : MonoBehaviour
{
    public static VRManager instance;

    [Header("카메라 및 XR 오브젝트")]
    public Camera monitorCamera;        // 일반 모니터용 카메라
    public GameObject xrOrigin;         // XR Origin (XR Rig 전체 GameObject)

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
