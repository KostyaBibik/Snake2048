using System.Collections.Generic;
using Components.Boxes.Views.Impl;
using Services.Impl;
using UnityEngine;

namespace Helpers
{
    public static class BotPathEvaluator
    {
        private const float ThreatRadiusSquared = 4f;
        private const float PathStep = 1f; 
        private const int MaxIterations = 25; 

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
            var iterationCount = 0;
        
            while (Vector3.SqrMagnitude(currentPosition - end) > 0.5625f)
            {
                if (iterationCount >= MaxIterations)
                {
                    return null;
                }

                currentPosition += direction * PathStep; 
                path.Add(currentPosition);

                iterationCount++;
            }
        
            return path;
        }

        private static bool IsThreatNearby(BoxService boxService, BoxView botView, Vector3 position)
        {
            var boxes = boxService.GetAllBoxes();
        
            foreach (var box in boxes)
            {
                if (box.isIdle)
                    continue;
            
                if (!boxService.AreInSameTeam(botView, box) && box.Grade > botView.Grade)
                {
                    if (Vector3.SqrMagnitude(box.transform.position - position) < ThreatRadiusSquared)
                    {
                        return true;
                    }
                }
            }
        
            return false;
        }
    }
}
