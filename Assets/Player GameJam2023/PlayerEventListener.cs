using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Scripts.Player.Platformer;
using UnityEngine;

namespace Player_GameJam2023
{
    public class PlayerEventListener : MonoBehaviour, IPlayerEventsListener
    {
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private Transform _transform;

        [SerializeField]
        private float _flipSpeed = 360f;
        [SerializeField]
        private float _durationUntilBehavingAsNonGround = 0.1f;

        private float _currentYRotation = 90f;
        private float _currentYRotationOffset = 0;
        private float _remainingNoneGroundTime = 0f;
        private CancellationToken _cancellationToken;

        private bool _isAlreadyJumping = false;
        private bool _isAlreadyWaitingForFall = false;
        private static readonly int Walk = Animator.StringToHash("Walk");
        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int DoubleJump = Animator.StringToHash("DoubleJump");
        private static readonly int XVelocity = Animator.StringToHash("VelocityX");

        private void Start()
        {
            _cancellationToken = this.GetCancellationTokenOnDestroy();
        }

        private void Update()
        {
            _transform.localRotation = Quaternion.Euler(Vector3.up * (_currentYRotation + _currentYRotationOffset));
            _remainingNoneGroundTime -= Time.deltaTime;
        }

        public void OnJump()
        {
            _animator.SetBool(Jump, true);
            if (_isAlreadyJumping)
            {
                _animator.SetBool(DoubleJump, true);
                StopCoroutine(AirFlip());
                StartCoroutine(AirFlip()); 
            }
            _isAlreadyJumping = true;
        }

        public void OnFall()
        {
            if (_isAlreadyJumping || _isAlreadyWaitingForFall)
            {
                return;
            }

            _isAlreadyJumping = true;
            _isAlreadyWaitingForFall = true;
            FallWithDelay().Forget();
        }

        private async UniTaskVoid FallWithDelay()
        {
            await UniTask.WaitUntil(() => _remainingNoneGroundTime <= 0, cancellationToken: _cancellationToken);
            _isAlreadyWaitingForFall = false;
            _animator.SetBool(Jump, true);
        }

        private IEnumerator AirFlip()
        {
            while (_isAlreadyJumping)
            {
                _currentYRotationOffset += _flipSpeed * Time.deltaTime;
                yield return null;
            }

            while (Math.Abs(_transform.rotation.x) > 1)
            {
                _currentYRotationOffset += _flipSpeed * Time.deltaTime;
                yield return null;
            }

            _currentYRotationOffset = 0;
        }

        public void OnGround()
        {
            _remainingNoneGroundTime = _durationUntilBehavingAsNonGround;
            _isAlreadyJumping = false;
            _animator.SetBool(Jump, false);
            _animator.SetBool(DoubleJump, false);
        }

        public void OnVelocity(Vector3 currentVelocity)
        {
            if (currentVelocity.x > 0.01f)
            {
                _currentYRotation = 90f;
            }
            else if (currentVelocity.x < -0.01f)
            {
                _currentYRotation = -90f;
            }
            
            _animator.SetFloat(XVelocity, currentVelocity.x);
            if (Math.Abs(currentVelocity.x) > 0.01f)
            {
                _animator.SetBool(Walk, true);
            }
            else
            {
                _animator.SetBool(Walk, false);
            }
        }
    }
}