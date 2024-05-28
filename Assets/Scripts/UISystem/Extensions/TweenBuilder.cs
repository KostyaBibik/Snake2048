using System;
using System.Collections;
using UnityEngine;

namespace GameUtilities.CoroutineHelper
{
    public class BreakTrigger
    {
        public bool isBreak = false;
    }

    public static class TweenBuilder
    {
        public static IEnumerator InvokeInCoroutine(this Action callback)
        {
            callback?.Invoke();
            yield break;
        }

        public static IEnumerator SetActive(GameObject gameObject, bool active)
        {
            gameObject.SetActive(active);

            yield break;
        }

        public static IEnumerator PlayAndWait(this Animator animator, string clipName)
        {
            if (animator != null)
            {
                animator.Play(clipName);

                if (animator.updateMode == AnimatorUpdateMode.UnscaledTime)
                {
                    yield return new WaitForSecondsRealtime(animator.GetCurrentAnimatorStateInfo(0).length);
                }
                else
                {
                    yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
                }
            }
        }

        public static IEnumerator Wait(float time)
        {
            yield return new WaitForSeconds(time);
        }

        public static IEnumerator Wait(CoroutineSequence sequence)
        {
            while(sequence.IsRunning)
            {
                yield return null;
            }
        }
        
        public static IEnumerator WaitWhile<T>(T target, Predicate<T> condition)
        {
            if (condition == null)
            {
                yield break;
            }

            while(condition.Invoke(target))
            {
                yield return null;
            }
        }

        public static IEnumerator LerpValue01(Action<float> callback)
        {
            return LerpValue01(1F, true, callback);
        }

        /// <param name="callback">(time, prevTime)</param>
        public static IEnumerator LerpValue01(float duration, bool unscaledTime, Action<float, float, BreakTrigger> callback)
        {
            var time = 0F;
            var prevTime = 0F;
            var trigger = new BreakTrigger();

            if (duration == 0)
            {
                callback?.Invoke(1F, 0F, trigger);
                yield break;
            }

            while (time < 1.0F && !trigger.isBreak)
            {
                callback?.Invoke(time, prevTime, trigger);

                yield return null;

                prevTime = time;
                time += (unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) / duration;
            }

            if (!trigger.isBreak)
            {
                callback?.Invoke(1F, prevTime, trigger);
            }
        }

        /// <param name="callback">(time)</param>
        public static IEnumerator LerpValue01(float duration, bool unscaledTime, Action<float> callback)
        {
            return LerpValue01(duration, unscaledTime, (time, prevTime, trigger) =>
            {
                callback?.Invoke(time);
            });
        }
    }
}