using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class K_CountDown : MonoSingleton<K_CountDown>
{
    int turnlimit;
    //float count;
    System.Diagnostics.Stopwatch stopWatch;
    int flag = 3;
    Dictionary<string, GameObject> sign;

    public override void OnInitialize()
    {
        turnlimit = K_OptionData.Get<int>("TurnLimit", 0);
        this.gameObject.SetActive(K_Flag.State("GameMode") == 3);
        //this.count = this.turnlimit;
        stopWatch = new System.Diagnostics.Stopwatch();
    }

    void Start()
    {
        sign = new Dictionary<string, GameObject>();
        "123".ForEach(x => sign.Add(x.ToString(), GetComponentsInChildren<Transform>().First(y => y.name.Equals(x.ToString())).gameObject));
        sign.Values.ForEach(x => x.SetActive(false));
    }

    public void Pause()
    {
        if (!this.gameObject.activeSelf || turnlimit == 0)
            return;

        stopWatch.Stop();
        StopCoroutine("go");
        K_Report.Log("<color=olive><b>" + name + "</b> : Pause</color>");
    }

    public void Stop()
    {
        if (!this.gameObject.activeSelf || turnlimit == 0)
            return;

        stopWatch.Reset();
        flag = 3;
        StopCoroutine("go");
        //this.count = 0;
        sign.Values.ForEach(x => x.SetActive(false));
        K_Report.Log("<color=olive><b>" + name + "</b> : Stop</color>");
    }

    public void Go()
    {
        if (!this.gameObject.activeSelf || turnlimit == 0)
            return;

        stopWatch.Start();
        StartCoroutine("go");
        K_Report.Log("<color=olive><b>" + name + "</b> : Go</color>");
    }

    IEnumerator go()
    {
        float count;
        do
        {
            //count -= Time.deltaTime;
            count = turnlimit - stopWatch.Elapsed.Seconds;

            if (count < flag && flag > 0)
            {
                K_Report.Log("<color=olive><b>" + name + "</b> : go \n " + flag + "</color>");
                sign[flag--.ToString()].SetActive(true);
            }
            yield return new WaitForFixedUpdate();
        } while (count >= 0);

        K_Rule.Instance.SendMessage("TimeOver");
        stopWatch.Reset();

        yield break;
    }
}
