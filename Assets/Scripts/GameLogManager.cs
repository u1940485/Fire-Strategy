using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogManager : MonoBehaviour {

    List<string> logs;
    public Text logText;
    int index;
    
	// Use this for initialization
	void Start () {
        logs = new List<string>();
        index = 0;
        //logText.text = "None";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void storeLog(string log)
    {

        logs.Add(log);
        index = logs.Count - 1;
        logText.text = logs[index];

    }
    public void previousLog()
    {
        if (index > 0) {
            index--;
            logText.text = logs[index];
        }
    }
    public void nextLog()
    {
        if (index < logs.Count - 1) {
            index++;
            logText.text = logs[index];
        }

    }
}
