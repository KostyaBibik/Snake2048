using System.Linq;
using Components.Boxes;
using Database;
using Enums;
using Helpers;
using Installers;
using Services.Impl;
using UnityEngine;
using Views.Impl;
using Zenject;

namespace Infrastructure.Factories.Impl
{
    public class BoxEntityFactory : IEntityFactory<EBoxGrade, BoxView>
    {
        private readonly BoxPrefabsConfig _boxPrefabsConfig;
        
        private BoxService _boxService;
        private BoxStateFactory _boxStateFactory;
        
        public BoxEntityFactory(BoxPrefabsConfig boxPrefabsConfig)
        {
            _boxPrefabsConfig = boxPrefabsConfig;
        }
        
        [Inject]
        private void Construct(
            BoxStateFactory boxStateFactory,
            BoxService boxService
        )
        {
            _boxStateFactory = boxStateFactory;
            _boxService = boxService;
        }

        public BoxView Create(EBoxGrade grade)
        {
            var boxPrefab = _boxPrefabsConfig.GetPrefab(grade).view;
            var boxInstance = DiContainerRef.Container.InstantiatePrefabForComponent<BoxView>(boxPrefab);
            var boxTransform = boxInstance.transform;
            boxInstance.stateContext = new BoxContext(boxInstance);
            var state = _boxStateFactory.CreateIdleState();
            boxInstance.stateContext.SetState(state);

            boxTransform.position = Vector3.zero;
            boxTransform.rotation = Quaternion.identity;
            
            _boxService.AddEntityOnService(boxInstance);
            return boxInstance;
        }
    }
}