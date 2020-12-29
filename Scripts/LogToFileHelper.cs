using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


namespace LoggerScripts{


public class LogToFileHelper : MonoBehaviour
{

    void SaveFile(string fileName, string data)
     {
        string destination = Application.persistentDataPath + fileName;
        if (File.Exists(destination))
        {
            //Debug.Log(destination+" already exists.");
            var sr = File.AppendText(destination);
            var s = "";
            if (fileName.Contains(".json")){
                s = "{\"data\": " + data + ", \"timestamp\": {\"" + System.DateTime.Now + "\"}}";
            }
            else {
                s = data + " --- " + System.DateTime.Now;
            }
            sr.WriteLine(s);
            sr.Close();
        }
        else {
            var sr = File.CreateText(destination);
            var s = "";
            if (fileName.Contains(".json")){
                s = "{\"data\": " + data + ", \"timestamp\": {\"" + System.DateTime.Now + "\"}}";
            }
            else {
                s = data + " --- " + System.DateTime.Now;
            }
            sr.WriteLine(s);
            sr.Close();
        }
        
        
     }

    public IEnumerator LogToFileVector3Array(string file_name, Vector3[] arr)
    {
        while (true) {
            string s = "";
            Vector3 last = arr[arr.Length - 1];
            foreach (Vector3 v in arr){
                if (v != last){s+= v + ", ";}
            }
            if (file_name.Contains(".json")){
                s = "{\"" + s + "\"}";
            }
             yield return new WaitForSeconds(LoggingConfig.frequency);
             SaveFile(file_name, s);
         }
    }

    public IEnumerator LogToFileStringArray(string file_name, string[] arr)
    {
        while (true) {
            string s = "";
            string last = arr[arr.Length - 1];
            foreach (string v in arr){
                if (v != last){s+= v + ", ";}
            }
            if (file_name.Contains(".json")){
                s = "{\"" + s + "\"}";
            }
             yield return new WaitForSeconds(LoggingConfig.frequency);
             SaveFile(file_name, s);
         }
    }
    
    public IEnumerator LogToFileString(string file_name, string str)
    {
        while (true){
            string s = "\""+str+"\"";
            yield return new WaitForSeconds(LoggingConfig.frequency);
            SaveFile(file_name, s);
        }
    }

    public IEnumerator LogToFile(string file_name, string _data)
    {
        while (true) {
             yield return new WaitForSeconds(LoggingConfig.frequency);
             SaveFile(file_name, _data);
         }
    }
}
}