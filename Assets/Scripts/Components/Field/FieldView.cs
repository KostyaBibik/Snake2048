using UnityEngine;

namespace Components.Field
{
    [RequireComponent(typeof(Collider))]
    public class FieldView : MonoBehaviour
    {
        private Collider _collider;

        public Collider Collider => _collider == null
            ? InitCollider()
            : _collider;

        private void Awake()
        {
            InitCollider();
        }

        private Collider InitCollider()
        {
            if (_collider == null)
            {
                _collider = GetComponent<Collider>();
            }

            return _collider;
        }
    }
}