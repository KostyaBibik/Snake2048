using System;
using System.Collections.Generic;
using System.Linq;
using Database;
using Helpers;
using Services.Impl;
using UnityEngine;
using Views.Impl;
using Random = UnityEngine.Random;

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
            var targetIsDestroyed = _targetAimBox == null || _targetAimBox.isDestroyed;
            
            if (targetIsDestroyed || _timeSinceLastTargetUpdate >= _targetUpdateInterval)
            {
                if (Random.value < _randomChangeChance)
                {
                    _targetAimBox = FindRandomTargetBox(botView);
                }

                _timeSinceLastTargetUpdate = 0f;
            }

            if (!targetIsDestroyed
                && Vector3.Distance(botView.transform.position, _targetAimBox.transform.position) > _minimumTargetDistance)
            {
                MoveTowardsTarget(botView);
            }
            else
            {
                Wander(botView);
            }

            CheckBoundsAndAdjustDirection();
        }

        private void MoveTowardsTarget(BoxView botView)
        {
            var direction = (_targetAimBox.transform.position - botView.transform.position).normalized;
            _currentDirection = Vector3.Slerp(_currentDirection, direction, Time.deltaTime * _speed).normalized;
            _currentDirection.y = 0;
            var relatedSpeed = _speed * Time.deltaTime;
            var rb = botView.Rigidbody;
            rb.MovePosition(rb.position + _currentDirection * relatedSpeed);

            _timeSinceLastDirectionChange = 0f;
        }

        private void Wander(BoxView boxView)
        {
            _timeSinceLastDirectionChange += Time.deltaTime;

            if (_timeSinceLastDirectionChange >= _changeDirectionInterval)
            {
                _targetDirection = GetRandomDirection();
                _timeSinceLastDirectionChange = 0f;
            }

            _currentDirection = Vector3.Slerp(_currentDirection, _targetDirection, Time.deltaTime * _speed).normalized;
            _currentDirection.y = 0;
            var relatedSpeed = _speed * Time.deltaTime;
            var rb = boxView.Rigidbody;
            rb.MovePosition(rb.position + _currentDirection * relatedSpeed);
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
            var randomAngle = Random.Range(0, 360);
            return new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle)).normalized;
        }

        /*private BoxView FindRandomTargetBox(BoxView botView)
        {
            var validBoxes = _boxService.GetAllBoxes()
                .Where(box => box != botView)
                .Where(box => !_boxService.AreInSameTeam(botView, box))
                .Where(box => (box.isIdle && box.Grade <= botView.Grade) || (box.Grade < botView.Grade))
                .Where(box => HasHigherOrEqualGradeInTeam(box, botView))
                //.Where(box => BotPathEvaluator.IsPathSafe(_boxService, botView, box))
                .OrderBy(box => Vector3.Distance(botView.transform.position, box.transform.position))
                .Take(10) 
                .ToList();

            if (!validBoxes.Any())
            {
                return null;
            }

            var weightedBoxes = validBoxes.Select(box =>
            {
                var distance = Vector3.Distance(botView.transform.position, box.transform.position);
                var gradeFactor = botView.Grade - box.Grade;
                var weight = gradeFactor / distance;
                return (box, weight);
            }).OrderByDescending(x => x.weight).ToList();

            var randomValue = Random.Range(0, weightedBoxes.Count);
            return weightedBoxes[randomValue].box;
        }*/

        private BoxView FindRandomTargetBox(BoxView botView)
        {
            var allBoxes = _boxService.GetAllBoxes();
            var validBoxes = new List<(BoxView box, float distance)>();

            for (var i = 0; i < allBoxes.Count; i++)
            {
                var box = allBoxes[i];
                if (box == botView || _boxService.AreInSameTeam(botView, box))
                    continue;

                if ((box.isIdle && box.Grade <= botView.Grade) || (box.Grade < botView.Grade))
                {
                    if (HasHigherOrEqualGradeInTeam(box, botView))
                    {
                        var distance = Vector3.Distance(botView.transform.position, box.transform.position);
                        validBoxes.Add((box, distance));
                    }
                }
            }

            if (validBoxes.Count == 0)
            {
                return null;
            }

            validBoxes.Sort((a, b) => a.distance.CompareTo(b.distance));
            var topBoxes = new (BoxView box, float distance)[Math.Min(10, validBoxes.Count)];
            for (var i = 0; i < topBoxes.Length; i++)
            {
                topBoxes[i] = validBoxes[i];
            }

            var weightedBoxes = new List<(BoxView box, float weight)>();
            for (var i = 0; i < topBoxes.Length; i++)
            {
                var (box, distance) = topBoxes[i];
                var gradeFactor = botView.Grade - box.Grade;
                var weight = gradeFactor / distance;
                weightedBoxes.Add((box, weight));
            }

            weightedBoxes.Sort((a, b) => b.weight.CompareTo(a.weight));
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

            var highestGradeInTeam = team[0].Grade;
            for (var i = 1; i < team.Count; i++)
            {
                if (team[i].Grade > highestGradeInTeam)
                {
                    highestGradeInTeam = team[i].Grade;
                }
            }
            return highestGradeInTeam < selfBox.Grade;
        }

        public void ExitState(BoxContext context)
        {
        }
    }
}
