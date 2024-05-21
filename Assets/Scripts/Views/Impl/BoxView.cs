using System;
using Components.Boxes;
using Enums;
using Signals;
using TMPro;
using UnityEngine;
using Zenject;

namespace Views.Impl
{
    public class BoxView : MonoBehaviour, IEntityView
    {
        [SerializeField] private EBoxGrade grade;
        [SerializeField] private TextMeshProUGUI _nickText;

        public EBoxGrade Grade => grade;

        public BoxContext stateContext;
        public bool isDestroyed { get; set; }
        public bool isPlayer { get; set; }
        public bool isIdle { get; set; }
        public bool isBot { get; set; }
        public bool isMerging { get; set; }

        private SignalBus _signalBus;

        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void SetNickname(string nick, bool activeText = true)
        {
            _nickText.gameObject.SetActive(activeText);
            _nickText.text = nick;
        }
        
        private void Update()
        {
            stateContext?.Update();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(isIdle)
                return;
            
            if(isDestroyed)
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