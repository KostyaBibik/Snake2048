using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace GameUtilities.CoroutineHelper
{
    public class CoroutinePlayable
    {
        public bool IsRunning
        {
            get => RunnedRoutines > 0;
        }

        public int RunnedRoutines 
        { 
            get;
            private set;
        }

        private Coroutine[] _currentCoroutine;


        /// <summary>
        /// Запустить параллельное исполнение
        /// </summary>
        /// <param name="coroutines"></param>
        public void Run(params IEnumerator[] coroutines)
        {
            if (IsRunning)
            {
                throw new Exception("Sequence is already running and cannot be started until it stops !!!");
            }

            RunnedRoutines = coroutines.Length;
            _currentCoroutine = coroutines
                .Select(x => CoroutineHost.Host.StartCoroutine(SemaphoreCoroutine(x)))
                .ToArray();
        }
        
        public void Stop()
        {
            if (IsRunning)
            {
                CoroutineHost.Host.StopCoroutines(_currentCoroutine);
                RunnedRoutines = 0;
            }
        }

        private IEnumerator SemaphoreCoroutine(IEnumerator coroutine)
        {
            yield return coroutine;

            RunnedRoutines--;
        }
    }
}