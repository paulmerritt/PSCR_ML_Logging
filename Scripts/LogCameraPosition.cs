using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
namespace LoggerScripts{
    public class LogCameraPosition : MonoBehaviour
    {
        // Start is called before the first frame update
        private string[] text;
        private LogToConsoleHelper consoler = new LogToConsoleHelper();

        public int session_id = 0;

        void Start()
        {
            text = new string[3];
            LogToFileHelper logger = new LogToFileHelper();
            //establishing logger for storing interactions locally as a backup
            StartCoroutine(logger.LogToFileStringArray("log_camera.json", text));
            //creating new session
            StartCoroutine(consoler.NewSession("http://"+LoggingConfig.ip_address+":57000/ext/"+ LoggingConfig.api_key + "/new_session"));
        }

        // Update is called once per frame
        void Update()
        {
            text[0] = "" + GameObject.Find("Main Camera").transform.position;
            text[1] = "" + GameObject.Find("Main Camera").transform.rotation;
            try{
                LogToConsoleHelper.jsn_sent j = new LogToConsoleHelper.jsn_sent();
                j.entry_id = 1;
                j.message_data = "camera position: " + text[0];
                j.time_created = ""+System.DateTime.Now;
                j.category = "External";
                string s = "[" + JsonUtility.ToJson(j) + "]";
                //post current camera position to session
                if (consoler.session_id != 0){
                        StartCoroutine(consoler.PostRequest("http://"+LoggingConfig.ip_address+":57000/ext/"+ LoggingConfig.api_key+"/"+consoler.session_id, s));
                }
                text[3] = consoler.receivedTextPost;
            }
            catch (Exception e){
                text[3] = e.ToString();
            }
        }
        IEnumerator checkCamera(){
            while (true){
                
                yield return new WaitForSeconds(1f);
            }
            
        }
    }
}