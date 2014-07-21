using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class K_Deck : MonoSingleton<K_Deck>
{
    Queue<K_PlayingCard> cardsInDeck = new Queue<K_PlayingCard>();

    public K_PlayingCard[] Cards { get { return cardsInDeck.ToArray(); } }

    Animator anim;

    public override void OnInitialize()
    {
        anim = GetComponent<Animator>();
        anim.Play("Opened");

        cardsInDeck.Clear();

        K_PlayingCard[] cards = K_PlayingCardManager.Instance.Cards;
        cards.Shuffle();

        K_Report.Log("<color=blue><b>" + name + "</b> : Deck Shuffle</color> \n" + cards.ToString<MonoBehaviour>());

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].transform.position = new Vector3(transform.position.x, transform.position.y, (cards.Length - i) * -0.0001f);
            cards[i].gameObject.StageIn();
        }

        cardsInDeck = new Queue<K_PlayingCard>(cards);

        this.transform.localScale = cards[0].transform.localScale.x * K_PlayingCardManager.Instance.CardSize;
        UIEventListener.Get(this.gameObject).onClick = null;
    }

    public void Clear()
    {
        cardsInDeck.Clear();
        gameObject.StageOut();
        UIEventListener.Get(this.gameObject).onClick = null;
    }

    public IEnumerable<K_PlayingCard> Draws(int count)
    {
        K_PlayingCard[] cards = new K_PlayingCard[Mathf.Min(cardsInDeck.Count(), count)];
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i] = cardsInDeck.Dequeue();
        }

        K_Report.Log("<color=blue><b>" + name + "</b> : Least Of Deck </color> \n" + cards.ToString<MonoBehaviour>());
        return cards;
    }

    public void NeedForDraw()
    {
        if (K_OptionData.Get<int>("AutoDraw") == 1)
            return;

        if (cardsInDeck.Count() < 1)
        {
            anim.Play("PickMe");
            UIEventListener.Get(this.gameObject).onClick = g =>
            {
                anim.Play("Opened");
                K_Rule.Instance.SendMessage("Draw");
            };
        }
        else
        {
            GameObject go = cardsInDeck.First().gameObject;
            go.SendMessage("Play", "PickMe");
            UIEventListener.Get(cardsInDeck.First().gameObject).onClick = g =>
            {
                g.SendMessage("Play", "Opened");
                K_Rule.Instance.SendMessage("Draw");
            };
        }
    }
    ////////////////////////////////////////////////////////

    public void ShowNextCard()
    {
        K_Report.Log("<color=blue><b>" + name + "</b> : ShowNextCard</color> \n");

        int r = Mathf.Min(2, cardsInDeck.Count());
        for (int i = 0; i < r; i++)
        {
            K_PlayingCard card = cardsInDeck.ElementAt(i);

            UIEventListener.Get(card.gameObject).onClick = g => K_Rule.Instance.Draw();

            card.AddPosition(new Vector3(transform.position.x, transform.position.y - (r - i) * 0.12f, card.transform.position.z), 1f, NormalCurve.Ease.In);
            card.GoWork();
        }

        if (cardsInDeck.Count() < 1)
            UIEventListener.Get(gameObject).onClick = g => K_Rule.Instance.Draw();
    }

    // ReDeck
    IEnumerator Shuffle(K_PlayingCard[] cards)
    {
        K_Report.Log(name + " : Shuffle");

        //Func<float, float, float> r = (n, m) => UnityEngine.Random.Range(n, m);
        //Action<K_PlayingCard, K_Deck> w = (c, d) =>
        //{
        //    Vector2 op = c.GetXY() != Vector2.zero ? c.GetXY() : new Vector2(r(-0.5f, 0.5f), r(-0.5f, 0.5f));
        //    float distance = Vector2.Distance(op, d.transform.position);
        //    float theta = Mathf.Atan2(op.y, op.x);
        //    Vector3 beta = new Vector3(Mathf.Cos(theta) * distance * r(1.1f, 1.4f) + r(-0.3f, 0.3f),
        //                               Mathf.Sin(theta) * distance * r(1.1f, 1.4f) + r(-0.3f, 0.3f),
        //                               c.transform.position.z);
        //    Func<Vector3> s = () => c.transform.position;
        //    Action<Vector3, Vector3, float> loop = (f, t, v) => c.transform.position = Vector3.Lerp(f, t, v);
        //    K_ReadyToWork_V1.TweenAnimate(c.RTW, s, loop, () => beta, s, K_TimeCurve.EaseOut(r(0.3f, 0.4f)));
        //    c.RTW.Delay(r(0.0f, 0.1f));
        //    K_ReadyToWork_V1.TweenAnimate(c.RTW, s, loop, () => new Vector3(this.transform.position.x, this.transform.position.y, c.transform.position.z), s, K_TimeCurve.EaseIn(r(0.3f, 0.4f)));
        //};

        //yield return StartCoroutine(this.WorkLoop(x => transform.Translate(Vector3.up * x), 
        //                                          K_TimeCurve.EaseIn(0.3f), 
        //                                          () => transform.position.y < Screen.width / 1.5f));
        //this.SetXY(new Vector3(0f, transform.position.y));
        //yield return StartCoroutine(this.WorkLoop(x => transform.Translate(Vector3.down * x), 
        //                                          K_TimeCurve.EaseIn(0.3f), 
        //                                          () => transform.position.y > 0f));
        //Array.ForEach(this._cards.ToArray(), card => {
        //    card.SetXY(this.GetXY());
        //    card.gameObject.StageIn();});
        //RTW.MoreWork(x => Array.ForEach(cards, card =>
        //{
        //    card.RTW.WorkGone();
        //    w(card, this);
        //    card.RTW.MoreWork(y => y.SendMessage("unselect"));
        //    card.RTW.ForceWork();
        //}));
        //yield return new WaitForSeconds(1f);
        yield break;
    }
}