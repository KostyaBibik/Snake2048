using System.Linq;
using Database;
using Helpers;
using Services.Impl;
using UnityEngine;
using Views.Impl;

namespace Components.Boxes.States.Impl
{
    public class BotMoveState : IBoxState
    {
        private readonly BoxService _boxService;
        private readonly GameSettingsConfig _settingsConfig;

        private float _speed;
        private float _changeDirectionInterval; 
        private float _timeSinceLastDirectionChange;
        private Vector3 _targetDirection;
        private Vector3 _currentDirection;
        private Transform _botTransform;

        private BoxView _targetAimBox;
        private float _targetUpdateInterval = 1f; 
        private float _timeSinceLastTargetUpdate;

        private float _minimumTargetDistance = 1f; 
        private float _randomChangeChance = 0.1f; 

        private Bounds _gameBounds;

        public BotMoveState(
            BoxService boxService,
            GameSettingsConfig settingsConfig,
            GameSceneHandler sceneHandler
        )
        {
            _boxService = boxService;
            _settingsConfig = settingsConfig;
            _gameBounds = sceneHandler.FieldView.Collider.bounds;
        }

        public void EnterState(BoxContext context)
        {
            _botTransform = context.BoxView.transform;
            _speed = _settingsConfig.BoxMoveSpeed;
            _timeSinceLastDirectionChange = 0f;
            _changeDirectionInterval = .25f;
            _targetDirection = GetRandomDirection();
            _timeSinceLastTargetUpdate = 0f;
            _currentDirection = _botTransform.forward; 
        }

        public void UpdateState(BoxContext context)
        {
            var botView = context.BoxView;
            _timeSinceLastTargetUpdate += Time.deltaTime;

            if (_targetAimBox == null || !_targetAimBox.gameObject.activeInHierarchy || 
                _timeSinceLastTargetUpdate >= _targetUpdateInterval)
            {
                if (_targetAimBox == null || !_targetAimBox.gameObject.activeInHierarchy)
                {
                    _targetAimBox = FindRandomTargetBox(botView);
                }
                else if (_timeSinceLastTargetUpdate >= _targetUpdateInterval && Random.value < _randomChangeChance)
                {
                    _targetAimBox = FindRandomTargetBox(botView);
                }

                _timeSinceLastTargetUpdate = 0f;
            }

            if (_targetAimBox != null && Vector3.Distance(botView.transform.position, _targetAimBox.transform.position) > _minimumTargetDistance)
            {
                var direction = (_targetAimBox.transform.position - botView.transform.position).normalized;
                _currentDirection = Vector3.Slerp(_currentDirection, direction, Time.deltaTime * _speed).normalized;
                _currentDirection.y = 0;
                botView.transform.Translate(_currentDirection * (_speed * Time.deltaTime), Space.World);
                _timeSinceLastDirectionChange = 0f;
            }
            else
            {
                _timeSinceLastDirectionChange += Time.deltaTime;

                if (_timeSinceLastDirectionChange >= _changeDirectionInterval)
                {
                    _targetDirection = GetRandomDirection();
                    _timeSinceLastDirectionChange = 0f;
                }
                
                _currentDirection = Vector3.Slerp(_currentDirection, _targetDirection, Time.deltaTime * _speed).normalized;
                _currentDirection.y = 0;
                
                _botTransform.Translate(_currentDirection * (_speed * Time.deltaTime), Space.World);
            }

            CheckBoundsAndAdjustDirection();
        }

        private void CheckBoundsAndAdjustDirection()
        {
            var position = _botTransform.position;
            var newDirection = _currentDirection;

            if (position.x < _gameBounds.min.x || position.x > _gameBounds.max.x ||
                position.z < _gameBounds.min.z || position.z > _gameBounds.max.z)
            {
                newDirection = Vector3.Reflect(_currentDirection, _gameBounds.ClosestPoint(position));
                newDirection.y = 0;
            }

            if (newDirection != _currentDirection)
            {
                _currentDirection = Vector3.Slerp(_currentDirection, newDirection, Time.deltaTime * _speed).normalized;
            }
        }

        private Vector3 GetRandomDirection()
        {
            var direction = new Vector3(
                Random.Range(_currentDirection.x - 1f, _currentDirection.x + 1f),
                0,
                Random.Range(_currentDirection.z - 1F, _currentDirection.z + 1f)
            ).normalized;
            return direction;
        }
        
        private BoxView FindRandomTargetBox(BoxView botView)
        {
            var boxes = _boxService.GetAllBoxes()
                .Where(box => box != botView)
                .Where(box => !_boxService.AreInSameTeam(botView, box))
                .Where(box => (box.isIdle && box.Grade <= botView.Grade) || (box.Grade < botView.Grade))
                .Where(box => HasHigherOrEqualGradeInTeam(box, botView))
                .Where(box => BotPathEvaluator.IsPathSafe(_boxService, botView, box))
                .OrderBy(box => Vector3.Distance(botView.transform.position, box.transform.position))
                .ToList();

            if (!boxes.Any())
            {
                return null;
            }

            var weightedBoxes = boxes.Select(box =>
            {
                var distance = Vector3.Distance(botView.transform.position, box.transform.position);
                var gradeFactor = botView.Grade - box.Grade;
                var weight = gradeFactor / distance;
                return (box, weight);
            }).OrderByDescending(x => x.weight).ToList();

            var randomValue = Random.Range(0, weightedBoxes.Count);
            return weightedBoxes[randomValue].box;
        }

        private bool HasHigherOrEqualGradeInTeam(BoxView targetBox, BoxView selfBox)
        {
            if (targetBox.isIdle)
                return true;
            
            var team = _boxService.GetTeam(targetBox);
            if (!team.Any())
                return false;
            
            var highestGradeInTeam = team.Max(teammate => teammate.Grade);
            return highestGradeInTeam < selfBox.Grade;
        }
        
        public void ExitState(BoxContext context)
        {
        }
    }
}
