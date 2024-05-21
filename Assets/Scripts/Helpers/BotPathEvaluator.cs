using System.Collections.Generic;
using Services.Impl;
using UnityEngine;
using Views.Impl;

namespace Helpers
{
    public static class BotPathEvaluator
    {
        public static bool IsPathSafe(BoxService boxService, BoxView botView, BoxView targetBox)
        {
            var path = CalculatePath(botView.transform.position, targetBox.transform.position);
            if (path == null)
                return false;

            foreach (var position in path)
            {
                if (IsThreatNearby(boxService, botView, position))
                    return false;
            }
        
            return true;
        }

        private static List<Vector3> CalculatePath(Vector3 start, Vector3 end)
        {
            var path = new List<Vector3>();
            var direction = (end - start).normalized;
            var currentPosition = start;
            var maxIterations = 50; 
            var iterationCount = 0;
        
            while (Vector3.Distance(currentPosition, end) > .75f)
            {
                if (iterationCount >= maxIterations)
                {
                    return null;
                }

                currentPosition += direction * .5f; 
                path.Add(currentPosition);

                iterationCount++;
            }
        
            return path;
        }

        private static bool IsThreatNearby(BoxService boxService, BoxView botView, Vector3 position)
        {
            var threatRadius = 2f; 
            var boxes = boxService.GetAllBoxes();
        
            foreach (var box in boxes)
            {
                if (box.isIdle)
                    continue;
            
                if (!boxService.AreInSameTeam(botView, box) && box.Grade > botView.Grade)
                {
                    if (Vector3.Distance(box.transform.position, position) < threatRadius)
                    {
                        return true;
                    }
                }
            }
        
            return false;
        }
    }
}
