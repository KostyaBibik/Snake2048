using Components.Boxes;
using UnityEngine;

namespace Components.Field
{
    public class FieldView : MonoBehaviour
    {
        private BoxSpawnWaypoint[] _waypoints = new BoxSpawnWaypoint[]{};

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