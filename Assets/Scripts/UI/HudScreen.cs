using UnityEngine;

public class HudScreen : MonoBehaviour
{
    [SerializeField] private GameObject _playContriner;
    [SerializeField] private GameObject _recordContainer;
    [SerializeField] private RecordManager _recordManager;

    private void Start()
    {
        _recordManager.OnStartRecording += OnStartRecording;
        _recordManager.OnStartPlaying += OnStartPlaying;
    }

    private void OnStartPlaying(bool isPlaying)
    {
        _playContriner.SetActive(isPlaying);
    }

    private void OnStartRecording(bool isRecording)
    {
        _recordContainer.SetActive(isRecording);
    }

    private void OnDestroy()
    {
        _recordManager.OnStartRecording -= OnStartRecording;
        _recordManager.OnStartPlaying -= OnStartPlaying;
    }
}
