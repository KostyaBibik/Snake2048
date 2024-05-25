using System;
using UnityEngine;
using System.Collections;
using Database;
using Services.Impl;
using Enums;
using Helpers;
using Infrastructure.Factories.Impl;
using UniRx;
using IInitializable = Zenject.IInitializable;
using Random = UnityEngine.Random;

public class BotSpawnSystem : IInitializable, IDisposable
{
    private int _initialBotCount;
    private int _maxBotCount;
    private float _minSpawnDistance;
    private float _spawnInterval;
    private Bounds _locationBounds;
    private IDisposable _observer;

    private readonly BoxPool _boxPool;
    private readonly BoxStateFactory _boxStateFactory;
    private readonly BoxService _boxService;
    private readonly BotService _botService;
    private readonly GameSettingsConfig _gameSettingsConfig;
    private readonly GameSceneHandler _sceneHandler;

    private readonly Collider[] _overlapResults = new Collider[10];
    private const string BoxLayerMask = "Box";
    
    public BotSpawnSystem(
        BoxPool boxPool,
        BoxStateFactory boxStateFactory,
        BoxService boxService,
        BotService botService,
        GameSettingsConfig gameSettingsConfig,
        GameSceneHandler sceneHandler
    )
    {
        _boxPool = boxPool;
        _boxStateFactory = boxStateFactory;
        _boxService = boxService;
        _botService = botService;
        _gameSettingsConfig = gameSettingsConfig;
        _sceneHandler = sceneHandler;
    }

    public void Initialize()
    {
        _initialBotCount = _gameSettingsConfig.InitialBotCount;
        _maxBotCount = _gameSettingsConfig.MaxCountBots;
        _minSpawnDistance = _gameSettingsConfig.MinSpawnDistance;
        _spawnInterval = _gameSettingsConfig.BotSpawnInterval;
        _locationBounds = _sceneHandler.FieldView.Collider.bounds;

        SpawnInitialBots();
        
        _observer?.Dispose();

        _observer = Observable.FromCoroutine(SpawnBotsWithDelay)
            .Subscribe();
    }

    private void SpawnInitialBots()
    {
        for (var i = 0; i < _initialBotCount; i++)
        {
            SpawnBot();
        }
    }
    
    private void SpawnBot()
    {
        var attempts = 0;
        while (true)
        {
            var spawnPosition = GetRandomPositionInBounds(_locationBounds);
            if (!IsPositionOccupied(spawnPosition))
            {
                var bot = _boxPool.GetBox(EBoxGrade.Grade_2); 
                
                bot.isBot = true;
                bot.gameObject.name = "Bot"; 
                bot.transform.position = spawnPosition;
                bot.SetNickname("Bot");
                
                var state = _boxStateFactory.CreateBotMoveState();
                bot.stateContext.SetState(state);
                
                _botService.AddEntityOnService(bot);
                _boxService.AddEntityOnService(bot);
                _boxService.RegisterNewTeam(bot);
                
                break;
            }

            attempts++;
            if (attempts >= 10)
            {
                break;
            }
        }
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
        var numColliders = Physics.OverlapSphereNonAlloc(position, _minSpawnDistance, _overlapResults, LayerMask.GetMask(BoxLayerMask));
        return numColliders > 0;
    }
    
    private IEnumerator SpawnBotsWithDelay()
    {
        var delay = new WaitForSeconds(_spawnInterval);

        while (true)
        {
            yield return delay;

            yield return new WaitUntil(() => _botService.GetBotCount() < _maxBotCount);
            
            SpawnBot();
        }
    }
    
    public void Dispose()
    {
        _observer?.Dispose();
    }
}