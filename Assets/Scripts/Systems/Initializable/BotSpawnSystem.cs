using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Database;
using Services.Impl;
using Enums;
using Helpers;
using Infrastructure.Factories.Impl;
using Services;
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
    private readonly NicknamesConfig _nicknamesConfig;
    private readonly GameSceneHandler _sceneHandler;
    private readonly GameMatchService _gameMatchService;

    private readonly Collider[] _overlapResults = new Collider[10];
    private const string BoxLayerMask = "Box";
    
    public BotSpawnSystem(
        BoxPool boxPool,
        BoxStateFactory boxStateFactory,
        BoxService boxService,
        BotService botService,
        GameSettingsConfig gameSettingsConfig,
        NicknamesConfig nicknamesConfig,
        GameSceneHandler sceneHandler,
        GameMatchService gameMatchService
    )
    {
        _boxPool = boxPool;
        _boxStateFactory = boxStateFactory;
        _boxService = boxService;
        _botService = botService;
        _gameSettingsConfig = gameSettingsConfig;
        _nicknamesConfig = nicknamesConfig;
        _sceneHandler = sceneHandler;
        _gameMatchService = gameMatchService;
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
        Observable.FromCoroutine(InitialSpawn).Subscribe();
    }
    
    private IEnumerator InitialSpawn()
    {
        yield return new WaitUntil(() => _gameMatchService.EGameModeStatus == EGameModeStatus.Play);
            
        var playerBoxView = _boxService.GetAllBoxes().FirstOrDefault(box => box.isPlayer);
        var playerHighGrade = _boxService.GetHighGradeInTeam(playerBoxView);

        for (var i = 0; i < _initialBotCount; i++)
        {
            var botGrade = CalculateBotGrade(playerHighGrade);
            SpawnBot(botGrade);
        }
    }
    
    private void SpawnBot(EBoxGrade eBoxGrade)
    {
        if(eBoxGrade == EBoxGrade.None)
            return;
        
        var attempts = 0;
        while (true)
        {
            var spawnPosition = GetRandomPositionInBounds(_locationBounds);
            if (!IsPositionOccupied(spawnPosition))
            {
                var bot = _boxPool.GetBox(eBoxGrade); 
                
                bot.isBot = true;
                bot.gameObject.name = "Bot"; 
                bot.transform.position = spawnPosition;
                var nick = _nicknamesConfig.GetRandomNickname();
                bot.SetNickname(nick);
                
                var state = _boxStateFactory.CreateBotMoveState();
                bot.stateContext.SetState(state);
                
                _botService.AddEntityOnService(bot);
                _boxService.AddEntityOnService(bot);
                _boxService.RegisterNewTeam(bot, nick);

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
        var saveGrade = EBoxGrade.None;
        
        while (true)
        {
            yield return delay;

            yield return new WaitUntil(() => _boxService.GetBotTeamsCount() < _maxBotCount);
            
            yield return new WaitUntil(() => _gameMatchService.EGameModeStatus == EGameModeStatus.Play);

            var playerBoxView = _boxService.GetAllBoxes().FirstOrDefault(box => box.isPlayer);
            if (playerBoxView == null)
            {
                SpawnBot(saveGrade);
                continue;
            }
            
            var playerHighGrade = _boxService.GetHighGradeInTeam(playerBoxView);
            var botGrade = CalculateBotGrade(playerHighGrade);
            saveGrade = botGrade;
            
            SpawnBot(botGrade);
        }
    }
    
    private EBoxGrade CalculateBotGrade(EBoxGrade playerHighGrade)
    {
        var probabilityGradeAdd3 = 0.15f;
        var probabilityGradeAdd2 = 0.2f;
        var probabilityGradeAdd1 = 0.15f;

        var randomValue = Random.value;

        if (randomValue < probabilityGradeAdd3)
        {
            return playerHighGrade.NextSteps(3);
        }
        else if (randomValue < probabilityGradeAdd3 + probabilityGradeAdd2)
        {
            return playerHighGrade.NextSteps(2);
        }
        else if (randomValue < probabilityGradeAdd3 + probabilityGradeAdd2 + probabilityGradeAdd1)
        {
            return playerHighGrade.Next();
        }
        else
        {
            return EBoxGrade.Grade_2.GetRandomEnumBetween(playerHighGrade);
        }
    }
    
    public void Dispose()
    {
        _observer?.Dispose();
    }
}