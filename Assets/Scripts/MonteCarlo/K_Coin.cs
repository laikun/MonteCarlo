using System;
using UnityEngine;
using System.Collections;

// 코인 한도는 서버에 저장해서 가져올 필요성 있음 즉, 막대한 수정이 필요함....
public class K_Coin : MonoSingleton<K_Coin>
{
    int coin;
    bool noLimit;

    public override string ToString()
    {
        return noLimit ? "" : "<b>" + new string('\u2665', coin) + "</b>";
    }

    public override void OnInitialize()
    {
        coin = K_OptionData.Get<int>("Coin", 5);
        noLimit = coin == 0;
        GetComponent<UILabel>().text = noLimit ? "" : new string('\u2665', coin);
    }

    public bool UseCoin(){
        if (coin > 0) {
            K_OptionData.Set<int>("Coin", coin);
            GetComponent<UILabel>().text = noLimit ? "" : new string('\u2665', coin);
            return true;
        } else {
            return false;
        }
    }
}
