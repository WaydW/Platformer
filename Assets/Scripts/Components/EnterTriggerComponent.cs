﻿using System;
using UnityEngine;
using UnityEngine.Events;

namespace Scripts.Components
{
    public class EnterTriggerComponent : MonoBehaviour
    {
        [SerializeField] private string _tag;
        [SerializeField] private EnterCollisionComponent.EnterEvent _action;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(_tag))
            { 
                _action?.Invoke(other.gameObject);
            }
        }
    }
}