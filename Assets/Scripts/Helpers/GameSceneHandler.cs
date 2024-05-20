using Components.Field;
using UnityEngine;

namespace Helpers
{
    public class GameSceneHandler : MonoBehaviour
    {
        [SerializeField] private FieldView fieldView;
        public FieldView FieldView => fieldView;
    }
}