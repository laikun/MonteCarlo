using UnityEngine;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

public class K_Cell : MonoSingleton<K_Cell>
{
    int column = 4;
    int row = 4;

    class Cell
    {
        public K_PlayingCard card;
        public Vector2 coordination;
        public Vector2 position;
    }
    Cell[] cells;

    public K_PlayingCard[] Cards { get { return cells.Select(cell => cell.card).Where(card => card != null).ToArray(); } }

    public int BlankCellCount { get { return cells.Count(cell => cell.card == null); } }

    #region logic
    public override void OnInitialize()
    {
        column = K_OptionData.Get<int>("Column", 4);
        row = K_OptionData.Get<int>("Row", 4);

        float[] px = new float[column];
        float[] py = new float[row];
        Vector2 cellSize = transform.localScale;
        Vector2 cardSize = K_PlayingCardManager.Instance.CardSize;
        Vector2 separate = K_OptionData.Get<float>("Separate", 0.15f) *cardSize;

        Vector2 unit = new Vector2((transform.localScale.x - (column + 1) * separate.x) / column, (cellSize.y - (row + 1) * separate.y) / row);
        float scale = Math.Min(unit.x / cardSize.x, unit.y / cardSize.y);

        Array.ForEach(K_PlayingCardManager.Instance.Cards, x => x.Size = new Vector2(scale, scale));

        Vector3 zp = new Vector3(this.transform.position.x - cellSize.x / 2 + separate.x, cellSize.y / 2 + this.transform.position.y - separate.y);

        for (int i = 0; i < column; i++)
        {
            px[i] = (2 * i + 1) * unit.x / 2 + separate.x * i + zp.x;
        }
        for (int i = 0; i < row; i++)
        {
            py[i] = ((2 * i + 1) * unit.y / 2 + separate.y * i) * -1 + zp.y;
        }

        cells = new Cell[row * column];

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = new Cell();
            cells[i].card = null;
            cells[i].coordination = new Vector2(i % column, i / column);
            cells[i].position = new Vector2(px[(int)cells[i].coordination.x], py[(int)cells[i].coordination.y]);
        }
    }

    public void LoadToCell(IEnumerable<K_PlayingCard> cards = null)
    {
        K_Report.Log(name + " : LoadToCell");

        if (cards == null)
            cards = K_Deck.Instance.Draws(cells.Count(x => x.card == null));

        cards = new K_PlayingCard[0].Concat(cells.Where(x => x.card != null).Select(x => x.card)).Concat(cards);
        cards = cards.Concat(new K_PlayingCard[row * column - cards.Count()]);
        cards.ForEach((c, i) => cells[i].card = c);
    }

    public void Remove(K_PlayingCard card)
    {
        cells.First(c => c.card != null && c.card.Equals(card)).card = null;
    }

    public void Remove(IEnumerable<K_PlayingCard> cards)
    {
        cards.ForEach(card => this.Remove(card as K_PlayingCard));
    }

    public void Clear()
    {
        if (cells == null)
            return;

        Array.ForEach(cells, cell => cell.card = null);
    }

    /// <summary>
    /// 직선상의 카드들을 취득.
    /// null값이 들어감
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IEnumerable<K_PlayingCard>> GetCardsInLinear()
    {
        // y = 0
        for (int i = 0; i < row; i++)
        {
            yield return cells.Where(c => c.coordination.x == i).Select(c => c.card);
        }
        // x = 0
        for (int i = 0; i < column; i++)
        {
            yield return cells.Where(c => c.coordination.y == i).Select(c => c.card);
        }
        // y = x
        for (int i = -row + 1; i < column; i++)
        {
            yield return cells.Where(c => c.coordination.y == c.coordination.x + i).Select(c => c.card);
        }
        // y = -x
        for (int i = 0; i < row + column; i++)
        {
            yield return cells.Where(c => c.coordination.y == -c.coordination.x + i).Select(c => c.card);
        }
    }

    public bool IsNeighbor(IEnumerable<K_PlayingCard> cards)
    {
        if (cards.Count() < 2)
            return false;

        Vector2[] cp = cards.Select(x => cells.First(y => x.Equals(y.card)).coordination).ToArray();

        if (cards.Count() == 2)
            return Mathf.Abs(cp[0].x - cp[1].x) < 2 && Mathf.Abs(cp[0].y - cp[1].y) < 2;

        Vector2 max = new Vector2(cp.Max(c => c.x), cp.Max(c => c.y));
        Vector2 min = new Vector2(cp.Min(c => c.x), cp.Min(c => c.y));

        if ( max.x - min.x != cards.Count() - 1 || max.y - min.y != cards.Count() - 1)
            return false;

        // 직선상의 배치 확인
        if (cp.Skip(2).All(p => p.y == (cp[1].y - cp[0].y) / (cp[1].x - cp[0].x) * (p.x - cp[0].x) - cp[0].y))
        {
            // 네 장 이상의 카드일 경우 경로상의 모든 카드가 대상이 되어야 함
            if (cards.Count() > 3)
                return (min.y == 0 && max.y == column) || (min.x == 0 && max.x == row) || (min.x == min.y && max.x == max.y);
            else
                return true;
        }
        else return false;

        //// 네 장 이상의 카드일 경우 경로상의 모든 카드가 대상이 되어야 함
        //if (line.x == 1 && line.y == count && edge.y == count - 1) // 종
        //    return count < 4 ? true : cp.Min(a => a.y) == 0 && cp.Max(a => a.y) == K_DevelopOptionData.GetInt("Column") ? true : false;
        //if (line.x == count && line.y == 1 && edge.x == count - 1) // 횡
        //    return count < 4 ? true : cp.Min(a => a.x) == 0 && cp.Max(a => a.x) == K_DevelopOptionData.GetInt("Row") ? true : false;
        //if (line.x == count && line.y == count && edge.x == count - 1 && edge.y == count - 1) // 대각선
        //    return count < 4 ? true : cp.Min(a => a.y) == 0 && cp.Max(a => a.y) == K_DevelopOptionData.GetInt("Column") ? true : false;
    }

    #endregion

    IEnumerator goToCell(float delay = 0.03f)
    {
        K_Report.Log(name + " : goToCell");

        foreach (var card in cells.Where(cell => cell.card != null && cell.card.transform.position.V2() != cell.position).Select(c => c.card))
        {
            StartCoroutine(this.goToCell(card));
            yield return new WaitForSeconds(delay);
        }
        yield break;
    }

    IEnumerator goToCell(K_PlayingCard card)
    {
        Cell target = cells.First(c => c.card.Equals(card));

        if (target == null)
            yield break;

        if (target.position.AttachZ(card) != card.transform.position)
            card.transform.position = target.position.AttachZ(card.transform.position.z);

        yield break;
    }

    void cellIn_deck(K_PlayingCard card)
    {
        K_Report.Log(name + " : cellIn_deck " + card.name);

        Vector3 p = Array.Find(cells, x => x.card.Equals(card)).position.AttachZ(card);

        card.AddPosition(new Vector3(card.transform.position.x, cells.Last().position.y, card.transform.position.z), 2f, NormalCurve.Ease.Out);
        card.AddPosition(new Vector3(-Camera.main.orthographicSize, cells.Last().position.y, p.z), 2f, NormalCurve.Ease.In);
        card.AddPosition(new Vector3(Camera.main.orthographicSize, p.y, p.z));
        card.AddPosition(p, 2f, NormalCurve.Ease.Out);
    }

    void cellIn(K_PlayingCard card)
    {
        K_Report.Log(name + " : cellin " + card.name);

        Vector3[] p = new Vector3[] { card.transform.position, cells.First(c => c.card != null && c.card.Equals(card)).position.AttachZ(card.transform.position.z) };

        if (p[0].y != p[1].y)
        {

            p = new Vector3[] {p[0], new Vector3(-Camera.main.orthographicSize, p[0].y, p[0].z), 
                new Vector3(Camera.main.orthographicSize, p[1].y, p[0].z), p[1]};

            card.AddPosition(p[1], 2f, NormalCurve.Ease.In);
            card.AddPosition(p[2]);
            card.AddPosition(p[3], 2f, NormalCurve.Ease.Out);
        }
        else
        {
            card.AddPosition(p[1], 2f, NormalCurve.Ease.InOut);
        }
        card.GoWork();
    }
}
