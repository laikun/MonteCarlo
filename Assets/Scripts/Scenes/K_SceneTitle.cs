using UnityEngine;
using System.Collections;

public class K_SceneTitle : K_Scene
{
    public void GoStandard()
    {
        K_Flag.On("GameMode", 1);
        K_SceneManager.SwitchScene("Play");
    }

    public void GoTimeAttackMode()
    {
        K_Flag.On("GameMode", 2);
        K_SceneManager.SwitchScene("Play");
    }

    public void GoUltimate()
    {
        K_Flag.On("GameMode", 3);
        K_SceneManager.SwitchScene("Play");
    }
    
    void Start()
    {
        K_DevelopOption.Instance.OnScreen = true;
    }
}
