using DefaultNamespace;
using UnityEngine;

public class GripperController : MonoBehaviour
{
    [SerializeField] private float _speed = 20;
    [SerializeField] private FingerController _fingerAController;
    [SerializeField] private FingerController _fingerBController;

    private InputManager _inputManager;

    public void Initialize(InputManager inputManager)
    {
        _inputManager = inputManager;
    }

    public void SetPosition(int value)
    {
        _fingerAController.SetPosition(value);
        _fingerBController.SetPosition(value);
    }

    private void FixedUpdate()
    {
        var deltaZ = _inputManager.GripperZ;

        if (deltaZ == 0) return;

        var gripChange = deltaZ * _speed * Time.fixedDeltaTime;
        var currentGrip = (_fingerAController.GetCurrentGrip() + _fingerBController.GetCurrentGrip()) / 2.0f;
        var targetGrip = Mathf.Clamp01(currentGrip + gripChange);
        
        _fingerAController.UpdateGrip(targetGrip);
        _fingerBController.UpdateGrip(targetGrip);
    }
}