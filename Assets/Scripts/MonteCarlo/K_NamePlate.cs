using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class K_NamePlate : MonoBehaviour {

    public void Set(string id, string name, int score)
    {
        this.FBlog("Set (" + id + ", " + name + ", " + score + ")");

        foreach (var item in GetComponentsInChildren<UILabel>())
        {
            if (item.name.Contains("id"))
                item.text = id;
            else if (item.name.Contains("name"))
                item.text = name;
            else if (item.name.Contains("score"))
                item.text = score.ToString();
        }

        Dictionary<string, string> modifiers = new Dictionary<string, string>();
        modifiers.Add("type", "square");
        modifiers.Add("height", "80");
        modifiers.Add("width", "80");

        FB.API(id + "/picture", Facebook.HttpMethod.GET, getUserPicture, modifiers);
    }

    void getUserPicture(FBResult result)
    {
        this.FBlog("getUserPicture => " + result.Error + "\n");
        GetComponentInChildren<UITexture>().mainTexture = result.Texture;
    }
}
