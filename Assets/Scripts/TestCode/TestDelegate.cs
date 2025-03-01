using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestDelegate : MonoBehaviour
{
    Action action2;

    Action action;

    public void Start()
    {
        EventCenter<Action>.Instance.AddEvent("test",One);
        EventCenter<Action>.Instance.AddEvent("test",Two);

        EventCenter<Action>.Instance.GetAction("test").Invoke();

    }

    public void One()
    {
        Debug.Log("one");
    }


    public void Two()
    {
        Debug.Log("two");
    }
}
