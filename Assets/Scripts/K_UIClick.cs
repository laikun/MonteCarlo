using UnityEngine;
using System.Collections;

public class K_UIClick : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseDown()
    {
        SendMessageUpwards("Request", name, SendMessageOptions.DontRequireReceiver);
    }
}
