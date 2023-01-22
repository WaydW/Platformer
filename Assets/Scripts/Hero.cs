﻿using UnityEngine;

namespace Scripts
{
    public class Hero : MonoBehaviour
    {
        [SerializeField] private float _speed;
        [SerializeField] private float _jumpForce;
        
        [SerializeField] private float _groundCheckRadius;
        [SerializeField] private Vector3 _groundCheckPositionDelta;
        [SerializeField] private LayerMask _groundLayer;
        // [SerializeField] private LayerCheck _groundCheck; // GroundCheck with circle collider
        
        private Vector2 _direction;
        private Rigidbody2D _rigidbody;
        private Animator _animator;
        private SpriteRenderer _sprite;

        private static readonly int IsGroundKey = Animator.StringToHash("is-ground");
        private static readonly int VerticalVelocityKey = Animator.StringToHash("vertical-velocity");
        private static readonly int IsRunningKey = Animator.StringToHash("is-running");
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _sprite = GetComponent<SpriteRenderer>();
        }
        
        private void FixedUpdate()
        {
            _rigidbody.velocity = new Vector3(_direction.x * _speed, _rigidbody.velocity.y);

            var isJumping = _direction.y > 0;
            var isGrounded = IsGrounded();
            
            if (isJumping)
            {
                if (IsGrounded() && _rigidbody.velocity.y <= 0.1f)
                {
                    _rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
                }
            } 
            else if (_rigidbody.velocity.y > 0 )
            {
                _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y * 0.5f);
            }

            UpdateSpriteDirection();

             
            _animator.SetBool(IsGroundKey, isGrounded);
            _animator.SetFloat(VerticalVelocityKey, _rigidbody.velocity.y);
            _animator.SetBool(IsRunningKey, _direction.x != 0);
        }

        private void UpdateSpriteDirection()
        {
            if (_direction.x > 0)
            {
                _sprite.flipX = false;
            }
            else if (_direction.x < 0)
            {
                _sprite.flipX = true;
            }
        }

        public void SetDirection(Vector2 direction)
        {
            _direction = direction;
        }
        
        public void SaySomething()
        {
            Debug.Log("Say!");
        }

        private bool IsGrounded()
        {
            // return _groundCheck.isTouchingLayer; // GroundCheck with circle collider

             var hit = Physics2D.CircleCast(transform.position + _groundCheckPositionDelta, 
                 _groundCheckRadius,Vector2.down,0,  _groundLayer);

             return hit.collider != null;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = IsGrounded() ? Color.green : Color.red;
            Gizmos.DrawSphere(transform.position + _groundCheckPositionDelta, _groundCheckRadius);
        }
    }

}