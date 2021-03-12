using UnityEngine;
using UnityEngine.XR.MagicLeap;
using MagicLeap.Core.StarterKit;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
    
    namespace LoggerScripts{
    public class TrackAllPoses : MonoBehaviour
    {
        private const float CONFIDENCE_THRESHOLD = 0.95f;

        [Space, SerializeField, Tooltip("Flag to specify if left hand should be tracked.")]
        private bool _trackLeftHand = true;

        [SerializeField, Tooltip("Flag to specify id right hand should be tracked.")]
        private bool _trackRightHand = true;

        private string[] cur_pose;
        private LogToConsoleHelper consoler = new LogToConsoleHelper();
	    public int session_id = 0;
        private Scene scn; 
        private string sceneName;
        private bool sceneSelected = false;

        /// <summary>
        /// Calls Start on MLHandTrackingStarterKit.
        /// </summary>
        void Start()
        {
            MLResult result = MLHandTrackingStarterKit.Start();

            #if PLATFORM_LUMIN
            if (!result.IsOk)
            {
                Debug.LogErrorFormat("Error: KeyPoseVisualizer failed on MLHandTrackingStarterKit.Start, disabling script. Reason: {0}", result);
                enabled = false;
                return;
            }
            #endif
            cur_pose = new string[2];
            //cur_pose[0] = "def";
            LogToFileHelper logger = new LogToFileHelper();

            scn = SceneManager.GetActiveScene();
            sceneName = scn.name;

            //establishing logger for storing interactions locally as a backup
            StartCoroutine(logger.LogToFileStringArray("log_poses.json", cur_pose));
            //creating new session
            StartCoroutine(consoler.NewSession("http://"+LoggingConfig.ip_address+":" + LoggingConfig.port +"/ext/"+ LoggingConfig.api_key + "/new_session"));
            StartCoroutine(checkHands());
            
        }


        /// <summary>
        /// Clean up.
        /// </summary>
        void OnDestroy()
        {
            MLHandTrackingStarterKit.Stop();
        }

        /// <summary>
        /// Updates color of sprite renderer material based on confidence of the KeyPose.
        /// </summary>
        void Update()
        {
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
        }


        IEnumerator checkHands(){

            while (true) {

                

                float confidenceLeft =  0.0f;
                float confidenceRight = 0.0f;

                if (_trackLeftHand)
                {
                    #if PLATFORM_LUMIN
                    confidenceLeft = GetKeyPoseConfidence(MLHandTrackingStarterKit.Left);
                    #endif
                }

                if (_trackRightHand)
                {
                    #if PLATFORM_LUMIN
                    confidenceRight = GetKeyPoseConfidence(MLHandTrackingStarterKit.Right);
                    #endif
                }

                float confidenceValue = Mathf.Max(confidenceLeft, confidenceRight);
                PostPoseToConsole(cur_pose[0]);
                yield return new WaitForSeconds(LoggingConfig.frequency);
                
            }
        }

        void PostPoseToConsole(string pose){
            try{
                LogToConsoleHelper.jsn_sent j = new LogToConsoleHelper.jsn_sent();
                 j.entry_id = 1;
			    j.message_data = "Pose is " + pose;
			    j.time_created = ""+System.DateTime.Now;
			    j.category = "External";
                
                 
                string s = "[" + JsonUtility.ToJson(j) + "]";
                //post poses to console
                if (consoler.session_id != 0){
                    StartCoroutine(consoler.PostRequest("http://"+LoggingConfig.ip_address+":" + LoggingConfig.port +"/ext/"+ LoggingConfig.api_key+"/"+consoler.session_id, s));
                }
                cur_pose[1] = consoler.receivedTextPost;
            }
            catch (Exception e){
                cur_pose[1] = e.ToString();
            }
        }

        #if PLATFORM_LUMIN
        /// <summary>
        /// Gets the confidence value for the hand being tracked.
        /// </summary>
        /// <param name="hand">Hand to check the confidence value on.</param>
        private float GetKeyPoseConfidence(MLHandTracking.Hand hand)
        {
            
            if (hand != null)
            {
                switch (hand.KeyPose){
                    case MLHandTracking.HandKeyPose.Finger:{
                        cur_pose[0] = "Finger";
                        break;
                    }
                    case MLHandTracking.HandKeyPose.Pinch:{
                        cur_pose[0] = "Pinch";
                        break;
                    }
                    case MLHandTracking.HandKeyPose.Thumb:{
                        cur_pose[0] = "Thumb";
                        break;
                    }
                    case MLHandTracking.HandKeyPose.L:{
                        cur_pose[0] = "L";
                        break;
                    }
                    case MLHandTracking.HandKeyPose.OpenHand:{
                        cur_pose[0] = "Open Hand";
                        break;
                    }
                    case MLHandTracking.HandKeyPose.Ok:{
                        cur_pose[0] = "Ok";
                        break;
                    }
                    case MLHandTracking.HandKeyPose.C:{
                        cur_pose[0] = "C";
                        break;
                    }
                    case MLHandTracking.HandKeyPose.NoPose:{
                        cur_pose[0] = "No Pose";
                        break;
                    }
                }
            }
            
            PostPoseToConsole(cur_pose[0]);
            return hand.HandKeyPoseConfidence;
        }
        #endif
    }
    

    }