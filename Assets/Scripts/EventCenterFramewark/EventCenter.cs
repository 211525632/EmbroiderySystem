using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
///  EventCenter<Action> one;
///  EventCenter<Action<int>> two;
///  two.addList<Action<int> >(int )
///  
/// Delegate 是值类型，不是引用类型。不会自动变换
/// </summary>
/// <typeparam name="T">Action的类型，比如：多参数的情况</typeparam>

public class EventCenter<T> where T : Delegate
{
    private static EventCenter<T> _eventCenter;

    public static EventCenter<T> Instance
    {
        get
        {
            if (object.ReferenceEquals(null, _eventCenter))
            {
                _eventCenter = new EventCenter<T>();
            }

            return _eventCenter;
        }
    }
    private EventCenter()
    {

    }

    private readonly Dictionary<string, T> _actionDict = new();

    T _outPut;

    public void AddEvent(string key,T action)
    {
        lock (_actionDict)
        {

            _actionDict.TryGetValue(key, out _outPut);

            if (object.ReferenceEquals(_outPut, null))
                _actionDict.Add(key, action);
            else
            {
                _actionDict[key] = Delegate.Combine(action, _outPut) as T;
            }

        }
    }

    /// <summary>
    /// 通过
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public T GetAction(string key)
    {
        lock (_actionDict)
        {
            _actionDict.TryGetValue(key, out _outPut);

            return _outPut;
        }
    }

}
