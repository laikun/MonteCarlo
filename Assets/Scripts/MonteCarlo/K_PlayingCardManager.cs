using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// 게임에 쓰이는 카드를 생성
/// </summary>
public class K_PlayingCardManager : MonoSingleton<K_PlayingCardManager>
{
    private List<K_PlayingCard> cards = new List<K_PlayingCard>();
    public K_PlayingCard[] Cards { get { return cards.ToArray(); } }
    
    public Vector2 CardSize
    {
        get
        {
            if (cards.Count == 0)
                throw new Exception("No Cards!");
            else
                return cards [0].Size;
        }
    }

    object _lock = new object();

    public override void OnInitialize()
    {
        lock (_lock)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/K_PlayingCard");

            foreach (Texture texture in Resources.LoadAll<Texture>("Images/PlayingCards"))
            {
                Transform tr = transform.FindChild(texture.name);

                if (tr != null)
                    continue;

                GameObject card = Instantiate(prefab) as GameObject;
                card.name = texture.name;
                card.transform.parent = transform;
                K_PlayingCard pc = card.transform.GetOrAddComponent<K_PlayingCard>();
                pc.SetCard(texture.name.First().ToString(), int.Parse(Regex.Match(texture.name, @"\d{2}").Value), texture);
                cards.Add(pc);
                card.StageOut();
            }
        }
    }
}