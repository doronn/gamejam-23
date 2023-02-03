using System;
using GameJamKit.Scripts.Utils.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomIdle : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    private static readonly int Idle = Animator.StringToHash("RandomIdle");

    private float _nextIdleValue = 0;
    private float _currentIdleValue = 0;
    private void Start()
    {
        // Making it so this component could be enabled and disabled from the editor
    }

    [Button]
    void NextRandomIdle()
    {
        _nextIdleValue = Random.Range(0, 3);
    }

    private void Update()
    {
        _currentIdleValue = Mathf.Lerp(_currentIdleValue, _nextIdleValue, 0.1f);
        _animator.SetFloat(Idle, _currentIdleValue);
    }
}
