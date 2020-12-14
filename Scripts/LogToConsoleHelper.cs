using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Networking;
using System.Text;
using UnityEngine.Assertions;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace LoggerScripts{
public class LogToConsoleHelper : MonoBehaviour
{
	public int session_id = 0;

	//public string responseheader = "";
	//public string ishttperror = "";
	public string receivedTextPost = "";
	public string receivedTextGet = "";
	public string receivedTextSession = "";
	public bool isReady = true;
	



    IEnumerator GetRequest(string url)
    {
	
         var uwr = new UnityWebRequest(url, "GET");
	     uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
	     uwr.SetRequestHeader("Content-Type", "application/json");

	     //Send the request then wait here until it returns
	     yield return uwr.SendWebRequest();
	     if (uwr.isNetworkError)
	     {
	         Debug.Log("Error While Sending: " + uwr.error);
			 
	     }
	     else
	     {
	         
			 var json = JsonConvert.DeserializeObject<List<jsn_received>>(uwr.downloadHandler.text);
			 try{
				receivedTextGet = json.FirstOrDefault().message_data;
				
			 }
			 catch(NullReferenceException e){
				 Debug.Log("No new alerts to obtain");
				 Debug.Log(e.ToString());
				 receivedTextGet = "";
			 }
			
	     }
		 
		 uwr.Dispose();
		 
    }
	
	public IEnumerator NewSession(string url){
		yield return new WaitForSeconds(0f);
		var uwr = new UnityWebRequest(url, "POST");
		uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
		uwr.SetRequestHeader("Content-Type", "application/json");
		
		
		uwr.SendWebRequest();
		
		yield return new WaitForSeconds(3f); //it takes about 1-3 seconds for the session_id to come back from the server
		
		receivedTextSession = uwr.downloadHandler.text;
		if (receivedTextSession != ""){
				uwr.Dispose();
				
				
			}
			else if (uwr.isNetworkError)
			{
			Debug.Log("Error While Sending: " + uwr.error);
			}
		
		session_id = GetSessionID(receivedTextSession);
		

		//TestIfNewSession();
		 
		yield break;
	}

	public IEnumerator NewSessionWithName(string url, string json){
		yield return new WaitForSeconds(0f);
		var uwr = new UnityWebRequest(url, "POST");
		byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
	    uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);

		uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
		uwr.SetRequestHeader("Content-Type", "application/json");
		
		
		uwr.SendWebRequest();
		
		yield return new WaitForSeconds(3f); //it takes about 1-3 seconds for the session_id to come back from the server
		
		receivedTextSession = uwr.downloadHandler.text;
		if (receivedTextSession != ""){
				uwr.Dispose();
				
				
			}
			else if (uwr.isNetworkError)
			{
			Debug.Log("Error While Sending: " + uwr.error);
			}
		
		session_id = GetSessionID(receivedTextSession);
		

		//TestIfNewSession();
		 
		yield break;
	}

	public IEnumerator PostRequest(string url, string json)
	 {
		 //isReady = false;
	     var uwr = new UnityWebRequest(url, "POST");
	     byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
	     uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
	     uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
	     uwr.SetRequestHeader("Content-Type", "application/json");

	     //Send the request then wait here until it returns
	     uwr.SendWebRequest();

	     if (uwr.isNetworkError)
	     {
	         Debug.Log("Error While Sending: " + uwr.error);
			 receivedTextPost = "Error While Sending: " + uwr.error;
	     }
	     else
	     {
			 Debug.Log(uwr.downloadHandler.text);
			 receivedTextPost = uwr.downloadHandler.text;
			 
	     }
		 //ishttperror = "" + uwr.isHttpError;
		 //responseheader = "{" + string.Join(",", uwr.GetResponseHeaders().Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
		 uwr.Dispose();

		//TestIfPosted();
		//isReady = true;
		
		yield break;
	 }
// 	  string ToDebugString<TKey, TValue> (this IDictionary<TKey, TValue> dictionary)
// {
//     return "{" + string.Join(",", dictionary.Select(kv => kv.Key + "=" + kv.Value).ToArray()) + "}";
// }

	 void TestIfReceived(){
		 Debug.Log("Testing if able to receive alerts from session (restart office fire)");
			if (receivedTextGet != ""){
				Debug.Log("Test passed, received alerts: " + receivedTextGet);
			}
			else {
				Debug.Log("Test failed, unable receive new alerts");
				
			}
	 }
	 void TestIfNewSession(){
		Debug.Log("Testing if new session was created");
		if (receivedTextSession != "" && session_id != 0){
			Debug.Log("passed, new session is " + session_id);
		}
		else {
			Debug.Log("unable to create new session");
		}
		
	}

	 void TestIfPosted(){
		Debug.Log("Testing if posted to session");
		if (receivedTextPost != ""){
			Debug.Log("Test passed, posted to session and received: " + receivedTextPost);
		}
		else {
			Debug.Log("Test failed, unable to post to session");
			
		}
	 }

	 int GetSessionID(string s){
		 int id = session_id;
		 if (s.Contains("New Session ID")){
				 List<int> indices = new List<int>();
				 indices = FindEachIndex('\'', s);
				 id = int.Parse(s.Substring(indices[0], indices[1]-indices[0]-1));
			}
			else {
				
			}
			return id;
	 }

	 List<int> FindEachIndex(char c, string s){
		 int i = 0;
		 List<int> result = new List<int>();
		  foreach (char cc in s)
		{
			i++;
			if (c == cc){
				result.Add(i);
			}
		}
		return result;
	 }

    public class jsn_sent{
    	public int entry_id;
		public string session_name;
		public string time_created;
		public string category;
		public string message_data;
    	
    }
	public class jsn_received{
		public int entry_id;
		public string session_name;
		public string time_created;
		public string category;
		public string message_data;
	}

}
}