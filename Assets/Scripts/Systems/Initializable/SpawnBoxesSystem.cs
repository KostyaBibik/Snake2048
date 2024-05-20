using System;
using System.Collections;
using Enums;
using Infrastructure.Factories.Impl;
using Services.Impl;
using UniRx;
using UnityEngine;
using Zenject;

namespace Systems.Initializable
{
    public class SpawnBoxesSystem : IInitializable, IDisposable
    {
        private IDisposable _observer;
        private readonly BoxEntityFactory _boxEntityFactory;
        private readonly BoxService _boxService;
        
        private SpawnBoxesSystem(
            BoxEntityFactory boxEntityFactory,
            BoxService boxService
        )
        {
            _boxEntityFactory = boxEntityFactory;
            _boxService = boxService;
        }
        
        public void Initialize()
        {
            _observer?.Dispose();
            
            _observer = Observable.FromCoroutine(SpawnBoxesWithDelay)
                .Subscribe();
        }

        private IEnumerator SpawnBoxesWithDelay()
        {
            var delay = new WaitForSeconds(5f);
            
            do
            {
                yield return delay;
                var idleBox = _boxEntityFactory.Create(EBoxGrade.Grade_2);
                _boxService.RegisterNewTeam(idleBox);

            } while (true);
            
            yield return null;
        }

        public void Dispose()
        {
            _observer?.Dispose();
        }
    }
}