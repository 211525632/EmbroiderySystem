//using System;
//using System.Collections;
//using UnityEngine;

//namespace Assets.Scripts.TestCode
//{
//    public class TestMoniter : MonoBehaviour
//    {
//        MClass myClass = new MClass();

//        Iinterface normalInterface = null;

//        public void Awake()
//        {
//            normalInterface = new InterfaceMoniter<MClass>(myClass);
//        }

//        public void Update()
//        {
//            StartCoroutine(WaitforTimes());
//        }

//        public IEnumerator WaitforTimes()
//        {
//            yield return new WaitForSeconds(1f);
//            normalInterface.Func();
//        }
//    }

//    #region interface

//    interface Interface
//    {
//        void Func();
//    }

//    class Class : Interface
//    {
//        public void Func()
//        {
//            Debug.Log("Function");
//        }
//    }


//    #endregion

//}
