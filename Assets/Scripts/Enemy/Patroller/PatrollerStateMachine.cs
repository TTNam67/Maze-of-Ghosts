using System;
using System.Collections;
using System.Collections.Generic;
using Core.Observer_Pattern;
using Enemy;
using Enemy.Chaser;
using UnityEngine;
using FSM;
using Player;
using UnityEngine.Serialization;

namespace Enemy.Patroller
{
    
    public class PatrollerStateMachine : StateMachine
    {
        [HideInInspector] public PatrollerIdleState _patrollerIdleState;
        [HideInInspector] public PatrollerRunState _patrollerRunState;
        [HideInInspector] public PatrollerAttackState _patrollerAttackState;
        //Die State
        [HideInInspector] public PatrollerAllStates _patrollerAllStates;

        public  AudioSource _audioSource;
        public  Animator _animator;
        public  Collider2D _detectPlayerCollider2D;
        public Collider2D _detectWallCollider2D;
        public  Rigidbody2D _rigidbody2D;
        public Transform _player;
        public  Transform _self;
        public  AudioClip _biteSFX;
        public GameObject _directionArrow;

        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _animator = GetComponent<Animator>();
            _detectPlayerCollider2D = GetComponent<Collider2D>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _player = GameObject.FindWithTag("Player").transform;
            
            _self = this.transform;

            //The initialization of all states has to be before the "Start()" method which contains the "GetInitialState()" method -> using "Awake()" method
            _patrollerIdleState = new PatrollerIdleState("PatrollerIdleState", this);
            _patrollerRunState = new PatrollerRunState("PatrollerRunState", this);
            _patrollerAttackState = new PatrollerAttackState("PatrollerAttackState", this);
            _patrollerAllStates = new PatrollerAllStates("PatrollerAllState", this);
            
            //PStateMachine, _patrollerAllStates observing PatrollerStateMachine from now            
            AddObserver(_player.GetComponent<PStateMachine>());
            AddObserver(_patrollerAllStates);
        }

        protected override BaseState GetInitialState()
        {
            return _patrollerIdleState;
        }
        
        public void ChangeToRun()
        {
            ChangeState(_patrollerRunState);
        }
        
        //Make sure that the current script had implemented MonoBehaviour
        private void OnTriggerStay2D(Collider2D other)
        {
            // Debug.Log("Triggering");
            if (other.CompareTag("Player"))
            {
                NotifyObservers(IEvent.OnPlayerinRange);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            
            Vector2 collidePoint = other.GetContact(0).point;
            Vector2 newDirect = ((Vector2)_self.position - collidePoint).normalized;
            
            _rigidbody2D.AddForce(newDirect * _patrollerAllStates.Speed);
        }
    }

}