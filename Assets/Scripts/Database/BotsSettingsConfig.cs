using UnityEngine;

namespace Database
{
    [CreateAssetMenu(menuName = "Config/" + nameof(BotsSettingsConfig),
        fileName = nameof(BotsSettingsConfig), order = 3)]
    public class BotsSettingsConfig : ScriptableObject
    {
        [SerializeField] private float changeRandomDirectionInterval = 0.3f;
        public float ChangeRandomDirectionInterval => changeRandomDirectionInterval;
        
        [SerializeField] private float distanceInfluenceForFindTarget = 0.4f;
        public float DistanceInfluenceForFindTarget => distanceInfluenceForFindTarget;
        
        [SerializeField] private float gradeInfluenceForFindTarget = 0.4f;
        public float GradeInfluenceForFindTarget => gradeInfluenceForFindTarget;

        [SerializeField] private float chanceChangeTarget = .1f;
        public float ChanceChangeTarget => chanceChangeTarget;
        
        [SerializeField] private float intiAccelerationChance = .1f;
        public float IntiAccelerationChance => intiAccelerationChance;

        [Header("Spawn settings")] 
        [SerializeField] private float spawnChanceGradeDifference3 = .04f;
        public float SpawnChanceGradeDifference3 => spawnChanceGradeDifference3;
        
        [SerializeField] private float spawnChanceGradeDifference2 = .1f;
        public float SpawnChanceGradeDifference2 => spawnChanceGradeDifference2;
        
        [SerializeField] private float spawnChanceGradeDifference1 = .12f;
        public float SpawnChanceGradeDifference1 => spawnChanceGradeDifference1;
    }
}