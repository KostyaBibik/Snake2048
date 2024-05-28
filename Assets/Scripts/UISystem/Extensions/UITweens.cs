using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameUtilities.CoroutineHelper
{
    public static class UITweens
    {
        public class UIValueContainer : MonoBehaviour
        {
            public float alpha;
            public Vector3 scale;
        }

        private class Item
        {
            public Graphic Graphic;
            public float FromAlpha;
            public float ToAlpha;
            public Vector3 FromScale;
            public Vector3 ToScale;
        }
        
        public static IEnumerator FadeOutUnscaled(float duration, params GameObject[] targets)
        {
            return Fade(duration, true, true, targets);
        }

        public static IEnumerator FadeInUnscaled(float duration, params GameObject[] targets)
        {
            return Fade(duration, true, false, targets);
        }
        
        public static IEnumerator FadeScaleOutUnscaled(float duration, Vector3 fadeOutScale, params GameObject[] targets)
        {
            return FadeScale(duration, true, true, fadeOutScale, targets);
        }

        public static IEnumerator FadeScaleInUnscaled(float duration, params GameObject[] targets)
        {
            return FadeScale(duration, true, false, Vector3.zero, targets);
        }
        
        public static IEnumerator Fade(float duration, bool unscaledTime, bool fadeOut, params GameObject[] targets)
        {
            var colorDictionary = new List<Item>();

            foreach (var target in targets)
            {
                foreach (var graphic in target.GetComponentsInChildren<Graphic>(true))
                {
                    var container = AddIfNotAddedFadeContainer(graphic);

                    colorDictionary.Add(new Item()
                    {
                        Graphic = graphic,
                        FromAlpha = graphic.color.a,
                        ToAlpha = fadeOut ? 0 : container.alpha
                    });
                }
            }

            return TweenBuilder.LerpValue01(duration, unscaledTime, time =>
            {
                foreach (Item item in colorDictionary)
                {
                    var color = item.Graphic.color;
                    color.a = Mathf.Lerp(item.FromAlpha, item.ToAlpha, time);
                    item.Graphic.color = color;
                }
            });
        }
        
        public static IEnumerator FadeScale(float duration, bool unscaledTime, bool fadeOut, Vector3 targetSale, params GameObject[] targets)
        {
            var scaleDictionary = new List<Item>();

            foreach (var target in targets)
            {
                foreach (var graphic in target.GetComponentsInChildren<Graphic>(true))
                {
                    var container = AddIfNotAddedFadeContainer(graphic);

                    scaleDictionary.Add(new Item
                    {
                        Graphic = graphic,
                        FromScale = graphic.transform.localScale,
                        ToScale = fadeOut ? targetSale : container.scale
                    });
                }
            }

            return TweenBuilder.LerpValue01(duration, unscaledTime, time =>
            {
                foreach (Item item in scaleDictionary)
                {
                    var scale = Vector3.Lerp(item.FromScale, item.ToScale, time);
                    item.Graphic.transform.localScale = scale;
                }
            });
        }

        private static UIValueContainer AddIfNotAddedFadeContainer(Graphic graphic)
        {
            var container = graphic.gameObject.GetComponent<UIValueContainer>();

            if (container == null)
            {
                container = graphic.gameObject.AddComponent<UIValueContainer>();
                container.alpha = graphic.color.a;
                container.scale = graphic.transform.localScale;
            }

            return container;
        }
    }
}