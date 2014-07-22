using UnityEngine;
using System.Collections;

public class K_SceneInit : MonoSingleton<K_SceneInit>
{
    // Use this for initialization
    void Start()
    {
        K_FB temp = K_FB.Instance;
        StartCoroutine(Go());
    }

    IEnumerator Go()
    {
        yield return new WaitForSeconds(1f);
        K_SceneManager.SwitchScene("Title");
    }
}
