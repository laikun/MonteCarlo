using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class K_SubWindow : MonoSingleton<K_SubWindow> {

    Dictionary<string, GameObject> window = new Dictionary<string, GameObject>();
    string[] mode = { "Ready", "GameClear", "GameOver", "TimeOver" };

    public override void OnInitialize()
    {
        window = new Dictionary<string, GameObject>();
        this.gameObject.SetActive(true);

        mode.ForEach(s => window.Add(s, this.transform.FindChild(s).gameObject));

        this.enabled = false;
        this.gameObject.SetActive(false);
	}
	
    public void PopUp(string mode)
    {
        this.gameObject.SetActive(true);

        window.Where(w => !w.Key.Equals(mode)).ToList().ForEach(w => w.Value.SetActive(false));
        window[mode].SetActive(true);

    }

    public void Disapear()
    {
        this.gameObject.SetActive(false);
    }

    void Request(string str)
    {
        if (str.Contains("Go"))
        {
            this.Disapear();
            K_Rule.Instance.SendMessage("Play");
        }
        else if (str.Contains("OneMore"))
        {
            if (K_Coin.Instance.UseCoin())
            {
                this.Disapear();
                K_Rule.Instance.NewGame();
            }
            else
            {
                
            }
        }
        else if (str.Contains("Quit"))
        {
            K_ScenePlay.Instance.GoTitle();
        }
    }
}
