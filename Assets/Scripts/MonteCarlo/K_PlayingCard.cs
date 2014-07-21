using System.Collections;
using System.Linq;
using UnityEngine;
using System;
using System.Collections.Generic;

public class K_PlayingCard : K_ReadyToWork, IComparable<K_PlayingCard>
{
    #region Logic
    public string Suit { private set; get; }

    public int Number{ private set; get; }

    Vector2 size;
    public Vector2 Size
    {
        set
        {
            transform.localScale = value;
        }
        get
        {
            return size ;
        }
    }

    Animator anim;
 
    public K_PlayingCard SetCard(string str, int num, Texture texture)
    {
        this.Suit = str;
        this.Number = num;

        Transform front = Array.Find(GetComponentsInChildren<Transform>(), x => x.name.Equals("Front"));
        this.size = front.transform.localScale.V2();

        Material mat = new Material(front.renderer.sharedMaterial);
        mat.mainTexture = texture;
        front.renderer.material = mat;

        this.anim = this.transform.FindChild("Card").GetComponent<Animator>();

        return this;
    }

    public static bool IsStraight(IEnumerable<K_PlayingCard> cards)
    {
        string rank = "A234567890JQKA";
            return rank.Contains(string.Join("", cards.OrderBy(x => x.Number).Select(x => rank[x.Number - 1].ToString()).ToArray()));
    }

    public static bool IsPair(IEnumerable<K_PlayingCard> cards)
    {
        return cards.GroupBy(x => x.Number).Count() == 1;
    }

    public static bool IsEqualCombination(IEnumerable<K_PlayingCard> alpha, IEnumerable<K_PlayingCard> beta)
    {
        if (alpha.Count() != beta.Count())
            return false;

        alpha = alpha.OrderBy(x => x);
        beta = beta.OrderBy(x => x);

        for (int i = 0; i < alpha.Count(); i++)
        {
            if (alpha.ElementAt(i).CompareTo(beta.ElementAt(i)) != 0)
                return false;
        }

        return true;
    }

    public override string ToString()
    {
        return string.Format("[K_PlayingCard: Suit={0}, Number={1}]", Suit, Number);
    }
    #endregion

    public bool IsPlaying { private set; get; }

    /// <summary>
    /// 카드 애니메이션 실행
    /// Coroutine실행 필요
    /// </summary>
    /// <param name="name">name</param>
    /// <returns></returns>
    IEnumerator Play(string name)
    {
        IsPlaying = true;
        anim.Play(name);

        do
        {
            yield return null;
        } while (anim.GetCurrentAnimatorStateInfo(0).IsName(name));

        IsPlaying = false;
        yield break;
    }

    #region IComparable<K_PlayingCard> 멤버

    public int CompareTo(K_PlayingCard other)
    {
        if (Number != other.Number)
            return (Number != 1 ? Number : 14) - (other.Number != 1 ? other.Number : 14);

        return "hdcs".IndexOf(Suit) - "hdcs".IndexOf(other.Suit);
    }

    #endregion
}
