using System;
using System.Linq;
using Infrastructure.Factories.Impl;
using Services.Impl;
using Signals;
using Zenject;

namespace Systems.Runtime
{
    public class MergeBoxSystem : IInitializable, IDisposable
    {
        private readonly BoxService _boxService;
        private readonly SignalBus _signalBus;
        private readonly BoxStateFactory _boxStateFactory;

        public MergeBoxSystem(
            SignalBus signalBus,
            BoxService boxService,
            BoxStateFactory boxStateFactory
        )
        {
            _signalBus = signalBus;
            _boxService = boxService;
            _boxStateFactory = boxStateFactory;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<MergeBoxSignal>(OnMergeBoxSignal);
        }

        private void OnMergeBoxSignal(MergeBoxSignal mergeSignal)
        {
            var mergingBox = mergeSignal.mergingBox;

            var team = _boxService.GetTeam(mergingBox);
            var targetBox = team.FirstOrDefault(box => box.Grade == mergingBox.Grade && box != mergingBox);
            
            if (targetBox == null)
            {
                return;
            }

            var state = _boxStateFactory.CreateMergeState(targetBox, mergingBox.Grade);
            mergingBox.stateContext.SetState(state);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<MergeBoxSignal>(OnMergeBoxSignal);
        }
    }
}