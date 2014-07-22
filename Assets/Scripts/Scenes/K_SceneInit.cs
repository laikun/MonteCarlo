using UnityEngine;
using System.Collections;

public class K_SceneInit : MonoSingleton<K_SceneInit>
{
    // Use this for initialization
    void Start()
    {
        StartCoroutine(Go());
    }

    IEnumerator Go()
    {
        yield return new WaitForSeconds(1f);
        K_SceneManager.SwitchScene("Title");
        K_FB.Instance.FBdigest(() => { });
    }
}
