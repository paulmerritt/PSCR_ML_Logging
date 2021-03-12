using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using System;
using UnityEngine.SceneManagement;

namespace LoggerScripts{

public class TouchpadGestures : MonoBehaviour {

    private string[] text;
    private GameObject camera;
    private MLInput.Controller _controller;
    private LogToConsoleHelper consoler = new LogToConsoleHelper();
    private float elapsed = 0.0f;
    private Scene scn; 
    private string sceneName;
    private bool sceneSelected = false;

    #region Unity Methods
    void Start() {
        MLInput.Start();
        camera = GameObject.FindWithTag("MainCamera");
        _controller = MLInput.GetController(MLInput.Hand.Left);
        text = new string[4];
        // text[0] = "default"; 
        // text[1] = "default"; 
        // text[2] = "default";
        // text[3] = "default";

        scn = SceneManager.GetActiveScene();
        sceneName = scn.name;

        LogToFileHelper logger = new LogToFileHelper();
        //establishing logger for storing interactions locally as a backup
        StartCoroutine(logger.LogToFileStringArray("log_touchpad.json", text));
        //creating new session
        StartCoroutine(consoler.NewSession("http://"+LoggingConfig.ip_address+":" + LoggingConfig.port +"/ext/"+ LoggingConfig.api_key + "/new_session"));
        
    }

    void OnDestroy() {
        MLInput.Stop();
    }  
        

    void Update(){

        if (SceneManager.GetActiveScene().name != sceneName && !sceneSelected){
                    LogToConsoleHelper.jsn_sent j = new LogToConsoleHelper.jsn_sent();
                    j.entry_id = 1;
                    j.message_data = "Scene is: " + SceneManager.GetActiveScene().name;
                    j.time_created = ""+System.DateTime.Now;
                    j.category = "External";
                    string s = "[" + JsonUtility.ToJson(j) + "]";
                    StartCoroutine(consoler.PostRequest("http://"+LoggingConfig.ip_address+":" + LoggingConfig.port +"/ext/"+ LoggingConfig.api_key+"/"+consoler.session_id, s));
                    sceneSelected = true;
                }

        elapsed += Time.deltaTime;
        if (elapsed >= LoggingConfig.frequency) {

            

            elapsed = elapsed % LoggingConfig.frequency;
            

            float speed = Time.deltaTime * 5.0f;

            Vector3 pos = camera.transform.position + camera.transform.forward;
            gameObject.transform.position = Vector3.SlerpUnclamped(gameObject.transform.position, pos, speed);

            Quaternion rot = Quaternion.LookRotation(gameObject.transform.position - camera.transform.position);
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, rot, speed);


            string gestureType = _controller.CurrentTouchpadGesture.Type.ToString();
            string gestureState = _controller.TouchpadGestureState.ToString();
            string gestureDirection = _controller.CurrentTouchpadGesture.Direction.ToString();


            text[0] = "Type: " + gestureType;

            text[1] = "State: " + gestureState;

            text[2] = "Direction: " + gestureDirection;

            try{
                    LogToConsoleHelper.jsn_sent j = new LogToConsoleHelper.jsn_sent();
                    j.entry_id = 1;
                    j.message_data = text[0] + text[1] + text[2];
                    j.time_created = ""+System.DateTime.Now;
                    j.category = "External";
                    
                    string s = "[" + JsonUtility.ToJson(j) + "]";
                    //post any touchpad gestures to session
                    if (consoler.session_id != 0){
                        StartCoroutine(consoler.PostRequest("http://"+LoggingConfig.ip_address+":" + LoggingConfig.port +"/ext/"+ LoggingConfig.api_key+"/"+consoler.session_id, s));
                    }
                    text[3] = consoler.receivedTextPost;
                }
                catch (Exception e){
                    text[3] = e.ToString();
                }
            }
        
    }
    #endregion

    

    #region Private Methods

    

    // void updateGestureText() {
    //     string gestureType = _controller.CurrentTouchpadGesture.Type.ToString();
    //     string gestureState = _controller.TouchpadGestureState.ToString();
    //     string gestureDirection = _controller.CurrentTouchpadGesture.Direction.ToString();


    //     text[0] = "Type: " + gestureType;

    //     text[1] = "State: " + gestureState;

    //     text[2] = "Direction: " + gestureDirection;

    //     try{
    //             LogToConsoleHelper.jsn_sent j = new LogToConsoleHelper.jsn_sent();
    //             j.entry_id = 1;
	// 		    j.message_data = text[0] + text[1] + text[2];
	// 		    j.time_created = ""+System.DateTime.Now;
	// 		    j.category = "External";
                 
    //             string s = "[" + JsonUtility.ToJson(j) + "]";

    //             if (consoler.session_id != 0){
    //                 StartCoroutine(consoler.PostRequest("http://"+LoggingConfig.ip_address+":61000/ext/"+ LoggingConfig.api_key+"/"+consoler.session_id, s));
    //             }
    //             text[3] = consoler.receivedTextPost;
    //         }
    //         catch (Exception e){
    //             text[3] = e.ToString();
    //         }
    // }
    
    // void updateTransform() {
    //     float speed = Time.deltaTime * 5.0f;

    //     Vector3 pos = Camera.transform.position + Camera.transform.forward;
    //     gameObject.transform.position = Vector3.SlerpUnclamped(gameObject.transform.position, pos, speed);

    //     Quaternion rot = Quaternion.LookRotation(gameObject.transform.position - Camera.transform.position);
    //     gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, rot, speed);
    // }
    #endregion
}
}