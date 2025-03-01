using Pools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UiFramewark;
using UnityEngine;

namespace Assets.Scripts.Pools
{
    internal class PoolDemo:MonoBehaviour
    {
        public GameObject GameObject;

        private void Start()
        {
            PoolManager.Instance.InitNewPool<UiCollection>("key", 
                UiResoucesManager
                .Instance.LoadUi("key")
                .GetComponent<UiCollection>()
                );

            GameObject = PoolManager.Instance.GetPooledObj<UiCollection>("key").gameObject;

            StartCoroutine(ClearAll());
        }

        WaitForSeconds wait = new WaitForSeconds(3f);

        IEnumerator ClearAll()
        {
            yield return wait;
            PoolManager.Instance.GetPool<UiCollection>("key").DelePool();
        }

    }
}
