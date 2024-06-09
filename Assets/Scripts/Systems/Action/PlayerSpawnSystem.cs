using System;
using Enums;
using Helpers;
using Infrastructure.Factories.Impl;
using Infrastructure.Pools.Impl;
using Services;
using Services.Impl;
using Signals;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Systems.Action
{
    public class PlayerSpawnSystem : IInitializable, IDisposable
    {
        private readonly BoxPool _boxPool;
        private readonly BoxStateFactory _boxStateFactory;
        private readonly BoxService _boxService;
        private readonly GameMatchService _gameMatchService;
        private readonly SignalBus _signalBus;
        private readonly Collider[] _overlapResults = new Collider[10];
        private readonly Bounds _fieldBounds;

        private float _minSpawnDistance;
        private const string BoxLayerMask = "Box";
        
        private PlayerSpawnSystem(
            BoxPool boxPool,
            BoxStateFactory boxStateFactory,
            BoxService boxService,
            GameSceneHandler gameSceneHandler,
            GameMatchService gameMatchService, 
            SignalBus signalBus
        )
        {
            _boxPool = boxPool;
            _boxStateFactory = boxStateFactory;
            _boxService = boxService;
            _gameMatchService = gameMatchService;
            _signalBus = signalBus;
            _fieldBounds = gameSceneHandler.FieldView.Collider.bounds;
        }
        
        public void Initialize()
        {
            _minSpawnDistance = 2f;
            
            _signalBus.Subscribe<PlayerSpawnSignal>(SpawnPlayer);
        }

        private void SpawnPlayer(PlayerSpawnSignal signal)
        {
            var boxView = _boxPool.GetBox(signal.grade);

            var state = _boxStateFactory.CreateMoveState();
            var pos = GetPosToSpawn();
            var nick = _gameMatchService.playerNickname.Value;
            boxView.stateContext.SetState(state);
            boxView.isPlayer = true;
            boxView.SetNickname(nick);
            boxView.transform.position = pos;
            boxView.gameObject.SetActive(true);
            _boxService.RegisterNewTeam(boxView, nick);

            _signalBus.Fire(new CameraUpdateSignal
            {
                followBox = boxView
            });
        }

        private Vector3 GetPosToSpawn()
        {
            var attempts = 0;
            while (true)
            {
                var spawnPosition = GetRandomPositionInBounds(_fieldBounds);
                attempts++;
                
                if(!IsPositionOccupied(spawnPosition))
                {
                    return spawnPosition;
                }
                
                if (attempts >= 25)
                {
                    return Vector3.zero;
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
        
        public void Dispose()
        {
            _signalBus.Unsubscribe<PlayerSpawnSignal>(SpawnPlayer);
        }
    }
}