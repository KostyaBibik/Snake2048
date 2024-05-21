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
        private readonly Vector3[] _waypoints;
        private readonly BoxPrefabsConfig _boxPrefabsConfig;
        
        private BoxService _boxService;
        private BoxStateFactory _boxStateFactory;
        
        public BoxEntityFactory(
            GameSceneHandler sceneHandler,
            BoxPrefabsConfig boxPrefabsConfig
        )
        {
            _waypoints = sceneHandler.FieldView.GetWaypoints().Select(t => t.transform.position).ToArray();

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
            var pos = _waypoints[Random.Range(0, _waypoints.Length)];
            var boxTransform = boxInstance.transform;
            boxInstance.stateContext = new BoxContext(boxInstance);
            var state = _boxStateFactory.CreateIdleState();
            boxInstance.stateContext.SetState(state);

            boxTransform.position = pos;
            boxTransform.rotation = Quaternion.identity;
            
            _boxService.AddEntityOnService(boxInstance);
            return boxInstance;
        }
    }
}