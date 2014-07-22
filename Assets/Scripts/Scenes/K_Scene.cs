using UnityEngine;
using System.Collections;

public class K_Scene : MonoSingleton<K_Scene>
{
    public void Back()
    {
        K_SceneManager.PrevScene();
    }

    public void GoTitle()
    {
        K_SceneManager.SwitchScene("Title");
    }

    public void GoOption()
    {
        K_SceneManager.SwitchScene("Option");
    }

    public void GoRanking()
    {
        K_SceneManager.SwitchScene("Ranking");
    }
}
    