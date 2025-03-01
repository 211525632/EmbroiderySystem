using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventCenter<Action>.Instance.AddEvent("test", TriggerEvent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void TriggerEvent()
    {
        Debug.Log("�ɹ���������¼�");
    }

    public void TestEventCenter()
    {
        EventCenter<Action>.Instance.GetAction("test")?.Invoke();
    }

}
