using UnityEngine;
using System.Threading;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#region DelegateWork
public delegate void DelegateWork(GameObject gameObject);
public delegate void DelegateBoolWork(GameObject gameObject, bool boolean);
#endregion

public static class NormalCurve
{
    public enum Ease
    {
        Linear,
        InOut,
        In,
        Out
    }

    public static AnimationCurve Curve(Ease ease)
    {
        if (ease == Ease.Linear)
            return Linear;
        if (ease == Ease.InOut)
            return EaseInOut;
        if (ease == Ease.In)
            return EaseIn;
        if (ease == Ease.Out)
            return EaseOut;

        return Linear;
    }

    public static AnimationCurve Linear { get { return AnimationCurve.Linear(0f, 0f, 1f, 1f); } }

    public static AnimationCurve EaseInOut { get { return AnimationCurve.EaseInOut(0f, 0f, 1f, 1f); } }

    public static AnimationCurve EaseIn { get { return new AnimationCurve(new Keyframe(0f, 0f, 0f, 0f), new Keyframe(1f, 1f, 2f, 0f)); } }

    public static AnimationCurve EaseOut { get { return new AnimationCurve(new Keyframe(0f, 0f, 0f, 2f), new Keyframe(1f, 1f, 0f, 0f)); } }
}

public static class ExtensionMethods
{
    #region coroutine work
    /// <summary>
    /// flag가 참일 동안 대기 
    /// </summary>
    /// <param name="mono"></param>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static IEnumerator WorkWait(this MonoBehaviour mono, Func<bool> flag)
    {
        do
        {
            yield return new WaitForFixedUpdate();
        } while (flag());

        yield break;
    }

    /// <summary>
    /// flag가 참일 동안 대기 후 지정된 작업을 시행
    /// </summary>
    /// <param name="mono"></param>
    /// <param name="action">대기 후 실행 할 작업</param>
    /// <param name="flag">대기 기준이 되는 작업</param>
    /// <returns></returns>
    public static IEnumerator WorkDelay(this MonoBehaviour mono, Action<MonoBehaviour> action, Func<MonoBehaviour, bool> flag)
    {
        mono.enabled = false;

        do
        {
            yield return new WaitForFixedUpdate();
        } while (flag(mono));

        action(mono);

        mono.enabled = false;
    }

    public static IEnumerator WorkTimer(this MonoBehaviour mono, Action<MonoBehaviour> action, float timer)
    {
        mono.enabled = false;
        yield return new WaitForSeconds(timer);
        action(mono);
        mono.enabled = true;
    }

    public static IEnumerator WorkLoop(this MonoBehaviour mono, Action loop, Func<bool> flag)
    {
        mono.enabled = false;

        if (flag == null)
            flag = () => true;

        do
        {
            loop();
            mono.enabled = false;
            yield return null;
        } while (flag());

        mono.enabled = true;
    }

    public static IEnumerator WorkLoop(this MonoBehaviour mono, Action<float> loop, K_TimeCurve timecurve)
    {
        mono.enabled = false;

        do
        {
            timecurve.Progress();
            loop(timecurve.Eval);
            mono.enabled = false;
            yield return null;
        } while (timecurve.Eval != 1);

        mono.enabled = true;
        yield break;
    }

    #endregion

    /// <summary>
    /// Gets or add a component. Usage example:
    /// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
    /// </summary>
    static public T GetOrAddComponent<T>(this Component child) where T : Component
    {
        T result = child.GetComponent<T>();
        if (result == null)
        {
            result = child.gameObject.AddComponent<T>();
        }
        return result;
    }

    #region OnStage
    public static void StageIn(this GameObject go)
    {
        Array.ForEach(go.GetComponentsInChildren<Transform>(), x => x.gameObject.layer = LayerMask.NameToLayer("Stage"));
    }
    public static void StageOut(this GameObject go)
    {
        Array.ForEach(go.GetComponentsInChildren<Transform>(), x => x.gameObject.layer = LayerMask.NameToLayer("BackStage"));
    }
    public static void StageUI(this GameObject go)
    {
        Array.ForEach(go.GetComponentsInChildren<Transform>(), x => x.gameObject.layer = LayerMask.NameToLayer("UI"));
    }
    #endregion

    #region Set Position
    public static Vector2 V2(this Vector3 v3)
    {
        return new Vector2(v3.x, v3.y);
    }

    public static void SetXY(this MonoBehaviour mono, Vector2 xy)
    {
        mono.transform.position = new Vector3(xy.x, xy.y, mono.transform.position.z);
    }

    public static void SetZ(this MonoBehaviour mono, float z)
    {
        mono.transform.position = new Vector3(mono.transform.position.x, mono.transform.position.y, z);
    }

    public static Vector2 GetXY(this MonoBehaviour mono)
    {
        return mono.transform.position.V2();
    }

    public static Vector3 AttachZ(this Vector2 a, float z)
    {
        return new Vector3(a.x, a.y, z);
    }

    public static Vector3 AttachZ(this Vector2 a, MonoBehaviour b)
    {
        return new Vector3(a.x, a.y, b.transform.position.z);
    }
    #endregion

    public static void SetScale2D(this MonoBehaviour mono, float s)
    {
        mono.transform.localScale = new Vector3(s, s, mono.transform.localScale.z);
    }

    [ThreadStatic]
    private static System.Random
        Local;

    public static System.Random ThisThreadsRandom
    {
        get { return Local ?? (Local = new System.Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = ThisThreadsRandom.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> elements, Action<T> work)
    {
        foreach (var mono in elements) { work(mono); }

        return elements;
    }

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> elements, Action<T, int> work)
    {
        int index = 0;

        foreach (var mono in elements) { work(mono, index++); }

        return elements;
    }

    public static T DO<T>(this T something, Action<T> work)
    {
        work(something);
        return something;
    }

    #region Combinations
    //public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
    //{
    //    return k == 0 ? new[] { new T[0] } :
    //      elements.SelectMany((e, i) =>
    //        elements.Skip(i + 1).Combinations<T>(k - 1).Select(c => (new[] { e }).Concat(c)));
    //}

    /// <summary>
    /// 조합추출 (숫자)
    /// </summary>
    /// <param name="k">조합할 원소의 수</param>
    /// <param name="n">모집합 수</param>
    /// <returns></returns>
    public static IEnumerable<int[]> Combinations(int k, int n)
    {
        int[] result = new int[k];
        Stack<int> stack = new Stack<int>();
        stack.Push(0);

        while (stack.Count > 0)
        {
            int index = stack.Count - 1;
            int value = stack.Pop();

            while (value < n)
            {
                result[index++] = value++;
                stack.Push(value);
                if (index == k)
                {
                    yield return result;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 배열에서 가능한 조합을 반복기를 통해 취득.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="elements">모집합 배열</param>
    /// <param name="k">조합할 원소의 수</param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<T>> Combination<T>(this IEnumerable<T> elements, int k)
    {
        if (elements.Count() < k)
            yield return new T[0];

        T[] result = new T[k];
        Stack<int> stack = new Stack<int>();
        stack.Push(0);

        while (stack.Count > 0)
        {
            int index = stack.Count - 1;
            int value = stack.Pop();

            while (value < elements.Count())
            {
                result[index++] = elements.ElementAt(value++);
                stack.Push(value);
                if (index == k)
                {
                    yield return result;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 배열에서 가능한 조합의 전체 리스트를 취득함
    /// 성능상 문제 있음
    /// <see cref="ExtensionMethods.Combinations"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="elements">모집합 배열</param>
    /// <param name="k">부분집합 원소의 갯수</param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> elements, int k)
    {
        List<T[]> result = new List<T[]>();
        T[] item = new T[k];
        Stack<int> stack = new Stack<int>();
        stack.Push(0);

        while (stack.Count > 0)
        {
            int index = stack.Count - 1;
            int value = stack.Pop();

            while (value < elements.Count())
            {
                item[index++] = elements.ElementAt(value++);
                stack.Push(value);
                if (index == k)
                {
                    result.Add(new T[0].Concat(item).ToArray());
                    break;
                }
            }
        }
        return result.ToArray();
    }
    #endregion

    public static string ToString<T>(this IEnumerable<T> array) where T : MonoBehaviour
    {
        return "[" + string.Join(", ", array.Select(x => x != null ? x.ToString() : "(null)").ToArray()) + "]";
    }

    public static string ToString<T>(this T[] array) where T : MonoBehaviour
    {
        return "[" + string.Join(", ", array.Select(x => x != null ? x.name : "(null)").ToArray()) + "]";
    }

    #region Invoke
    public static void Invoke(this MonoBehaviour behaviour, string method, object options, float delay)
    {
        behaviour.StartCoroutine(_invoke(behaviour, method, delay, options));
    }

    private static IEnumerator _invoke(this MonoBehaviour behaviour, string method, float delay, object options)
    {
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        Type instance = behaviour.GetType();
        MethodInfo mthd = instance.GetMethod(method);
        mthd.Invoke(behaviour, new object[] { options });

        yield return null;
    }
    #endregion
}
