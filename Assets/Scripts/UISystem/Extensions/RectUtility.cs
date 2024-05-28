using UnityEngine;

namespace GameUtilities.Extensions
{
    public static class RectUtility
    {
        public static Rect Move(this Rect rect, float x, float y)
        {
            rect.x += x;
            rect.width -= x;

            rect.y += y;
            rect.height -= y;

            return rect;
        }

        public static Rect Width(this Rect rect, float width)
        {
            rect.width = width;

            return rect;
        }

        public static Rect Height(this Rect rect, float height)
        {
            rect.height = height;

            return rect;
        }

        public static Rect[] HorizontalSplit(this Rect rect, params float[] values)
        {
            float x = rect.x;

            Rect[] rects = new Rect[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                float width = rect.width * values[i];

                rects[i] = new Rect(
                    x,
                    rect.y,
                    width,
                    rect.height);

                x += width;
            }

            return rects;
        }

        public static Rect[] VerticalSplit(this Rect rect, params float[] values)
        {
            float y = rect.y;

            Rect[] rects = new Rect[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                float height = rect.height * values[i];

                rects[i] = new Rect(
                    rect.x,
                    y,
                    rect.width,
                    height);

                y += height;
            }

            return rects;
        }

        public static Rect[] VerticalSplit(this Rect rect, int count)
        {
            float y = rect.y;

            Rect[] rects = new Rect[count];

            for (int i = 0; i < count; i++)
            {
                float height = rect.height / count;

                rects[i] = new Rect(
                    rect.x,
                    y,
                    rect.width,
                    height);

                y += height;
            }

            return rects;
        }
    }
}