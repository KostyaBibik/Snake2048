using System;
using System.Collections;
using Database;
using Enums;
using Helpers;
using Infrastructure.Factories.Impl;
using Services.Impl;
using UniRx;
using UnityEngine;
using Views.Impl;
using Zenject;
using Random = UnityEngine.Random;

namespace Systems.Initializable
{
    public class SpawnBoxesSystem : IInitializable, IDisposable
    {
        private IDisposable _observer;
        
        private readonly BoxEntityFactory _boxEntityFactory;
        private readonly BoxService _boxService;
        private readonly GameSceneHandler _sceneHandler;
        private readonly GameSettingsConfig _gameSettingsConfig;

        private float _spawnRadius;
        private float _spawnInterval;
        private readonly Collider[] _overlapResults = new Collider[10];
        private const string BoxLayerMask = "Box";
        
        private SpawnBoxesSystem(
            BoxEntityFactory boxEntityFactory,
            BoxService boxService,
            GameSceneHandler sceneHandler,
            GameSettingsConfig gameSettingsConfig
        )
        {
            _boxEntityFactory = boxEntityFactory;
            _boxService = boxService;
            _sceneHandler = sceneHandler;
            _gameSettingsConfig = gameSettingsConfig;
        }
        
        public void Initialize()
        {
            _spawnRadius = _gameSettingsConfig.SpawnRadius;
            _spawnInterval = _gameSettingsConfig.SpawnInterval;
                
            _observer?.Dispose();

            _observer = Observable.FromCoroutine(SpawnBoxesWithDelay)
                .Subscribe();
        }

        private IEnumerator SpawnBoxesWithDelay()
        {
            var delay = new WaitForSeconds(_spawnInterval);
            var bounds = _sceneHandler.FieldView.Collider.bounds;
            
            do
            {
                yield return delay;
                
                Vector3 spawnPosition;
                var attempts = 0;
                const int maxAttempts = 10;

                do
                {
                    spawnPosition = GetRandomPositionInBounds(bounds);
                    attempts++;
                }
                while (IsPositionOccupied(spawnPosition) && attempts < maxAttempts);

                if (attempts >= maxAttempts)
                {
                    continue;
                }
                
                var idleBox = _boxEntityFactory.Create(EBoxGrade.Grade_2);
                idleBox.transform.position = spawnPosition;

                _boxService.RegisterNewTeam(idleBox);

            } while (true);
            
            yield return null;
        }

        private Vector3 GetRandomPositionInBounds(Bounds bounds)
        {
            var x = Random.Range(bounds.min.x, bounds.max.x);
            var y = bounds.max.y * 2;
            var z = Random.Range(bounds.min.z, bounds.max.z);
            return new Vector3(x, y, z);
        }
        
        private bool IsPositionOccupied(Vector3 position)
        {
            var numColliders = Physics.OverlapSphereNonAlloc(position, _spawnRadius, _overlapResults, LayerMask.GetMask(BoxLayerMask));
            return numColliders > 0;
        }
        
        public void Dispose()
        {
            _observer?.Dispose();
        }
    }
}