using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class K_CameraManager : MonoSingleton<K_CameraManager> {

    private UICamera uiCamera;
    private UICamera eventCamera;
    private Dictionary<UICamera, bool> activative = new Dictionary<UICamera, bool>(2);

    public override void OnInitialize()
    {
        uiCamera = GameObject.FindObjectsOfType<UICamera>().FirstOrDefault(x => x.tag.Equals("UICamera"));
        eventCamera = Camera.main.GetComponent<UICamera>();
    }

    public void AllEventActive(bool sw)
    {
        if (!sw)
        {
            activative = new Dictionary<UICamera, bool>(2);
            GameObject.FindObjectsOfType<UICamera>().ForEach(x => {
                activative.Add(x, x.enabled);
                x.enabled =false;
                });
        }
        else
        {
            activative.Keys.ForEach(x => x.enabled = activative[x]);
        }
    }

    public bool UIActive(bool sw)
    {
        if (uiCamera == null)
            return false;

        uiCamera.enabled = sw;

        return true;
    }

    public bool EventActive(bool sw)
    {
        if (eventCamera == null)
            return false;

        eventCamera.enabled = sw;

        return true;
    }
}
