using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.Assertions;
using Newtonsoft.Json;
using System.Linq;

//used for the laser pointer
namespace LoggerScripts{
public class Control6DOF : MonoBehaviour {
    #region Private Variables
    
    private const float _triggerThreshold = 0.2f;
    private MLInput.Controller _controller;
    private string[] text;

    #endregion

    
    public bool _triggerPressed = false;
    public bool _bumperPressed = false;
    public bool _homePressed = false;
    private LogToConsoleHelper consoler = new LogToConsoleHelper();

    #region Unity Methods
    void Start()
    {
        MLInput.Start();
        MLInput.OnControllerButtonDown += OnButtonDown;
        MLInput.OnControllerButtonUp += OnButtonUp;
        _controller = MLInput.GetController(MLInput.Hand.Left);
        text = new string[5];
        LogToFileHelper logger = new LogToFileHelper();
        //establishing logger for storing interactions locally as a backup
        StartCoroutine(logger.LogToFileStringArray("log_controller.json", text));
        //creating new session
        StartCoroutine(consoler.NewSession("http://"+LoggingConfig.ip_address+":" + LoggingConfig.port +"/ext/"+ LoggingConfig.api_key + "/new_session"));
    }   




    void OnButtonDown(byte controller_id, MLInput.Controller.Button button)
    {
        if (button == MLInput.Controller.Button.HomeTap)
        {
            _homePressed = true;
        }
        if (button == MLInput.Controller.Button.Bumper)
        {
            _bumperPressed = true;
        }
        
    }

    void OnButtonUp(byte controllerId, MLInput.Controller.Button button) {
        if (button == MLInput.Controller.Button.HomeTap)
        {
           _homePressed = false;
        }
        if (button == MLInput.Controller.Button.Bumper)
        {
            _bumperPressed = false;
        }
    }
    

    void Update () {
        transform.position = _controller.Position;
        transform.rotation = _controller.Orientation;    

        //if trigger value goes above the threshold, it was pressed
        if (_controller.TriggerValue > _triggerThreshold) {
            
            try{
                //if trigger was then released
                if(_triggerPressed == false){
                    LogToConsoleHelper.jsn_sent j = new LogToConsoleHelper.jsn_sent();
                    j.entry_id = 1;
			        j.message_data = "BUTTON TRIGGER";
			        j.time_created = ""+System.DateTime.Now;
			        j.category = "External";
                    

                    string s = "[" + JsonUtility.ToJson(j) + "]";
                    //log pressed trigger
                    if (consoler.session_id != 0){
                        StartCoroutine(consoler.PostRequest("http://"+LoggingConfig.ip_address+":" + LoggingConfig.port +"/ext/"+ LoggingConfig.api_key+"/"+consoler.session_id, s));
                    }
                }
                
            }catch(Exception e){
                Debug.Log("ohp something went wrong");
                Debug.Log(e.ToString());
            }
            _triggerPressed = true;

        }
        //if trigger is pressed and the value is back to 0, it's not pressed
        else if (_controller.TriggerValue == 0.0f && _triggerPressed) {
            _triggerPressed = false;
        }

        if(_bumperPressed == true){
            LogToConsoleHelper.jsn_sent j = new LogToConsoleHelper.jsn_sent();
            j.entry_id = 1;
            j.message_data = "BUMPER PRESSED";
            j.time_created = ""+System.DateTime.Now;
            j.category = "External";
            

            string s = "[" + JsonUtility.ToJson(j) + "]";
            //log pressed trigger
            if (consoler.session_id != 0){
                StartCoroutine(consoler.PostRequest("http://"+LoggingConfig.ip_address+":" + LoggingConfig.port +"/ext/"+ LoggingConfig.api_key+"/"+consoler.session_id, s));
            }
        }

        if(_homePressed == true){
            LogToConsoleHelper.jsn_sent j = new LogToConsoleHelper.jsn_sent();
            j.entry_id = 1;
            j.message_data = "HOME PRESSED";
            j.time_created = ""+System.DateTime.Now;
            j.category = "External";
            

            string s = "[" + JsonUtility.ToJson(j) + "]";
            //log pressed trigger
            if (consoler.session_id != 0){
                StartCoroutine(consoler.PostRequest("http://"+LoggingConfig.ip_address+":" + LoggingConfig.port +"/ext/"+ LoggingConfig.api_key+"/"+consoler.session_id, s));
            }
        }

        text[0] = "Controller Position: " + transform.position;        
        text[1] = "Controller Rotation: " + transform.rotation;        
        text[2] = "Trigger Pressed: " + _triggerPressed;       
        text[3] = "Bumper Pressed: " + _bumperPressed; 
        text[4] = "Home Pressed: " + _homePressed;
        
    }

    void OnDestroy()
    {
        MLInput.Stop();
        MLInput.OnControllerButtonDown -= OnButtonDown;
        MLInput.OnControllerButtonUp -= OnButtonUp;
    }

    
    #endregion
    
}



}