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
        [SerializeField] private float spawnInterval = 3f;
        public float SpawnInterval => spawnInterval;

        [SerializeField] private float spawnRadius = 2f;
        public float SpawnRadius => spawnRadius;
    }
}