using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 게임 스코어 기록
/// </summary>
public class K_Score : MonoSingleton<K_Score> {

    // 현제 표시되는 스코어
    int score = 0;
    // 플레이어 하이스코어
    K_FB.UserScore player;
    // 하이스코어유저
    K_FB.UserScore highUser;

    public override void OnInitialize()
    {
        this.score = 0;
        GetComponent<UILabel>().text = this.score.ToString();

        Func<FBResult, K_FB.UserScore> data = r =>
        {
            var d = (Facebook.MiniJSON.Json.Deserialize(r.Text) as Dictionary<string, object>)["data"] as List<object>;
            var u = d.OrderByDescending(x => (x as Dictionary<string, object>)["score"]).Take(1) as Dictionary<string, object>;
            if (u != null)
                return new K_FB.UserScore(u);
            else 
                return null;
        };

        // 게임 하이스코어 취득
        K_FB.Instance.FBdigest(() => FB.API(FB.AppId + "/scores", Facebook.HttpMethod.GET, r => highUser = data(r)));
        // 플레이어 하이스코어 취득
        K_FB.Instance.FBdigest(() => FB.API(FB.UserId + "/scores", Facebook.HttpMethod.GET, r => player = data(r)));
    }

    public void Add(int score)
    {
        this.Log("Add => " + score);
        this.score += score;
        GetComponent<UILabel>().text = this.score.ToString();
    }

    // Create or update a score
    public void Record()
    {
        this.Log("Record => " + this.score);

        if (player != null && this.score < player.score)
        {
            this.Log("Record - Not Recorded");
            return;
        }

        var scoreData = new Dictionary<string, string>() { { "score", this.score.ToString() } };

        K_FB.Instance.FBdigest(() => FB.API("/me/scores", Facebook.HttpMethod.POST, r => this.FBlog("Record - Score Recorded"), scoreData));
    }
}
