using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.UI;

namespace LoggerScripts {


public class HandTrackScript : MonoBehaviour
{
    //public enum HandPoses { Ok, Finger, Thumb, OpenHand, Fist, NoPose };
    private string[] pose;
    //public GameObject sphereThumb, sphereIndex, sphereWrist;
    public Text posetext;
    private MLHandTracking.HandKeyPose[] _gestures;
    private LogToConsoleHelper consoler = new LogToConsoleHelper();
    private void Start()
    {
        MLHandTracking.Start();
        pose = new string[2];
        _gestures = new MLHandTracking.HandKeyPose[5];
        _gestures[0] = MLHandTracking.HandKeyPose.Ok; //60
        _gestures[1] = MLHandTracking.HandKeyPose.Finger; //48
        _gestures[2] = MLHandTracking.HandKeyPose.OpenHand; //48-50
        _gestures[3] = MLHandTracking.HandKeyPose.Fist; //49-51
        _gestures[4] = MLHandTracking.HandKeyPose.Thumb; //48-50
        // L = 48-49
        pose[0] = "default";
        MLHandTracking.KeyPoseManager.EnableKeyPoses(_gestures, true, false);
        
        LogToFileHelper logger = new LogToFileHelper();
        StartCoroutine(logger.LogToFileStringArray("log_hands.json", pose));
        StartCoroutine(consoler.NewSession("http://"+LoggingConfig.ip_address+":57000/ext/"+ LoggingConfig.api_key + "/new_session"));
    }
    private void OnDestroy()
    {
        MLHandTracking.Stop();
    }
    private void Update()
    {
        
        pose[0] = GetGesture(MLHandTracking.Left);
        posetext.text = pose[0];
        pose[1] = posetext.text;
         LogToConsoleHelper.jsn_sent j = new LogToConsoleHelper.jsn_sent();
         j.entry_id = 1;
         j.message_data = "hand position: " + pose[0] + pose[1];
         j.time_created = ""+System.DateTime.Now;
         j.category = "External";
        

         string s = "[" + JsonUtility.ToJson(j) + "]";
         if (consoler.session_id != 0){
             StartCoroutine(consoler.PostRequest("http://"+LoggingConfig.ip_address+":57000/ext/"+ LoggingConfig.api_key+"/"+consoler.session_id, s));
         }
    }

    private string GetGesture(MLHandTracking.Hand hand)
    {
        if (hand != null && hand.HandKeyPoseConfidence > 0.9f)
        {
            switch (hand.KeyPose)
            {
                case MLHandTracking.HandKeyPose.Ok:
                return "Ok";

                case MLHandTracking.HandKeyPose.Finger:
                return "Finger";

                case MLHandTracking.HandKeyPose.OpenHand:
                return "OpenHand";

                case MLHandTracking.HandKeyPose.Fist:
                return "Fist";

                case MLHandTracking.HandKeyPose.Thumb:
                return "Thumb";

                default:
                return "NoPose";
            }   
        }
        else {
            return "null";
        }
    }
}
}