using UnityEngine;

namespace DefaultNamespace
{
    public class CubeController : MonoBehaviour
    {
        [SerializeField] private InputManager _inputManager;

        private Vector3 _defaultPosition;
        private Vector3 _defaultRotation;
        private void Start()
        {
            _defaultPosition = transform.position;
            _defaultRotation = transform.rotation.eulerAngles;
            // _inputManager.OnRespawnCubePressed += OnResetCubePositionPressed;
        }

        private void OnResetCubePositionPressed()
        {
            transform.position = _defaultPosition;
            transform.eulerAngles = _defaultRotation;
        }

        private void OnDestroy()
        {
            // _inputManager.OnRespawnCubePressed -= OnResetCubePositionPressed;
        }
    }
}