using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LoggerScripts {

public class LoggingConfig : MonoBehaviour
{

    public string _ip_address = "";
    public string _api_key = "";
    public string _port = "";
    public float _frequency_in_seconds = 0.0f;


    public static string ip_address = "192.168.0.163";
    public static string api_key = "pmerritt35c9049edc314a699e8937b1738f5707";

    
    public static string port = "61000";
    public static float frequency = .5f;

    void Start(){
        if (_ip_address != ""){
            ip_address = _ip_address;
        }
        if (_api_key != ""){
            api_key = _api_key;
        }
        if (_port != ""){
            port = _port;
        }
        if (_frequency_in_seconds > 0.0f){
            frequency = _frequency_in_seconds;
        }

    }
}

}