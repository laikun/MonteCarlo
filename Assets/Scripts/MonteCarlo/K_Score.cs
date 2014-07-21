using UnityEngine;
using System.Collections;

public class K_Score : MonoSingleton<K_Score> {

    double score = 0;

    public override void OnInitialize()
    {
        this.score = 0;
        GetComponent<UILabel>().text = this.score.ToString();
    }

    public override string ToString()
    {
        return this.score.ToString();
    }

    public void Add(int score)
    {
        this.score += score;
        GetComponent<UILabel>().text = this.score.ToString();
    }
}
