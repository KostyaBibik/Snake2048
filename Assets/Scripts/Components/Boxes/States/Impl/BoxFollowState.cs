using System.Collections.Generic;
using UnityEngine;

namespace Components.Boxes.States.Impl
{
    public class BoxFollowState : IBoxState
    {
        private Transform _leader;
        private float speed = 5f;
        private float _followDistance = 1;
        private readonly Queue<Vector3> _positionHistory;
        private readonly int _historyLength = 10;
        
        public BoxFollowState(Transform leader)
        {
            _leader = leader;
            _positionHistory = new Queue<Vector3>();
        }

        public void EnterState(BoxContext context)
        {
            for (var i = 0; i < _historyLength; i++)
            {
                _positionHistory.Enqueue(_leader.position);
            }
        }

        public void UpdateState(BoxContext context)
        {
            if (_leader == null)
                return;
            
            if (_positionHistory.Count >= _historyLength)
            {
                _positionHistory.Dequeue();
            }
            _positionHistory.Enqueue(_leader.position);
            
            var targetPosition = _positionHistory.Peek();
            var boxTransform = context.BoxView.transform;
            
            float distanceToTarget = Vector3.Distance(boxTransform.position, targetPosition);
            if (distanceToTarget < _followDistance)
            {
                return;
            }
            
            var newPosition = Vector3.Lerp(boxTransform.position, targetPosition, speed * Time.deltaTime);
            boxTransform.position = newPosition;
            
            var direction = (targetPosition - boxTransform.position).normalized;
            direction.y = 0; 
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                boxTransform.rotation = Quaternion.Slerp(boxTransform.rotation, targetRotation, speed * Time.deltaTime);
            }
        }

        public void ExitState(BoxContext context)
        {
        }
    }
}