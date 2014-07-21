using UnityEngine;
using System.Collections;

public class K_ScenePlay : K_Scene
{
    public override void OnInitialize()
    {
        // debug code
        try
        {
            K_Flag.State("GameMode");
        }
        catch (System.Exception)
        {
            K_DevelopOption.Instance.OnScreen = true;
            K_Flag.On("GameMode", 3);
            K_OptionData.Set<int>("TurnLimit", 5);
        }
        //
        K_Rule.Instance.SendMessage("NewGame");
    }

    public void GoPlay()
    {
        K_Rule.Instance.SendMessage("Play");
    }

    public void GoNewGame()
    {
        K_Rule.Instance.SendMessage("NewGame");
    }
}
    