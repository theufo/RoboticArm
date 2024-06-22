using UnityEngine;

namespace DefaultNamespace
{
    public class FingerController : MonoBehaviour
    {
        [SerializeField] private float _closedZ;
        [SerializeField] private ArticulationBody _articulation;

        private Vector3 _openPosition;
        
        private void Start()
        {
            _openPosition = transform.localPosition;
            
            //set limits
            var openZTarget = GetTargetZPosition(0.0f);
            var closeZTarget = GetTargetZPosition(1.0f);
            var min = Mathf.Min(openZTarget, closeZTarget);
            var max = Mathf.Max(openZTarget, closeZTarget);

            var drive = _articulation.zDrive;
            drive.lowerLimit = min;
            drive.upperLimit = max;
            _articulation.zDrive = drive;
        }

        public void UpdateGrip(float grip)
        {
            var targetZ = GetTargetZPosition(grip);
            
            var drive = _articulation.zDrive;
            drive.target = targetZ;
            _articulation.zDrive = drive;
        }

        public float GetCurrentGrip()
        {
            return Mathf.InverseLerp(_openPosition.z, _closedZ, transform.localPosition.z);
        }

        public void SetPosition(int value)
        {
            Helper.ForceSetTargetPosition(value, _articulation.zDrive, (val) => _articulation.zDrive = val);
        }

        private float GetTargetZPosition(float grip)
        {
            var zPosition = Mathf.Lerp(_openPosition.z, _closedZ, grip);
            var targetZ = (zPosition - _openPosition.z) * transform.parent.localScale.z;
            
            return targetZ;
        }
    }
}