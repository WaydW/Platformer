﻿using System.Collections;
using Scripts.ColliderBased;
using Scripts.Components.GoBased;
using Scripts.Creatures.Mobs.Patrolling;
using UnityEngine;

namespace Scripts.Creatures.Mobs
{
    public class MobAI : MonoBehaviour
    {
        [SerializeField] private ColliderCheck _vision;
        [SerializeField] private ColliderCheck _canAttack;
        
        [SerializeField] private float _alarmDelay = 0.5f;
        [SerializeField] private float _attackCooldown = 1f;
        [SerializeField] private float _missHeroCooldown = 0.5f;
        
        private bool _isDead;
        
        private Coroutine _current;
        private GameObject _target;
        private SpawnListComponent _particles;
        private Creature _creature;
        private Animator _animator;

        private Patrol _patrol;
        
        private static readonly int IsDeadKey = Animator.StringToHash("is-dead");


        private void Awake()
        {
            _particles = GetComponent<SpawnListComponent>();
            _creature = GetComponent<Creature>();
            _animator = GetComponent<Animator>();
            _patrol = GetComponent<Patrol>();
        }

        private void Start()
        {
            StartState(_patrol.DoPatrol());
        }

        public void OnHeroInVision(GameObject go)
        {
            if (_isDead) return;
            
            _target = go;
            StartState(AgroToHero());
        }

        public void OnDie()
        {
            _isDead = true;
            _animator.SetBool(IsDeadKey, true);
            _creature.SetDirection(Vector2.zero);
            
            if (_current != null)
                StopCoroutine(_current);
        }
        
        private void StartState(IEnumerator coroutine)
        {
            _creature.SetDirection(Vector2.zero);
            if (_current != null)
                StopCoroutine(_current);
            _current = StartCoroutine(coroutine);
        }
        
        private IEnumerator AgroToHero()
        {
            LookAtHero();
            _particles.Spawn("Exclamation");
            yield return new WaitForSeconds(_alarmDelay);
            
            StartState(GoToHero());
        }

        private void LookAtHero()
        {
            var direction = GetDirectionToTarget();
            _creature.SetDirection(Vector2.zero);
            _creature.UpdateSpriteDirection(direction);
        }

        private IEnumerator GoToHero()
        {
            while (_vision.IsTouchingLayer)
            {
                if (_canAttack.IsTouchingLayer)
                {
                    StartState(Attack());
                }
                else
                {
                    SetDirectionToTarget();
                }
                yield return null;
            }
            
            _creature.SetDirection(Vector2.zero);
            _particles.Spawn("MissHero");
            yield return new WaitForSeconds(_missHeroCooldown);
            
            StartState(_patrol.DoPatrol());
        }

        private IEnumerator Attack()
        {
            while (_canAttack.IsTouchingLayer)
            {
                _creature.Attack();
                yield return new WaitForSeconds(_attackCooldown);
            }
            
            StartState(GoToHero());
        }

        private void SetDirectionToTarget()
        {
            var direction = GetDirectionToTarget();
            
            _creature.SetDirection(direction);
        }

        private Vector2 GetDirectionToTarget()
        {
            var direction = _target.transform.position - transform.position;
            direction.y = 0;
            return direction.normalized;
        }
    }
}