using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class K_Foundation : MonoSingleton<K_Foundation>
{
    class Foundation
    {
        public K_PlayingCard card;
        public Vector2 position;
    }
    
    Foundation[] foundations;

    public K_PlayingCard[] Cards { get { return foundations.Select(found => found.card).Where(card => card != null).ToArray(); } }

    float scale;

    public override void OnInitialize()
    {
        int length = K_PlayingCardManager.Instance.Cards.Length;

        foundations = new Foundation[length];
        float unitx = this.transform.localScale.x / length;

        for (int i = 0; i < foundations.Length; i++)
        {
            foundations [i] = new Foundation();
            foundations [i].card = null;
            foundations [i].position = new Vector2((2 * i + 1) * unitx / 2 - this.transform.localScale.x / 2 - Math.Abs(this.transform.position.x), this.transform.position.y);
        }

        scale = transform.localScale.y / length;
    }

    public void Clear()
    {
        if (foundations == null)
            return;

        Array.ForEach(foundations, x => x.card = null);
    }

    public void Founding(K_PlayingCard[] cards)
    {
        cards = cards.OrderBy(x => x.Number).OrderBy(x => x.Suit).ToArray();
        Array.ForEach(cards, card => {
            foundations.First(found => found.card == null).card = card;
            StartCoroutine(founding(card));
        });
    }

    IEnumerator founding(K_PlayingCard card) {

        Vector3[] p = {card.transform.position, 
            new Vector3(-Camera.main.orthographicSize, card.transform.position.y, card.transform.position.z),
            new Vector3(Camera.main.orthographicSize, this.transform.position.y, -0.0001f * Array.IndexOf(foundations, card)),
            Array.Find(foundations, found => found.card.Equals(card)).position
        };

        yield return StartCoroutine(card.WorkLoop(f => card.transform.position = Vector3.Lerp(p[0], p[1], f), K_TimeCurve.EaseIn(Vector3.Distance(p[0], p[1]) * 0.3f)));

//        card.transform.localScale = new Vector3(card.transform.localScale.x * scale, card.transform.localScale.y * scale, 1f);
        card.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        card.Size = new Vector2(scale, scale);

        yield return StartCoroutine(card.WorkLoop(f => card.transform.position = Vector3.Lerp(p[2], p[3], f), K_TimeCurve.EaseIn(Vector3.Distance(p[2], p[3]) * 0.3f)));
    }
}
