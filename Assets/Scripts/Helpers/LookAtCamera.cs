using UnityEngine;
using Zenject;

namespace Helpers
{
    public class LookAtCamera : MonoBehaviour
    {
        [Inject] private Camera mainCamera;

        void Update()
        {
            if (mainCamera != null)
            {
                transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                    mainCamera.transform.rotation * Vector3.up);
            }
        }
    }
}