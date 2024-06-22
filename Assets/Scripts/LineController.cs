using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    [SerializeField] private Rigidbody _prefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private MeshRenderer _beltMeshRenderer;
    [SerializeField] private float _conveyorSpeed, _speed;
    [SerializeField] private Vector3 _direction;

    private List<Rigidbody> _objectOnLine = new();
    private Material _material;

    private bool _inProgress = false;
    
    private void Start()
    {
        _material = _beltMeshRenderer.material;
        _inputManager.OnRespawnCubePressed += OnRespawnCubePressed;
    }
    
    private void Update()
    {
        if (!_inProgress) return;
        
        _material.mainTextureOffset += new Vector2(1, 0) * _conveyorSpeed * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (!_inProgress) return;

        for (int i = 0; i <= _objectOnLine.Count - 1; i++)
        {
            var item = _objectOnLine[i];
            var position = item.position;
            position += _direction * _speed * Time.fixedDeltaTime;
            item.position = position;
            item.MovePosition(position);;
        }
    }

    private void OnRespawnCubePressed()
    {
        _inProgress = !_inProgress;

        if (_inProgress)
        {
            Instantiate(_prefab, _spawnPoint.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Cube"))
        {
            _objectOnLine.Add(collision.gameObject.GetComponent<Rigidbody>());
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Cube"))
        {
            _objectOnLine.Remove(collision.gameObject.GetComponent<Rigidbody>());
            collision.gameObject.GetComponent<Rigidbody>().AddForce(_speed * 30 * _direction);
        }
    }

    private void OnDestroy()
    {
        _inputManager.OnRespawnCubePressed -= OnRespawnCubePressed;
    }
}
