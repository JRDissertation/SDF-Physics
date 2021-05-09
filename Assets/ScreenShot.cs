using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShot : MonoBehaviour {


 private int i;

    void Start () {
        i = 0;
        Time.captureFramerate = 60;
    }
 
    // Update is called once per frame
    void Update () {
        i = i + 1;
        //UnityEngine.Debug.Log(i_to_string(i));
        ScreenCapture.CaptureScreenshot ("spheres/spheres"+i_to_string(i)+".png",1);
    }
    
    public string i_to_string(int i) {
        string r;
        if (i <= 9) {
            return "00" + i.ToString();
        }
        else if (i <= 99) {
            return "0" + i.ToString();
        }
        else {
            return i.ToString();
        }
    }

}