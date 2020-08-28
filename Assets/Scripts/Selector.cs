using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour {
    int scale = 1;
    Vector3 newTransformPosition;
	// Use this for initialization
	void Start () {

        this.transform.position = new Vector3(-3.5f, -0.5f, 0);
            
    }
	
	// Update is called once per frame
	void Update () {
        float newx = this.transform.position.x;
        float newy = this.transform.position.y;
        float newz = this.transform.position.z;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            newy = newy + scale;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            newy = newy - scale;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            newx = newx - scale;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            newx = newx + scale;
        }
        newTransformPosition = new Vector3(newx, newy, newz);
    }
    public Vector3 getNewTransformPosition() {
        return  newTransformPosition;
    }
    public void transformToNewPosition() {
        this.transform.position = newTransformPosition;
    }
}
