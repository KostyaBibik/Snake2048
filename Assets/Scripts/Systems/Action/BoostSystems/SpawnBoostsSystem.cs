﻿using System;
using System.Collections;
using Database;
using Enums;
using Helpers;
using Infrastructure.Pools;
using Infrastructure.Pools.Impl;
using Services;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Systems.Initializable
{
    public class SpawnBoostsSystem : IInitializable, IDisposable
    {
        private IDisposable _observer;
        private Bounds _boundsArea;
        
        private readonly GameSceneHandler _sceneHandler;
        private readonly GameSettingsConfig _gameSettingsConfig;
        private readonly GameMatchService _gameMatchService;
        private readonly BoostPool _boostPool;
        private readonly Collider[] _overlapResults = new Collider[10];
        private const string BoxLayerMask = "Box";
        private float _spawnInterval;
        
        public SpawnBoostsSystem(
            GameSceneHandler sceneHandler,
            GameSettingsConfig gameSettingsConfig,
            GameMatchService gameMatchService,
            BoostPool boostPool
        )
        {
            _sceneHandler = sceneHandler;
            _gameSettingsConfig = gameSettingsConfig;
            _gameMatchService = gameMatchService;
            _boostPool = boostPool;
        }
        
        public void Initialize()
        {
            _observer?.Dispose();

            _boundsArea = _sceneHandler.FieldView.Collider.bounds;
            _spawnInterval = _gameSettingsConfig.BoostSpawnInterval;

            _observer = Observable.FromCoroutine(SpawnBoostsWithDelay)
                .Subscribe();
        }

        private void SpawnBoostBox(EBoxBoost boostType)
        {
            if(boostType == EBoxBoost.None)
                return;
            
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
            
            var instance = _boostPool.GetBoost(boostType);
            instance.transform.position = spawnPosition;
            instance.isDestroyed = false;
            instance.gameObject.SetActive(true);
        }
        
        private IEnumerator SpawnBoostsWithDelay()
        {
            do
            {
                var delay = Random.Range(_spawnInterval, _spawnInterval);
                
                yield return new WaitForSeconds(delay);
                
                yield return new WaitUntil(() => _gameMatchService.IsGameRunning());

                var spawnCount = Random.Range(1, 2);

                for (var i = 0; i < spawnCount; i++)
                {
                    //var randomGrade = (EBoxBoost)Random.Range((int)EBoxBoost.Speed, (int)EBoxBoost.AddGrades);
                    var randomGrade = EBoxBoost.Speed.GetRandomEnumBetween(EBoxBoost.AddGrades);
                    Debug.Log($"randomGrade: {randomGrade}");
                    SpawnBoostBox(randomGrade);
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
        
        private bool IsPositionOccupied(Vector3 position)
        {
            var numColliders = Physics.OverlapSphereNonAlloc(position, 2f, _overlapResults, LayerMask.GetMask(BoxLayerMask));
            return numColliders > 0;
        }
        
        public void Dispose()
        {
            _observer?.Dispose();
        }
    }
}