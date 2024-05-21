using UnityEngine;

namespace Database
{
    [CreateAssetMenu(menuName = "Config/" + nameof(GameSettingsConfig),
        fileName = nameof(GameSettingsConfig), order = 3)]
    public class GameSettingsConfig : ScriptableObject
    {
        [Header("Box Settings")]
        [SerializeField] private float boxMoveSpeed = 2f;
        public float BoxMoveSpeed => boxMoveSpeed;

        [SerializeField] private float boxMoveSpeedOnMerge = 10f;
        public float BoxMoveSpeedOnMerge => boxMoveSpeedOnMerge;

        [SerializeField] private float boxFollowDistance = 1.5f;
        public float BoxFollowDistance => boxFollowDistance;

        [SerializeField] private float distanceForMerge = 0.1f;
        public float DistanceForMerge => distanceForMerge;

        [Space]
        [Header("Spawn Settings")]
        [SerializeField] private float minSpawnInterval = .3f;
        public float MinSpawnInterval => minSpawnInterval;
        
        [SerializeField] private float maxSpawnInterval = 1f;
        public float MaxSpawnInterval => maxSpawnInterval;

        [SerializeField] private float spawnRadius = 2f;
        public float SpawnRadius => spawnRadius;
        
        [SerializeField] private int initialIdleBoxCount = 10;
        public int InitialIdleBoxCount => initialIdleBoxCount;
        
        [SerializeField] private int minSpawnCount = 5;
        public int MinSpawnCount => minSpawnCount;
        
        [SerializeField] private int maxSpawnCount = 5;
        public int MaxSpawnCount => maxSpawnCount;
        
        [Space]
        [Header("Bot Settings")]
        [SerializeField] private int maxCountBots = 20;
        public int MaxCountBots => maxCountBots;
        
        [SerializeField] private int initialBotCount = 12;
        public int InitialBotCount => initialBotCount;
        
        [SerializeField] private float minSpawnDistance = 4;
        public float MinSpawnDistance => minSpawnDistance;
        
        [SerializeField] private float botSpawnInterval = 4;
        public float BotSpawnInterval => botSpawnInterval;
    }
}