using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class K_GUITextInSprite : MonoBehaviour {

    GUIText txt;

	// Use this for initialization
	void Start () {
        txt = GetComponentInChildren<GUIText>();
        txt.transform.position = Camera.main.WorldToViewportPoint(transform.position);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        txt.transform.position = Camera.main.WorldToViewportPoint(transform.position);
    }
}
