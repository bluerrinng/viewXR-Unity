using UnityEngine;

public class VRManager : MonoBehaviour
{
    public static VRManager instance;

    [Header("ī�޶� �� XR ������Ʈ")]
    public Camera monitorCamera;        // �Ϲ� ����Ϳ� ī�޶�
    public GameObject xrOrigin;         // XR Origin (XR Rig ��ü GameObject)

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
