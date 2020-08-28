using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour {
    int data = 1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void hello()
    {
        Debug.Log("hello");
    }
    public void haha(int num) {
        Debug.Log(data+num);
    }
}
