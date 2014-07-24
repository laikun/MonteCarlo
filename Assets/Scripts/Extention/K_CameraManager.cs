using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class K_CameraManager : MonoSingleton<K_CameraManager> {

    private UICamera uiCamera;
    private UICamera eventCamera;
    private Dictionary<UICamera, bool> activative = new Dictionary<UICamera, bool>(2);

    int layerStage;
    int layerGUI;

    public override void OnInitialize()
    {
        layerGUI = LayerMask.NameToLayer("GUI");
        layerStage = LayerMask.NameToLayer("Stage");

        eventCamera = Camera.main.GetComponent<UICamera>();
    }

    // 소인수분해
    IEnumerable<int> factorization(int k)
    {
        if (k < 1)
            return new int[0];
        int y = 0;
        while (Mathf.Pow(2, ++y) <= k) { }
        return new int[] { --y }.Concat(factorization(k - (int)Mathf.Pow(2, y)));
    }

    public void AllEventActive(bool sw)
    {
        IEnumerable<int> eventLayer = factorization(eventCamera.eventReceiverMask.value);

        eventCamera.eventReceiverMask.value += (sw ? 1 : -1) * (eventLayer.Contains(layerStage) ? layerStage : 0);
        eventCamera.eventReceiverMask.value += (sw ? 1 : -1) * (eventLayer.Contains(layerGUI) ? layerGUI : 0);
    }

    public void UIActive(bool sw)
    {
        IEnumerable<int> eventLayer = factorization(eventCamera.eventReceiverMask.value);
        eventCamera.eventReceiverMask.value += (sw ? 1 : -1) * (eventLayer.Contains(layerGUI) ? layerGUI : 0);
    }

    public void EventActive(bool sw)
    {
        IEnumerable<int> eventLayer = factorization(eventCamera.eventReceiverMask.value);
        eventCamera.eventReceiverMask.value += (sw ? 1 : -1) * (eventLayer.Contains(layerStage) ? layerStage : 0);
    }

    public void EventActive(string layerName, bool sw)
    {
        int layer = LayerMask.NameToLayer(layerName);

        IEnumerable<int> eventLayer = factorization(eventCamera.eventReceiverMask.value);
        eventCamera.eventReceiverMask.value += (sw ? 1 : -1) * (eventLayer.Contains(layer) ? layer : 0);
    }
}
