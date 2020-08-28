using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : MonoBehaviour {
    public GameObject test1;
    Test1 test1Script;
	// Use this for initialization
	void Start () {

        test1Script = test1.GetComponent<Test1>();
        //test1Script = this.test1;
        test1Script.hello();
        test1Script.haha(2);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
