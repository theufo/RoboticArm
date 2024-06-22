using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class RobotController : MonoBehaviour
    {
        [SerializeField] private float _speed = 20;
        [SerializeField] private ArticulationBody _articulationX;
        [SerializeField] private ArticulationBody _articulationZ;
        [SerializeField] private ArticulationBody _articulationY;
        [SerializeField] private ArticulationBody _articulationRotateY;
        [SerializeField] private GripperController _gripperController;
        [SerializeField] private InputManager _inputManager;

        private void Awake()
        {
            _gripperController.Initialize(_inputManager);
            _inputManager.OnResetPositionPressed += OnResetPositionPressed;
        }

        private void OnResetPositionPressed()
        {
            Helper.ForceSetTargetPosition(0, _articulationX.xDrive, (val) => _articulationX.xDrive = val);
            Helper.ForceSetTargetPosition(0, _articulationZ.zDrive, (val) => _articulationZ.zDrive = val);
            Helper.ForceSetTargetPosition(0, _articulationY.yDrive, (val) => _articulationY.yDrive = val);
            Helper.ForceSetTargetPosition(0, _articulationRotateY.xDrive, (val) => _articulationRotateY.xDrive = val);
            _gripperController.SetPosition(0);
        }

        private void FixedUpdate()
        {
            var deltaX = _inputManager.DeltaX;
            var deltaZ = _inputManager.DeltaZ;
            var deltaY = _inputManager.DeltaY;
            var rotateY = _inputManager.RotateY;

            _articulationX.SetTargetPosition(_speed, deltaX, _articulationX.xDrive, (val) => _articulationX.xDrive = val);
            _articulationZ.SetTargetPosition(_speed, deltaZ, _articulationZ.zDrive, (val) => _articulationZ.zDrive = val);
            _articulationY.SetTargetPosition(_speed, deltaY, _articulationY.yDrive, (val) => _articulationY.yDrive = val);
            _articulationRotateY.SetTargetRotation(_speed * 10, rotateY);
        }

        private void OnDestroy()
        {
            _inputManager.OnResetPositionPressed -= OnResetPositionPressed;
        }
    }
}