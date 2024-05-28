using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtilities.CoroutineHelper
{
    public class CoroutineSequence
    {
        public bool IsRunning { get; private set; }

        private readonly Action _endRoutineCallback;
        private readonly List<IEnumerator> _sequencedCoroutines;
        private Coroutine _semaphoreRoutine;

        public CoroutineSequence(Action callback = null)
        {
            _endRoutineCallback = callback;
            _sequencedCoroutines = new List<IEnumerator>();
        }

        public void End()
        {
            if (IsRunning)
            {
                CoroutineHost.Host.StopCoroutine(_semaphoreRoutine);
                _endRoutineCallback?.Invoke();
                IsRunning = false;
            }
        }

        public void Run(params IEnumerator[] coroutines)
        {
            if (IsRunning)
            {
                throw new Exception("Sequence is already running and cannot be started until it stops !!!");
            }

            _sequencedCoroutines.Clear();
            _sequencedCoroutines.AddRange(coroutines);
            _semaphoreRoutine = CoroutineHost.Host.StartCoroutine(SemaphoreCoroutine(_endRoutineCallback));
        }

        public void Stop()
        {
            if (IsRunning)
            {
                CoroutineHost.Host.StopCoroutine(_semaphoreRoutine);
                IsRunning = false;
            }
        }

        private IEnumerator SemaphoreCoroutine(Action endCallback)
        {
            IsRunning = true;

            foreach (IEnumerator routine in _sequencedCoroutines)
            {
                yield return routine;
            }

            IsRunning = false;

            endCallback?.Invoke();
        }
    }
}