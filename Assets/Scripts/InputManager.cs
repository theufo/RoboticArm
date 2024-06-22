using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Action<bool> OnDeltaXChanged;
    public Action<bool> OnDeltaZChanged;
    public Action<bool> OnDeltaYChanged;
    public Action<bool> OnRotateYChanged;
    public Action<bool> OnGripperZChanged;
    
    public Action OnRecordPressed;
    public Action OnPlayPressed;
    public Action OnResetPositionPressed;
    public Action OnRespawnCubePressed;

    public int DeltaX { get; private set; }
    public int DeltaZ { get; private set; }
    public int DeltaY { get; private set; }
    public int RotateY { get; private set; }
    public int GripperZ { get; private set; }

    private bool _isPlaying = false;
    private float _playTime = 0.0f;
    private List<RecordManager.InputData> _inputDatas = null;
    private Queue<RecordManager.InputData> _inputQueue;
    private Dictionary<InputType, RecordManager.InputData> _processingData;
    private List<InputType> _itemsToRemove = new(4);
    private Action _callback;

    public void Play(List<RecordManager.InputData> inputDatas, Action callback)
    {
        Debug.Log($"START playing");

        _isPlaying = true;
        
        _inputDatas = inputDatas;
        _callback = callback;
        
        _inputQueue = new Queue<RecordManager.InputData>();
        _inputDatas.ForEach(x => _inputQueue.Enqueue(x));
        _processingData = new Dictionary<InputType, RecordManager.InputData>()
        {
            {InputType.XAxis, null},
            {InputType.ZAxis, null},
            {InputType.YAxis, null},
            {InputType.YRotate, null},
            {InputType.ZGripper, null},
        };

        _playTime = 0.0f;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            OnPlayPressed?.Invoke();
        }
        
        UpdateProcessingData();

        if (_isPlaying) return;

        if (Input.GetKeyUp(KeyCode.R))
        {
            OnRecordPressed?.Invoke();
        }
        
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            OnResetPositionPressed?.Invoke();
        }
        
        if (Input.GetKeyUp(KeyCode.C))
        {
            OnRespawnCubePressed?.Invoke();
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            SetDeltaX(-1);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            SetDeltaX(1);
        }
        else
        {
            SetDeltaX(0);
        }
        
        if (Input.GetKey(KeyCode.UpArrow))
        {
            SetDeltaZ(-1);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            SetDeltaZ(1);
        }
        else
        {
            SetDeltaZ(0);
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            SetDeltaY(-1);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            SetDeltaY(1);
        }
        else
        {
            SetDeltaY(0);
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            SetRotateY(-1);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            SetRotateY(1);
        }
        else
        {
            SetRotateY(0);
        }
        
        if (Input.GetKey(KeyCode.Z))
        {
            SetGripperZ(-1);
        }
        else if (Input.GetKey(KeyCode.X))
        {
            SetGripperZ(1);
        }
        else
        {
            SetGripperZ(0);
        }
    }

    private void SetDeltaX(int value)
    {
        if (value == DeltaX) return;
        
        DeltaX = value;
        OnDeltaXChanged?.Invoke(value != 0);
    }
    
    private void SetDeltaZ(int value)
    {
        if (value == DeltaZ) return;

        DeltaZ = value;
        OnDeltaZChanged?.Invoke(value != 0);
    }
    
    private void SetDeltaY(int value)
    {
        if (value == DeltaY) return;

        DeltaY = value;
        OnDeltaYChanged?.Invoke(value != 0);
    }
    
    private void SetRotateY(int value)
    {
        if (value == RotateY) return;

        RotateY = value;
        OnRotateYChanged?.Invoke(value != 0);
    }
    
    private void SetGripperZ(int value)
    {
        if (value == GripperZ) return;

        GripperZ = value;
        OnGripperZChanged?.Invoke(value != 0);
    }
    
    private void UpdateProcessingData()
    {
        if (!_isPlaying) return;

        _playTime += Time.deltaTime;
        
        foreach (var pair in _processingData)
        {
            if (pair.Value != null && pair.Value.EndTime <= _playTime)
            {
                Debug.Log($"Data ended. {pair.Key} {pair.Value.InputType} {pair.Value.EndTime}");
                SetAxisValue(pair.Key, 0);
                _itemsToRemove.Add(pair.Key);
            }
        }

        _itemsToRemove.ForEach(x => _processingData[x] = null);
        _itemsToRemove.Clear();

        if (_inputQueue.TryPeek(out var item))
        {
            if (item.StartTime <= _playTime)
            {
                Debug.Log($"Data started. {item.InputType} {item.StartTime}");

                _inputQueue.Dequeue();
                _processingData[item.InputType] = item;
                SetAxisValue(item.InputType, item.Value);
            }
        }

        if (_inputQueue.Count <= 0 && _processingData.All(x => x.Value == null))
        {
            Stop();
        }
    }

    private void SetAxisValue(InputType inputType, int value)
    {
        switch (inputType)
        {
            case InputType.XAxis:
                SetDeltaX(value);
                break;
            case InputType.ZAxis:
                SetDeltaZ(value);
                break;
            case InputType.YAxis:
                SetDeltaY(value);
                break;
            case InputType.YRotate:
                SetRotateY(value);
                break;
            case InputType.ZGripper:
                SetGripperZ(value);
                break;
            case InputType.ResetPosition:
                if (value > 0) OnResetPositionPressed?.Invoke();
                break;
            case InputType.RespawnCube:
                if (value > 0) OnRespawnCubePressed?.Invoke();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(inputType), inputType, null);
        }
    }

    public void Stop()
    {
        _isPlaying = false;
        
        _inputDatas = null;
        _inputQueue = null;
        _processingData = null;
        
        Debug.Log($"STOP playing");
        
        _callback?.Invoke();
    }
}
