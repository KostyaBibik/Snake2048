using Components.Boxes;
using UnityEngine;

namespace Components.Field
{
    [RequireComponent(typeof(Collider))]
    public class FieldView : MonoBehaviour
    {
        private BoxSpawnWaypoint[] _waypoints = new BoxSpawnWaypoint[]{};

        private Collider _collider;

        public Collider Collider => _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }
        
        public BoxSpawnWaypoint[] GetWaypoints()
        {
            if (_waypoints.Length <= 0)
            {
                _waypoints = GetComponentsInChildren<BoxSpawnWaypoint>();
            }

            return _waypoints;
        }
    }
}