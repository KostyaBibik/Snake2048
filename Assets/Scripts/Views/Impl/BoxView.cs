using System;
using Components.Boxes;
using Enums;
using Signals;
using UnityEngine;
using Zenject;

namespace Views.Impl
{
    public class BoxView : MonoBehaviour, IEntityView
    {
        [SerializeField] private EBoxGrade grade;
        
        public EBoxGrade Grade => grade;
        
        public BoxContext stateContext;
        public bool isDestroyed { get; set; }
        public bool isPlayer { get; set; }

        private SignalBus _signalBus;
        
        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }
        
        private void Update()
        {
            stateContext?.Update();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!isPlayer)
                return;

            if(other.TryGetComponent(out BoxView enemyBox))
            {
                _signalBus.Fire(new EatBoxSignal
                {
                    eatenBox = enemyBox,
                    newOwner = this
                });
            }
        }
    }
}