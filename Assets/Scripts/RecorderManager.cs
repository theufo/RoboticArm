using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using UnityEngine;

public class RecordManager : MonoBehaviour
{
    [Serializable]
    public class SaveDataModel
    {
        public List<InputData> List;
    }
    
    [Serializable]
    public class InputData
    {
        public InputType InputType;
        public int Value;
        public float StartTime = -1;
        public float EndTime = -1;
    }

    [SerializeField] private InputManager _inputManager;
    [SerializeField] private RobotController _robotController;

    public Action<bool> OnStartRecording;
    public Action<bool> OnStartPlaying;
    
    private Dictionary<InputType, InputData> _inputDataInProgress = new Dictionary<InputType, InputData>();
    private List<InputData> _inputDatas = new List<InputData>();

    private bool _isRecording = false;
    private bool _isPlaying = false;

    private string _recordKey = "RecordKey";

    private void Start()
    {
        _inputManager.OnRecordPressed += OnRecordPressed;
        _inputManager.OnPlayPressed += OnPlayPressed;
        _inputManager.OnResetPositionPressed += OnResetPositionPressed;
        _inputManager.OnRespawnCubePressed += OnRespawnCubePressed;
        
        _inputDataInProgress.Add(InputType.XAxis, null);
        _inputDataInProgress.Add(InputType.ZAxis, null);
        _inputDataInProgress.Add(InputType.YAxis, null);
        _inputDataInProgress.Add(InputType.YRotate, null);
        _inputDataInProgress.Add(InputType.ZGripper, null);

        LoadData();
    }

    private void LoadData()
    {
        var saveData = PlayerPrefs.GetString(_recordKey, string.Empty);

        if (!string.IsNullOrEmpty(saveData))
        {
            var model = JsonUtility.FromJson<SaveDataModel>(saveData);
            _inputDatas = model.List;
        }
    }

    private void SaveData()
    {
        var model = new SaveDataModel();
        model.List = _inputDatas;
        
        var str = JsonUtility.ToJson(model);
        PlayerPrefs.SetString(_recordKey, str);
    }

    private void OnPlayPressed()
    {
        if (_isPlaying)
        {
            StopPlay();
        }
        else
        {
            StartPlay();
        }
    }

    private void StartPlay()
    {
        OnStartPlaying?.Invoke(true);

        if (_isRecording)
        {
            Unsubscribe();
        }

        if (!_inputDatas.Any()) return;

        _isPlaying = true;
        _inputManager.Play(_inputDatas, () =>
        {
            _isPlaying = false;
            OnStartPlaying?.Invoke(false);
        });
    }

    private void StopPlay()
    {
        _isPlaying = false;
        _inputManager.Stop();
        
        OnStartPlaying?.Invoke(false);
    }

    private void OnRecordPressed()
    {
        if (_isRecording)
        {
            Unsubscribe();
        }
        else
        {
            Subscribe();
        }
    }

    private void Subscribe()
    {
        Debug.Log($"START recording");
        OnStartRecording?.Invoke(true);
        
        _inputDatas.Clear();
        
        _isRecording = true;
        
        _inputManager.OnDeltaXChanged += OnDeltaXChanged;
        OnDeltaXChanged(_inputManager.DeltaX != 0);
        _inputManager.OnDeltaZChanged += OnDeltaZChanged;
        OnDeltaZChanged(_inputManager.DeltaZ != 0);
        _inputManager.OnDeltaYChanged += OnDeltaYChanged;
        OnDeltaYChanged(_inputManager.DeltaZ != 0);
        _inputManager.OnRotateYChanged += OnRotateYChanged;
        OnRotateYChanged(_inputManager.RotateY != 0);
        _inputManager.OnGripperZChanged += OnGripperZChanged;
        OnGripperZChanged(_inputManager.GripperZ != 0);
        _inputManager.OnResetPositionPressed += OnResetPositionPressed;
    }

    private void Unsubscribe()
    {
        _isRecording = false;
        _inputManager.OnDeltaXChanged -= OnDeltaXChanged;
        _inputManager.OnDeltaZChanged -= OnDeltaZChanged;
        _inputManager.OnDeltaYChanged -= OnDeltaYChanged;
        _inputManager.OnRotateYChanged -= OnRotateYChanged;
        _inputManager.OnGripperZChanged -= OnGripperZChanged;
        _inputManager.OnResetPositionPressed -= OnResetPositionPressed;

        var keyNames = new List<InputType>();
        foreach (var pair in _inputDataInProgress)
        {
            if (pair.Value != null)
            {
                pair.Value.EndTime = Time.time;
                keyNames.Add(pair.Key);
            }
        }
        
        keyNames.ForEach(x => _inputDataInProgress[x] = null);

        var timeDelta = _inputDatas[0].StartTime;
        
        _inputDatas.ForEach(x =>
        {
            x.StartTime -= timeDelta;
            x.EndTime -= timeDelta;
        });

        SaveData();
        OnStartRecording?.Invoke(false);
        Debug.Log($"END recording");
    }

    private void OnDeltaXChanged(bool hasValue)
    {
        ProcessValues(_inputManager.DeltaX, hasValue, InputType.XAxis);
    }
    
    private void OnDeltaZChanged(bool hasValue)
    {
        ProcessValues(_inputManager.DeltaZ, hasValue, InputType.ZAxis);
    }
    
    private void OnDeltaYChanged(bool hasValue)
    {
        ProcessValues(_inputManager.DeltaY, hasValue, InputType.YAxis);
    }
    
    private void OnRotateYChanged(bool hasValue)
    {
        ProcessValues(_inputManager.RotateY, hasValue, InputType.YRotate);
    }
    
    private void OnGripperZChanged(bool hasValue)
    {
        ProcessValues(_inputManager.GripperZ, hasValue, InputType.ZGripper);
    }
    
    private void OnResetPositionPressed()
    {
        var data = new InputData() {InputType = InputType.ResetPosition, Value = 1, StartTime = Time.time, EndTime = Time.time};
        _inputDatas.Add(data);
    }

    private void OnRespawnCubePressed()
    {
        var data = new InputData() {InputType = InputType.RespawnCube, Value = 1, StartTime = Time.time, EndTime = Time.time};
        _inputDatas.Add(data);
    }

    private void ProcessValues(int value, bool hasValue, InputType inputName)
    {
        if (hasValue)
        {
            if (_inputDataInProgress[inputName] != null)
            {
                _inputDataInProgress[inputName].EndTime = Time.time;
                _inputDataInProgress[inputName] = null;
                
                Debug.Log($"Stop recording delta X {Time.realtimeSinceStartup}");
            }

            var data = new InputData() {InputType = inputName, Value = value, StartTime = Time.time};
            _inputDatas.Add(data);
            _inputDataInProgress[inputName] = data;

            Debug.Log($"Start recording delta X {Time.realtimeSinceStartup} {value}");
        }
        else
        {
            if (_inputDataInProgress[inputName] == null) return;

            _inputDataInProgress[inputName].EndTime = Time.time;
            _inputDataInProgress[inputName] = null;

            Debug.Log($"Stop recording delta X {Time.realtimeSinceStartup}");
        }
    }

    private void OnDestroy()
    {
        _inputManager.OnRecordPressed -= OnRecordPressed;
        _inputManager.OnPlayPressed -= OnPlayPressed;
        _inputManager.OnResetPositionPressed -= OnResetPositionPressed;
        _inputManager.OnRespawnCubePressed -= OnRespawnCubePressed;
    }
}