using UnityEngine;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

public class K_Rule : MonoSingleton<K_Rule>
{
    bool autoDraw;
    bool pokerRule;

    public override void OnInitialize()
    {
        K_Flag.Reset("InPlay");
        K_Flag.SetHandler("InPlay", f => K_CameraManager.Instance.EventActive(f == 1));
    }

    #region Ready To Game

    /// <summary>
    /// 새로운 게임을 준비
    /// </summary>
    public void NewGame()
    {
        this.Log("NewGame");

        K_CameraManager.Instance.EventActive(false);

        pokerRule = K_OptionData.Get<int>("PokerRule") == 1;
        autoDraw = K_OptionData.Get<int>("AutoDraw") == 1;

        // cell, deck 초기화
        K_Cell.Instance.OnInitialize();
        K_Deck.Instance.OnInitialize();
        K_Foundation.Instance.OnInitialize();
        K_Score.Instance.OnInitialize();
        K_CountDown.Instance.OnInitialize();
        K_ElapsedTime.Instance.OnInitialize();

        // 카드 이벤트 리셋
        K_PlayingCardManager.Instance.Cards.ForEach(c =>
        {
            c.Anim.Play("Closed");
            UIEventListener.Get(c.gameObject).onClick = null;
        });

        K_Cell.Instance.LoadToCell();
        K_Cell.Instance.Cards.ForEach(c => K_Cell.Instance.SendMessage("goToCell", c));
        K_Flag.On("InPlay", false);

        findCombination();

        K_SubWindow.Instance.PopUp("Ready");
        K_CameraManager.Instance.UIActive(true);
    }
    #endregion

    /// <summary>
    /// 게임 플레이 시작
    /// </summary>
    /// <returns></returns>
    public void Play()
    {
        this.Log("Play");

        K_SubWindow.Instance.Disapear();
        K_PlayingCardManager.Instance.Cards.ForEach(c => c.Anim.Play("Open"));
        K_Deck.Instance.SendMessage("ShowNextCard");

        // 게임오버체크
        if (combinations.Count() < 1)
        {
            StartCoroutine("GameOver");
            return;
        }

        // 카드 이벤트 등록
        K_Cell.Instance.Cards.ForEach(c => UIEventListener.Get(c.gameObject).onClick = g => K_Rule.Instance.Select(c));
        K_Deck.Instance.Cards.Take(3).ForEach(c => UIEventListener.Get(c.gameObject).onClick = g => K_Rule.Instance.SendMessage("Draw", c));

        K_CountDown.Instance.Go();
        K_ElapsedTime.Instance.Go();

        K_Flag.On("InPlay", true);
    }

    #region Enable Combinations
    Dictionary<IEnumerable<K_PlayingCard>, string> combinations = new Dictionary<IEnumerable<K_PlayingCard>, string>();
    /// <summary>
    ///     /// 현재 셀에 있는 카드 중에서 가능한 조합들을 찾아 내고, 찾아내지 못할 경우 게임 오버 실행
    /// - 포커룰을 적용할 경우 처리 부담이 높음으로 코루틴형식으로 실행
    /// - 여전히 부담이 크고 잦은 호출이 필요함으로 대상을 셀 전체 혹은 전의 함수 실행 후 남은 카드로 함
    /// - 다시태어남
    /// </summary>
    /// <param name="mode">코루틴 - 일반 모드</param>
    /// <returns>bool</returns>
    bool findCombination()
    {
        K_Report.Log("<b>" + name + "</b> : findCombination ");

        var watch = System.Diagnostics.Stopwatch.StartNew();

        combinations.Clear();

        // 포커룰 혹은 일반룰
        int max = pokerRule ? Mathf.Max(K_OptionData.Get<int>("Column"), K_OptionData.Get<int>("Row")) : 2;

        System.Text.StringBuilder log = new System.Text.StringBuilder("<color=blue><b>" + name + "</b> : findCombination</color> \n");
        log.Append("<color=blue>");
        // 직선상의 카드들을 취득
        foreach (var item in K_Cell.Instance.GetCardsInLinear())
        {
            log.Append(item.ToArray().ToString<MonoBehaviour>() + "\n");

            // 2장 미만은 무의미
            if (item.Count() < 2)
                continue;

            // 조합할 경우의 수
            for (int k = 2; k <= max; k++)
            {
                // 인접한 카드에서 조합취득
                for (int i = 0; i <= item.Count() - k; i++)
                {
                    IEnumerable<K_PlayingCard> combination = item.Skip(i).Take(k);
                    // null 이 포함되어 있을경우
                    if (combination.Contains(null))
                        continue;

                    string rank = PokerHand(combination);
                    if (string.IsNullOrEmpty(rank))
                        continue;

                    combinations.Add(combination.OrderBy(x => x).ToArray(), rank);
                    log.Append("<i>" + rank + " - " + combination.ToArray().ToString<K_PlayingCard>() + "</i>\n");
                }
            }
        }

        log.Append("</color>");
        K_Report.Log(log.ToString());

        watch.Stop();
        K_Report.Log("<color=purple><b>" + name + "</b> : findCombination [TimeWatch :" + watch.ElapsedMilliseconds + "]</color>");

        return combinations.Count() > 0;
    }
    #endregion

    #region selectCard
    List<K_PlayingCard> selectedCards = new List<K_PlayingCard>();

    /// <summary>
    /// 선택된 카드들의 조합체크(로직부)
    /// </summary>
    /// <param name="card">새로 선택된 카드</param>
    /// <returns></returns>
    public bool Select(K_PlayingCard card)
    {
        this.Log("Select (" + card.name + ")", "blue");

        // 파라메터 에러
        if (card == null)
            return false;

        // 이미 선택 된 카드
        if (selectedCards.Contains(card))
            return false;

        // 카드 선택 애니메이션
        card.Anim.Play("Select");
        selectedCards.Add(card);

        // 두 장 미만은 조합 무의미
        if (selectedCards.Count < 2)
            return false;

        Action<K_PlayingCard> selectReset = c =>
        {
            selectedCards.Clear();
            selectedCards.Add(card);
            K_Cell.Instance.Cards.Where(x => !x.Equals(card)).ForEach(x => x.Anim.Play("Opened"));
        };

        K_PlayingCard[] target = selectedCards.OrderBy(c => c).ToArray();
        K_Report.Log("<color=blue><b>" + name + "</b> : Select. </color>\n SelectedCards \t" + target.ToString<MonoBehaviour>());

        // 준비된 조합 포함여부 검사
        IEnumerable<IEnumerable<K_PlayingCard>> combis = combinations.Keys.Where(x => x.Intersect(target).Count() == target.Length);

        // 조합가능한 카드가 아닐때
        if (combis.Count() < 1)
        {
            K_Report.Log("<color=blue><b>" + name + "</b> : Select. </color>\n  가능한 조합이 아님 "
                + "\n SelectCards \t" + selectedCards.ToArray().ToString<MonoBehaviour>()
                + "\n Combinations \t" + string.Join(" | ", combinations.Keys.Select(x => x.ToArray().ToString<MonoBehaviour>()).ToArray()));
            selectReset(card);
            return false;
        }

        // 조합 가능한 경우의 수가 두 가지 이상이면 적은 수의 조합을 선택
        IEnumerable<K_PlayingCard> combi = new K_PlayingCard[0].Concat(combis.OrderBy(x => x.Count()).FirstOrDefault());

        // 조합에 필요한 카드가 부족할때
        if (combi.Count() != target.Length)
            return false;

        // 조합성공
        K_Report.Log("<color=blue><b>" + name + "</b> : Select. </color>\n Combination \t" + combi.ToArray().ToString<K_PlayingCard>());
        // 제한시간 정지
        K_CountDown.Instance.Stop();

        combi.ToArray().ForEach(c =>
        {
            c.Anim.Play("SetOff");
            UIEventListener.Get(c.gameObject).onClick = null;
        });
        selectedCards.Clear();
        K_Cell.Instance.Remove(combi);
        K_Score.Instance.Add(ScoreTable(combinations.First(x => K_PlayingCard.IsEqualCombination(x.Key, combi)).Value));
        K_Foundation.Instance.Founding(combi.ToArray());
        combinations.Remove(combinations.Keys.First(x => K_PlayingCard.IsEqualCombination(x, combi)));

        // 현상태
        K_Report.Log("<color=blue><b>" + name + "</b> : Select. </color>\n SelectCards \t" + selectedCards.ToArray().ToString<K_PlayingCard>()
            + "\n Combinations \t" + string.Join(" | ", combinations.Keys.Select(x => x.ToArray().ToString<K_PlayingCard>()).ToArray()));

        // 클리어 체크
        if (K_Cell.Instance.Cards.Length == 0 && K_Deck.Instance.Cards.Count() == 0)
        {
            StartCoroutine("GameClear");
            return true;
        }

        // 카드 조합을 체크
        if (!findCombination())
            K_Deck.Instance.NeedForDraw();

        // 자동 드로우
        if (autoDraw)
        {
            StartCoroutine("Draw");
            return true;
        }
        // 턴 제한 시동
        else
        {
            K_CountDown.Instance.Go();
        }

        StartCoroutine("hit");

        return true;
    }

    IEnumerator hit()
    {
        // 카드 연출이 끝날 때 까지 대기
        do
        {
            yield return null;
        } while (K_Cell.Instance.Cards.Any(c => c.IsPlaying));

        // 현재 셀 위의 카드 중 조합가능한 카드가 없을 때
        if (combinations.Count() < 1)
        {
            K_Deck.Instance.NeedForDraw();
        }

        yield break;
    }
    #endregion

    #region Draw
    public void Draw()
    {
        this.Log("Draw");

        if (K_Flag.State("InPlay") != 1)
            return;

        // 비어있는 셀이 없을 때
        if (K_Cell.Instance.BlankCellCount == 0)
            return;

        // 카운트 다운 정지
        K_CountDown.Instance.Stop();
        K_Flag.On("InPlay", false);
        // 비어있는 셀을 채움
        K_Cell.Instance.LoadToCell(K_Deck.Instance.Draws(K_Cell.Instance.BlankCellCount));
        // 셀 채움 애니메이션
        K_Cell.Instance.SendMessage("goToCell", 0.05f);
        K_Cell.Instance.Cards.ForEach(c => c.Anim.Play("Opened"));
        // 선택된 카드 리셋
        selectedCards.Clear();
        // 게임오버 체크
        if (!findCombination())
        {
            StartCoroutine("GameOver");
            return;
        }

        // 덱에 남은 카드가 있을때
        K_Deck.Instance.ShowNextCard();

        // 셀 위의 카드에 이벤트 등록
        K_Cell.Instance.Cards.ForEach(c => UIEventListener.Get(c.gameObject).onClick = g => K_Rule.Instance.Select(c));

        // 제한시간 시동
        K_CountDown.Instance.Go();
        K_Flag.On("InPlay", true);

        return;
    }
    #endregion

    #region GameOver

    void playOver()
    {
        K_Flag.On("InPlay", false);

        selectedCards.Clear();
        combinations.Clear();
        K_PlayingCardManager.Instance.Cards.ForEach(c => UIEventListener.Get(c.gameObject).onClick = null);
        K_CountDown.Instance.Stop();
        K_ElapsedTime.Instance.Stop();
    }

    IEnumerator GameClear()
    {
        playOver();

        K_SubWindow.Instance.PopUp("GameClear");
        K_Score.Instance.Record();

        yield break;
    }

    IEnumerator GameOver()
    {
        playOver();

        K_SubWindow.Instance.PopUp("GameOver");

        yield break;
    }

    IEnumerator TimeOver()
    {
        playOver();

        K_SubWindow.Instance.PopUp("TimeOver");

        yield break;
    }
    #endregion

    #region Poker Hands
    string PokerHand(IEnumerable<K_PlayingCard> cards)
    {
        if (cards.Count() < 2)
            return null;

        // Check Pair
        if (cards.Count() < 3)
            return !K_PlayingCard.IsPair(cards) ? null : "Pair";

        // Check Three of a Kind
        if (cards.Count() < 4)
            return !K_PlayingCard.IsPair(cards) ? null : "Three Of A Kind";

        // Check Two Pair or Full House
        if (cards.GroupBy(x => x.Number).Count() == 2
            && cards.Skip(1).Where(x => K_PlayingCard.IsPair(new K_PlayingCard[] { x, cards.ElementAt(0) })).Count() == 1)
            return cards.Count() == 4 ? "Two Pair" : "Full House";

        // Check Four Of A Kind
        if (cards.Count() == 4 && cards.GroupBy(x => x.Number).Count() == 1)
            return "Four Of A Kind";

        // Check Flush
        bool flush = cards.GroupBy(x => x.Suit).Count() == 1;

        // Check Straight
        if (K_PlayingCard.IsStraight(cards))
        {
            // Check Royal Flush ( only 5 card case )
            if (cards.Count() > 4 && cards.Max(x => x.Number) == 13 && cards.Min(x => x.Number) == 1)
                return "Royal Straight Flush";

            return flush ? "Straight Flush" : "Straight";
        }

        if (flush)
            return "Flush";

        return null;
    }

    int ScoreTable(string rank)
    {
        switch (rank)
        {
            case "Royal Straight Flush":
                return 23;
            case "Straight Flush":
                return 19;
            case "Four Of A Kind":
                return 17;
            case "Full House":
                return 13;
            case "Flush":
                return 11;
            case "Straight":
                return 7;
            case "Three Of A Kind":
                return 5;
            case "Two Pair":
                return 3;
            case "One Pair":
                return 1;
            case "Pair":
                return 1;
            default:
                return 0;
        }
    }
    #endregion

    #region OnGUI
    void OnGUI()
    {
        // Pair Guide
        if (combinations.Count > 0 && K_Flag.State("InPlay") == 1)
        {
            int t = -60;
            foreach (var item in combinations)
            {
                t += 15;
                foreach (var item2 in item.Key)
                {
                    Rect rect = new Rect(0, 0, 40, 20);
                    Vector2 v = Camera.main.WorldToScreenPoint(item2.transform.position);
                    rect.center = new Vector2(v.x, Screen.height - v.y + t);
                    GUI.Box(rect, item2.Number + "_" + System.Text.RegularExpressions.Regex.Match(item.Value, @"[A-Z]*").Value);
                }
            }
        }
    }
    #endregion
}
