using Enums;

namespace Helpers.Game
{
    public class ResultUtility
    {
        public static int CalcResultScore(float leadTime, int countKills, EBoxGrade finalGrade, int sumPoints)
        {
            var resultScore = 0;
            
            resultScore += (int)finalGrade;
            resultScore += (int)leadTime * 20;
            resultScore += sumPoints;
            resultScore += countKills * 1000;

            return resultScore;
        }
    }
}