using System.Collections.Generic;
using System.Linq;
using Components.Boxes.Views.Impl;
using Services.Impl;
using UnityEngine;

namespace Helpers.Game
{
    public static class BotPathEvaluator
    {
        private const float ThreatRadiusSquared = 6f;
        private const float PathStep = 1f; 
        private const int MaxIterations = 3; 

        public static bool IsPathSafe(BoxService boxService, BoxView botView, BoxView targetBox)
        {
            var path = CalculatePath(botView.transform.position, targetBox.transform.position);
            if (path == null)
                return false;

            for (var index = 0; index < path.Length; index++)
            {
                var position = path[index];
                if (IsThreatNearby(boxService, botView, position))
                    return false;
            }

            return true;
        }

        private static Vector3[] CalculatePath(Vector3 start, Vector3 end)
        {
            var path = new Vector3[MaxIterations];
            var direction = (end - start).normalized;
            var currentPosition = start;
            var iterationCount = 0;
            path[iterationCount] = currentPosition;
        
            while (Vector3.SqrMagnitude(currentPosition - end) > 0.5625f)
            {
                if (iterationCount >= MaxIterations)
                    break;
                
                currentPosition += direction * PathStep; 
                path[iterationCount] = currentPosition;

                iterationCount++;
            }
        
            return path;
        }

        private static bool IsThreatNearby(BoxService boxService, BoxView botView, Vector3 position)
        {
            var boxes = boxService
                .GetAllTeams()
                .Where(b => b.Leader != botView);

            foreach (var t in boxes)
            {
                var box = t.Leader;
                if (box.isIdle)
                    continue;

                if (box.Grade > botView.Grade)
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
