using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace LoggerScripts{
    public class LogCameraPosition : MonoBehaviour
    {
        private string[] headpose;
        private GameObject camera;
        private LogToConsoleHelper consoler = new LogToConsoleHelper();
        private float elapsed = 0.0f;
        private Scene scn; 
        private string sceneName;
        private bool sceneSelected = false;

        #region Unity Methods
        void Start() {
            
            camera = GameObject.FindWithTag("MainCamera");
            
            headpose = new string[2];
            LogToFileHelper logger = new LogToFileHelper();
            //establishing logger for storing interactions locally as a backup
            StartCoroutine(logger.LogToFileStringArray("log_headpose.json", headpose));
            //creating new session
            scn = SceneManager.GetActiveScene();
            sceneName = scn.name;
            StartCoroutine(consoler.NewSession("http://"+LoggingConfig.ip_address+":" + LoggingConfig.port +"/ext/"+ LoggingConfig.api_key + "/new_session"));
        }    
        private void OnDisable() {
        
        } 
        
        void Update() {

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
                    headpose[0] = "" + camera.transform.position;
                    
                    LogToConsoleHelper.jsn_sent j = new LogToConsoleHelper.jsn_sent();
                    j.entry_id = 1;
                    j.message_data = "headpose: " + headpose[0];
                    j.time_created = ""+System.DateTime.Now;
                    j.category = "External";
                    

                    string s = "[" + JsonUtility.ToJson(j) + "]";
                    //post where eyes are currently looking
                    if (consoler.session_id != 0){
                        StartCoroutine(consoler.PostRequest("http://"+LoggingConfig.ip_address+":" + LoggingConfig.port +"/ext/"+ LoggingConfig.api_key+"/"+consoler.session_id, s));
                    }
                    
                    
                
            }
        }
        #endregion
    }
}