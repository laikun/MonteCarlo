using UnityEngine;
using System.Collections;

public class K_ElapsedTime : MonoSingleton<K_ElapsedTime> {

    //float time;
    System.Diagnostics.Stopwatch timeWatch;

    public override void OnInitialize()
    {
        this.gameObject.SetActive(K_Flag.State("GameMode") != 1);
        //this.time = 0f;
        timeWatch = new System.Diagnostics.Stopwatch();
        StopCoroutine("progress");
    }

    public void Stop()
    {
        StopCoroutine("reflesh");
    }

    public void Go()
    {
        if (!gameObject.activeSelf)
            return;

        timeWatch.Start();
        StartCoroutine("reflesh");
    }

    protected IEnumerator reflesh()
    {
        UILabel label = GetComponent<UILabel>();
        do
        {
            label.text = string.Format("{0:D2}:{1:D2}.{2:D2}", timeWatch.Elapsed.Minutes, timeWatch.Elapsed.Seconds, timeWatch.Elapsed.Milliseconds / 10);
            yield return null;
        } while (true);
    }
}
