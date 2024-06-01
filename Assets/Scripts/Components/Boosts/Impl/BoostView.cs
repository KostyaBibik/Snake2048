using System;
using Components.Boxes.Views.Impl;
using Enums;
using Infrastructure.Pools;
using Services;
using Signals;
using UnityEngine;
using Zenject;

namespace Components.Boosts.Impl
{
    public class BoostView : MonoBehaviour, IBoostView
    {
        [SerializeField] private EBoxBoost boostType;

        private SignalBus _signalBus;
        private BoostPool _boostPool;
        
        public bool isDestroyed { get; set; }
        public EBoxBoost BoostType => boostType;

        [Inject]
        private void Construct(
            SignalBus signalBus,
            BoostPool pool
            )
        {
            _signalBus = signalBus;
            _boostPool = pool;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isDestroyed)
                return;
            
            if (other.transform.parent && other.transform.parent.TryGetComponent(out BoxView box))
            {
                _signalBus.Fire(new BoxBoostSignal
                {
                    box = box,
                    boostType = BoostType
                });
                
                _boostPool.ReturnToPool(this, BoostType);
            }
        }
    }
}