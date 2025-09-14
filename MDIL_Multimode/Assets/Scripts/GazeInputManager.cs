using UnityEngine;
using Varjo.XR;

public class GazeInputManager : MonoBehaviour
{
    public LayerMask imageLayerMask;
    public VarjoEyeTracking.GazeCalibrationMode gazeCalibrationMode = VarjoEyeTracking.GazeCalibrationMode.Fast;

    void Start()
    {
        Debug.Log("VR Initialized");


        //Check if Varjo XR-4 Gaze Setting Enabled
        if(VarjoEyeTracking.IsGazeAllowed())
        {
            Debug.Log("Gaze Enabled"); 
        }
        else
        {
            Debug.Log("Gaze Disabled");
        }


        //Check if Varjo XR-4 EyeTracking Calibrated
        if(VarjoEyeTracking.IsGazeCalibrated())
        {
            Debug.Log("Gaze Calibrated");
        }
        else
        {
            Debug.Log("Gaze Not Calibrated");
        }
    }

    void Update()
    {
        //Get GazeData from the GetGaze Methods
        VarjoEyeTracking.GazeData gazeData = VarjoEyeTracking.GetGaze();

        if(gazeData.status != VarjoEyeTracking.GazeStatus.Invalid)
        {
            //Ray Originates from the gaze.origin (Eye Tracker)
            Vector3 gazeOrigin = gazeData.gaze.origin;

            //Ray Direction from the gaze.origin
            Vector3 gazeDirection = gazeData.gaze.forward;


            RaycastHit hit;

            if (Physics.Raycast(gazeOrigin, gazeDirection, out hit, imageLayerMask))
            {
                GameObject gazedobject = hit.collider.gameObject;

                objectPos(gazedobject.transform.position);

            }

        }
    }


    public void objectPos(Vector3 pos)
    {
        Debug.Log("Ray hit object with position : " + pos); 
    } 
   
}
