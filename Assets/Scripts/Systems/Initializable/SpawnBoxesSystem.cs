﻿using System;
using System.Collections;
using Database;
using Enums;
using Helpers;
using Infrastructure.Pools.Impl;
using Services;
using Services.Impl;
using UniRx;
using UnityEngine;
using IInitializable = Zenject.IInitializable;
using Random = UnityEngine.Random;

namespace Systems.Initializable
{
    public class SpawnBoxesSystem : IInitializable, IDisposable
    {
        private IDisposable _observer;
        
        private readonly BoxService _boxService;
        private readonly GameSceneHandler _sceneHandler;
        private readonly GameSettingsConfig _gameSettingsConfig;
        private readonly BoxPool _boxPool;
        private readonly GameMatchService _gameMatchService;

        private float _spawnRadius;
        private float _minSpawnInterval;
        private float _maxSpawnInterval;
        private float _minSpawnCount;
        private float _maxSpawnCount;
        private int _initialCount;
        private Bounds _boundsArea;
        private readonly Collider[] _overlapResults = new Collider[10];
        private float _spawnChanceGradeDifference3 = .01f;
        private float _spawnChanceGradeDifference2 = .02f;
        private float _spawnChanceGradeDifference1 = .04f;
        
        private const string BoxLayerMask = "Box";
        
        private SpawnBoxesSystem(
            BoxService boxService,
            GameSceneHandler sceneHandler,
            GameSettingsConfig gameSettingsConfig,
            BoxPool boxPool,
            GameMatchService gameMatchService
        )
        {
            _boxService = boxService;
            _sceneHandler = sceneHandler;
            _gameSettingsConfig = gameSettingsConfig;
            _boxPool = boxPool;
            _gameMatchService = gameMatchService;
        }
        
        public void Initialize()
        {
            _spawnRadius = _gameSettingsConfig.SpawnRadius;
            _minSpawnInterval = _gameSettingsConfig.MinSpawnInterval;
            _maxSpawnInterval = _gameSettingsConfig.MaxSpawnInterval;
            _minSpawnCount = _gameSettingsConfig.MinSpawnCount;
            _maxSpawnCount = _gameSettingsConfig.MaxSpawnCount;
            _initialCount = _gameSettingsConfig.InitialIdleBoxCount;
            _boundsArea = _sceneHandler.FieldView.Collider.bounds;
            
            SpawnInitialBoxes(_initialCount);    
            
            _observer?.Dispose();

            _observer = Observable.FromCoroutine(SpawnBoxesWithDelay)
                .Subscribe();
        }
        
        private void SpawnInitialBoxes(int count)
        {
            Observable.FromCoroutine(() => InitialSpawn(count)).Subscribe();
        }

        private IEnumerator InitialSpawn(int count)
        {
            yield return new WaitUntil(() => _gameMatchService.IsGameRunning());
            
            for (var i = 0; i < count; i++)
            {
                var randomGrade = EBoxGrade.Grade_2.GetRandomEnumBetween(EBoxGrade.Grade_4);
                SpawnBox(randomGrade);
            }
        }
        
        private void SpawnBox(EBoxGrade eBoxGrade)
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
                
            var idleBox = _boxPool.GetBox(eBoxGrade);
            idleBox.transform.position = spawnPosition;
            idleBox.isIdle = true;
            idleBox.isDestroyed = false;
            idleBox.isPlayer = false;
            idleBox.isBot = false;
            idleBox.gameObject.SetActive(true);
            
            _boxService.RegisterNewTeam(idleBox, string.Empty);
        }
        
        private IEnumerator SpawnBoxesWithDelay()
        {
            do
            {
                var delay = Random.Range(_minSpawnInterval, _maxSpawnInterval);
                
                yield return new WaitForSeconds(delay);

                yield return new WaitUntil(() => _gameMatchService.IsGameRunning());

                Team first = null;
                foreach (var b in _boxService.GetAllTeams())
                {
                    if (b.Leader.isPlayer)
                    {
                        first = b;
                        break;
                    }
                }

                var playerGrade = EBoxGrade.Grade_8;
                if (first != null)
                {
                    playerGrade = first.Leader.Grade;
                }

                var spawnCount = Random.Range(_minSpawnCount, _maxSpawnCount);

                for (var i = 0; i < spawnCount; i++)
                {
                    var randomGrade = CalculateRandomGrade(playerGrade);

                    SpawnBox(randomGrade);
                }

            } while (true);
        }

        private Vector3 GetRandomPositionInBounds()
        {
            var x = Random.Range(_boundsArea.min.x, _boundsArea.max.x);
            var y = _boundsArea.max.y * 2;
            var z = Random.Range(_boundsArea.min.z, _boundsArea.max.z);
            return new Vector3(x, y, z);
        }
        
        private EBoxGrade CalculateRandomGrade(EBoxGrade playerHighGrade)
        {
            var randomValue = Random.value;
            var randomGrade = EBoxGrade.Grade_2;
            
            if (randomValue < _spawnChanceGradeDifference3)
            {
                randomGrade = EBoxGrade.Grade_2.GetRandomEnumBetween(playerHighGrade.NextSteps(3));
            }
            else if (randomValue < _spawnChanceGradeDifference3 + _spawnChanceGradeDifference2)
            {
                randomGrade = EBoxGrade.Grade_2.GetRandomEnumBetween(playerHighGrade.NextSteps(2));
            }
            else if (randomValue < _spawnChanceGradeDifference3 + _spawnChanceGradeDifference2 + _spawnChanceGradeDifference1)
            {
                randomGrade =  EBoxGrade.Grade_2.GetRandomEnumBetween(playerHighGrade.Next());
            }
            else
            {
                randomGrade = EBoxGrade.Grade_2.GetRandomEnumBetween(EBoxGrade.Grade_4);
            }

            if (randomGrade > EBoxGrade.Grade_64)
            {
                randomGrade = EBoxGrade.Grade_32.GetRandomEnumBetween(EBoxGrade.Grade_128);
            }
            
            return randomGrade;
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