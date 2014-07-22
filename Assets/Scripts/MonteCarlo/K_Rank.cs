using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class K_Rank : MonoSingleton<K_Rank> {

    // 스코어 취득
    public override void OnInitialize()
    {
        this.FBlog("getScroe");
        // 게임 전체의 스코어 취득
        K_FB.Instance.FBdigest(() => FB.API(FB.AppId + "/scores", Facebook.HttpMethod.GET, setRank));
    }

    void setRank(FBResult result)
    {
        this.FBlog("setRank " + result.Error);

        var data = (List<object>)(Facebook.MiniJSON.Json.Deserialize(result.Text) as Dictionary<string, object>)["data"];

        GameObject nameplate = Resources.Load<GameObject>("Prefabs/K_nameplate");

        int rank = 0;
        foreach (Dictionary<string, object> item in data.OrderByDescending(x => (x as Dictionary<string, object>)["score"]).Take(20))
	    {
            K_FB.UserScore userScore = new K_FB.UserScore(item);

            GameObject plate = Instantiate(nameplate, transform.position, Quaternion.identity) as GameObject;
            plate.transform.parent = this.transform;
            plate.name = (++rank).ToString(@"00");
            plate.transform.localScale = Vector3.one;

            plate.transform.GetOrAddComponent<K_NamePlate>().Set(userScore.id, userScore.name, userScore.score);
        }

        GetComponent<UIGrid>().repositionNow = true;
    }
}
