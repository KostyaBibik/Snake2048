using System;
using System.Collections;
using UnityEngine;

namespace GameUtilities.CoroutineHelper
{
    public class CoroutineHostMono : MonoBehaviour
    {
        public void StopCoroutines(params Coroutine[] coroutines)
        {
            foreach (var coroutine in coroutines)
            {
                StopCoroutine(coroutine);
            }
        }

        public Coroutine StartCoroutine(IEnumerator coroutine, Action callback)
        {
            return StartCoroutine(SemaphoreRoutine(coroutine, callback));
        }

        private IEnumerator SemaphoreRoutine(IEnumerator coroutine, Action callback)
        {
            yield return coroutine;

            callback?.Invoke();
        }
    }

    public static class CoroutineHost
    {
        public static CoroutineHostMono Host
        {
            get
            {
                if(host == null)
                {
                    host = CreateHost();
                }

                return host;
            }
        }

        private static CoroutineHostMono host = null;

        private static CoroutineHostMono CreateHost()
        {
            GameObject hostHolder = new GameObject("Coroutine host");

            return hostHolder.AddComponent<CoroutineHostMono>();
        }
    }
}