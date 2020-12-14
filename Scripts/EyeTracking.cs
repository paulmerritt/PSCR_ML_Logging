﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using TMPro;

namespace LoggerScripts{
public class EyeTracking : MonoBehaviour {
    #region Public Variables
    public GameObject Camera;
    #endregion

    #region Private Variables
    private Vector3 _heading;
    #endregion

    private string[] eyes;
    private LogToConsoleHelper consoler = new LogToConsoleHelper();
    #region Unity Methods
    void Start() {
        MLEyes.Start();
        transform.position = Camera.transform.position + Camera.transform.forward * 5.0f;
        eyes = new string[2];
        LogToFileHelper logger = new LogToFileHelper();
        //establishing logger for storing interactions locally as a backup
        StartCoroutine(logger.LogToFileStringArray("log_eye.json", eyes));
        //creating new session
        StartCoroutine(consoler.NewSession("http://"+LoggingConfig.ip_address+":" + LoggingConfig.port +"/ext/"+ LoggingConfig.api_key + "/new_session"));
    }    
    private void OnDisable() {
        MLEyes.Stop();
    } 
    
    void Update() {
        if (MLEyes.IsStarted) {
            RaycastHit rayHit;
            eyes[0] = "" + MLEyes.FixationPoint;
            _heading = MLEyes.FixationPoint - Camera.transform.position;
            LogToConsoleHelper.jsn_sent j = new LogToConsoleHelper.jsn_sent();
            j.entry_id = 1;
            j.session_name = "Eye Positions";
            j.message_data = "eye position: " + eyes[0] + " looking at " + eyes[1];
            j.time_created = ""+System.DateTime.Now;
            j.category = "External";
            

            string s = "[" + JsonUtility.ToJson(j) + "]";
            //post where eyes are currently looking
            if (consoler.session_id != 0){
                StartCoroutine(consoler.PostRequest("http://"+LoggingConfig.ip_address+":" + LoggingConfig.port +"/ext/"+ LoggingConfig.api_key+"/"+consoler.session_id, s));
            }
            
            // add the object the eyes are looking at, if such exists
            if (Physics.Raycast(Camera.transform.position, _heading, out rayHit, 10.0f)) {
                eyes[1] = rayHit.collider.gameObject.name;
            }
            else { 
                eyes[1] = "no object";
            }
            
        }
    }
    #endregion
}
}