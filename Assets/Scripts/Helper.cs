using System;
using UnityEngine;

namespace DefaultNamespace
{
    public static class Helper
    { 
        public static void SetTargetPosition(this ArticulationBody body, float speed, int delta, ArticulationDrive articulationDrive, Func<ArticulationDrive, ArticulationDrive> func)
        {
            if (delta == 0) return;
        
            var xDrivePosition = body.jointPosition[0];
            var targetPosition = xDrivePosition + delta * Time.fixedDeltaTime * speed;

            articulationDrive.target = targetPosition;
            func?.Invoke(articulationDrive);
        }
        
        public static void ForceSetTargetPosition(float target, ArticulationDrive articulationDrive, Func<ArticulationDrive, ArticulationDrive> func)
        {
            var targetPosition = target;

            articulationDrive.target = targetPosition;
            func?.Invoke(articulationDrive);
        }

        public static void SetTargetRotation(this ArticulationBody body, float speed, float delta)
        {
            if (delta == 0) return;
            
            var rotationChange = delta * speed * Time.fixedDeltaTime;
            var currentRotationRads = body.jointPosition[0];
            var currentRotation = Mathf.Rad2Deg * currentRotationRads;

            var drive = body.xDrive;
            drive.target = currentRotation + rotationChange;
            body.xDrive = drive;
        }
    }
}