using System;
using System.Collections.Generic;
using System.Linq;
using Systems.Action;
using Components.Boxes.Views.Impl;
using Database;
using Helpers;
using Services.Impl;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Components.Boxes.States.Impl
{
    public class BotMoveState : IBoxState
    {
        private readonly BoxService _boxService;
        private readonly GameSettingsConfig _settingsConfig;
        private readonly AccelerationBoxSystem _accelerationBoxSystem;
        private readonly BotsSettingsConfig _botsSettingsConfig;
        private readonly float _targetUpdateInterval = .7f;

        private float _accelerationSpeed;
        private float _boostSpeed;
        private Transform _botTransform;
        private float _changeDirectionInterval;
        private Vector3 _currentDirection;

        private float _currentSpeed;
        private Bounds _gameBounds;

        private float _minimumTargetDistance = 1f;
        private float _randomChangeChance;
        private float _accelerationChance;

        private float _speed;

        private BoxView _targetAimBox;
        private Vector3 _targetDirection;
        private float _timeSinceLastDirectionChange;
        private float _timeSinceLastTargetUpdate;
        private float _gradeInfluence;
        private float _distanceInfluence;

        public BotMoveState(
            BoxService boxService,
            GameSettingsConfig settingsConfig,
            GameSceneHandler sceneHandler,
            AccelerationBoxSystem accelerationBoxSystem,
            BotsSettingsConfig botsSettingsConfig
        )
        {
            _boxService = boxService;
            _settingsConfig = settingsConfig;
            _accelerationBoxSystem = accelerationBoxSystem;
            _botsSettingsConfig = botsSettingsConfig;
            _gameBounds = sceneHandler.FieldView.Collider.bounds;
        }

        public void EnterState(BoxContext context)
        {
            _botTransform = context.BoxView.transform;

            InitParameters();
        }

        private void InitParameters()
        {
            _timeSinceLastTargetUpdate = 0f;
            _timeSinceLastDirectionChange = 0f;

            _changeDirectionInterval = _botsSettingsConfig.ChangeRandomDirectionInterval;
            _gradeInfluence = _botsSettingsConfig.GradeInfluenceForFindTarget;
            _distanceInfluence = _botsSettingsConfig.DistanceInfluenceForFindTarget;
            _accelerationChance = _botsSettingsConfig.IntiAccelerationChance;
            _randomChangeChance = .3f;
            
            _speed = _settingsConfig.BoxMoveSpeed;
            _boostSpeed = _settingsConfig.BoxBoostSpeed;
            _accelerationSpeed = _settingsConfig.BoxAccelerationSpeed;
            
            _currentDirection = _botTransform.forward;
            _targetDirection = GetRandomDirection();
        }
        
        public void UpdateState(BoxContext context)
        {
            var botView = context.BoxView;

            _currentSpeed = CalcCurrentSpeed(botView);

            _timeSinceLastTargetUpdate += Time.deltaTime;
            var targetIsDestroyed = _targetAimBox == null || _targetAimBox.isDestroyed;

            if (targetIsDestroyed || _timeSinceLastTargetUpdate >= _targetUpdateInterval)
            {
                if (Random.value < _randomChangeChance)
                    _targetAimBox = FindRandomTargetBox(botView);

                _timeSinceLastTargetUpdate = 0f;
            }

            if (!targetIsDestroyed)
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
            var relatedSpeed = _currentSpeed * Time.deltaTime;
            var rb = botView.Rigidbody;

            _currentDirection = Vector3.Slerp(_currentDirection, direction, relatedSpeed).normalized;
            _currentDirection.y = 0;
            var targetVector = _currentDirection * relatedSpeed;
            targetVector.y = 0;

            rb.velocity = targetVector;

            _timeSinceLastDirectionChange = 0f;

            if (_currentDirection == Vector3.zero)
                return;

            var targetRotation = Quaternion.LookRotation(_currentDirection);
            botView.transform.rotation = Quaternion.Slerp(botView.transform.rotation, targetRotation, relatedSpeed);
            
            if (Random.value < _accelerationChance && !botView.IsAccelerationActive)
            {
                _accelerationBoxSystem.ChangeAccelerationStatusForBotTeam(botView, true);
            }
        }

        private void Wander(BoxView botView)
        {
            var relatedSpeed = _currentSpeed * Time.deltaTime;
            var rb = botView.Rigidbody;

            _timeSinceLastDirectionChange += Time.deltaTime;

            if (_timeSinceLastDirectionChange >= _changeDirectionInterval)
            {
                _targetDirection = GetRandomDirection();
                _timeSinceLastDirectionChange = 0f;
            }

            _currentDirection = Vector3.Slerp(_currentDirection, _targetDirection, relatedSpeed).normalized;
            _currentDirection.y = 0;

            var targetVector = _currentDirection * relatedSpeed;
            targetVector.y = 0;

            rb.velocity = targetVector;

            if (_currentDirection == Vector3.zero)
                return;

            var targetRotation = Quaternion.LookRotation(_currentDirection);
            botView.transform.rotation = Quaternion.Slerp(botView.transform.rotation, targetRotation, relatedSpeed);
        }

        private void CheckBoundsAndAdjustDirection()
        {
            var position = _botTransform.position;
            var newDirection = _currentDirection;
            var relatedSpeed = _currentSpeed * Time.deltaTime;

            if (position.x < _gameBounds.min.x || position.x > _gameBounds.max.x ||
                position.z < _gameBounds.min.z || position.z > _gameBounds.max.z)
            {
                newDirection = Vector3.Reflect(_currentDirection, _gameBounds.ClosestPoint(position));
                newDirection.y = 0;
            }

            if (newDirection != _currentDirection)
            {
                _currentDirection = Vector3.Slerp(_currentDirection, newDirection, relatedSpeed).normalized;
                _currentDirection.y = 0;
            }
        }

        private float CalcCurrentSpeed(BoxView view)
        {
            var speed = _speed;

            if (view.IsSpeedBoosted)
                speed = _speed + _boostSpeed;
            else if (view.IsAccelerationActive)
                speed = _speed + _accelerationSpeed;

            return speed;
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
            var allBoxes = _boxService.GetAllTeams().Select(b => b.Leader).ToArray();
            var validBoxes = new List<(BoxView box, float distance)>();

            for (var i = 0; i < allBoxes.Length; i++)
            {
                var box = allBoxes[i];
                if (box == botView || _boxService.AreInSameTeam(botView, box))
                    continue;

                if (box.isIdle && box.Grade <= botView.Grade || box.Grade < botView.Grade)
                    if (HasHigherOrEqualGradeInTeam(box, botView))
                    {
                        var distance = Vector3.Distance(botView.transform.position, box.transform.position);
                        validBoxes.Add((box, distance));
                    }
            }

            if (validBoxes.Count == 0) 
                return null;

            var topBoxes = new (BoxView box, float distance)[Math.Min(10, validBoxes.Count)];
            for (var i = 0; i < topBoxes.Length; i++)
            {
                topBoxes[i] = validBoxes[i];
            }

            var weightedBoxes = new List<(BoxView box, float weight)>();

            for (var i = 0; i < topBoxes.Length; i++)
            {
                var (box, distance) = topBoxes[i];
                var gradeFactor = botView.Grade.IndexDifference(box.Grade);

                gradeFactor = Mathf.Max(1, gradeFactor);
                
                var invertedGradeFactor = 1.0f / gradeFactor;
                var weight = (float)(Math.Pow(invertedGradeFactor, _gradeInfluence)) / (distance * _distanceInfluence);
                
                weightedBoxes.Add((box, weight));
            }

            weightedBoxes.Sort((a, b) => a.weight.CompareTo(b.weight));

            var totalWeight = weightedBoxes.Sum(wb => wb.weight);
            var randomValue = Random.Range(0, totalWeight);
            var cumulativeWeight = 0f;

            foreach (var (box, weight) in weightedBoxes)
            {
                cumulativeWeight += weight;
                if (randomValue <= cumulativeWeight) 
                    return box;
            }

            return weightedBoxes.Last().box; 
        }


        private bool HasHigherOrEqualGradeInTeam(BoxView targetBox, BoxView selfBox)
        {
            if (targetBox.isIdle)
                return true;

            return targetBox.Grade <= selfBox.Grade;
        }

        public void ExitState(BoxContext context)
        {
        }
    }
}