using System;
using System.Collections;
using Database;
using Enums;
using Helpers;
using Infrastructure.Factories.Impl;
using Services.Impl;
using UniRx;
using UniRx.Toolkit;
using UnityEngine;
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
        private readonly BoxPool _boxPool;

        private float _spawnRadius;
        private float _spawnInterval;
        private int _initialCount;
        private Bounds _boundsArea;
        private readonly Collider[] _overlapResults = new Collider[10];
        private const string BoxLayerMask = "Box";
        
        private SpawnBoxesSystem(
            BoxEntityFactory boxEntityFactory,
            BoxService boxService,
            GameSceneHandler sceneHandler,
            GameSettingsConfig gameSettingsConfig,
            BoxPool boxPool
        )
        {
            _boxEntityFactory = boxEntityFactory;
            _boxService = boxService;
            _sceneHandler = sceneHandler;
            _gameSettingsConfig = gameSettingsConfig;
            _boxPool = boxPool;
        }
        
        public void Initialize()
        {
            _spawnRadius = _gameSettingsConfig.SpawnRadius;
            _spawnInterval = _gameSettingsConfig.MinSpawnInterval;
            _initialCount = _gameSettingsConfig.InitialIdleBoxCount;
            _boundsArea = _sceneHandler.FieldView.Collider.bounds;
            
            SpawnInitialBoxes(_initialCount);    
            
            _observer?.Dispose();

            _observer = Observable.FromCoroutine(SpawnBoxesWithDelay)
                .Subscribe();
        }
        
        private void SpawnInitialBoxes(int count)
        {
            for (var i = 0; i < count; i++)
            {
                SpawnBox();
            }
        }

        private void SpawnBox()
        {
            Vector3 spawnPosition;
            var attempts = 0;
            const int maxAttempts = 10;
            
            do
            {
                spawnPosition = GetRandomPositionInBounds();
                attempts++;
            }
            while (IsPositionOccupied(spawnPosition) && attempts < maxAttempts);
            
            if (attempts >= maxAttempts)
            {
                return;
            }
                
            //var idleBox = _boxEntityFactory.Create(EBoxGrade.Grade_2);
            var idleBox = _boxPool.GetBox(EBoxGrade.Grade_2);
            idleBox.gameObject.SetActive(true);
            idleBox.transform.position = spawnPosition;
            idleBox.isIdle = true;
            idleBox.isDestroyed = false;
            idleBox.isPlayer = false;
            idleBox.isBot = false;

            _boxService.RegisterNewTeam(idleBox);
        }
        
        private IEnumerator SpawnBoxesWithDelay()
        {
            do
            {
                var delay = Random.Range(_gameSettingsConfig.MinSpawnInterval, _gameSettingsConfig.MaxSpawnInterval);
                
                yield return new WaitForSeconds(delay);

                var spawnCount = Random.Range(_gameSettingsConfig.MinSpawnCount, _gameSettingsConfig.MaxSpawnCount);

                for (var i = 0; i < spawnCount; i++)
                {
                    SpawnBox();
                }

            } while (true);
            
            yield return null;
        }

        private Vector3 GetRandomPositionInBounds()
        {
            var x = Random.Range(_boundsArea.min.x, _boundsArea.max.x);
            var y = _boundsArea.max.y * 2;
            var z = Random.Range(_boundsArea.min.z, _boundsArea.max.z);
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