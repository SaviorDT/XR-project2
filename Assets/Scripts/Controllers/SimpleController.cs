using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
    public enum CommandOption
    {
        Normal,
        Force,
        Queue
    }

    private struct MoveCommand
    {
        public Vector3 Target;
        public float Duration;
    }

    private struct RotateCommand
    {
        public Vector3 TargetEuler;
        public float Duration;
    }

    private Vector3 _target;
    private float _speed;
    private bool _isMoving;
    private readonly Queue<MoveCommand> _moveQueue = new();
    private Quaternion _targetRotation;
    private float _rotationSpeed;
    private bool _isRotating;
    private readonly Queue<RotateCommand> _rotateQueue = new();

    public void MoveTo(Vector3 target, float duration, CommandOption option = CommandOption.Force)
    {
        if (_isMoving)
        {
            if (option == CommandOption.Normal)
            {
                return;
            }

            if (option == CommandOption.Queue)
            {
                _moveQueue.Enqueue(new MoveCommand { Target = target, Duration = duration });
                return;
            }
        }

        StartMove(target, duration);
    }
    public void RotateTo(Vector3 targetEuler, float duration, CommandOption option = CommandOption.Force)
    {
        if (_isRotating)
        {
            if (option == CommandOption.Normal)
            {
                return;
            }

            if (option == CommandOption.Queue)
            {
                _rotateQueue.Enqueue(new RotateCommand { TargetEuler = targetEuler, Duration = duration });
                return;
            }
        }

        StartRotate(targetEuler, duration);
    }
    void Start()
    {
    }
    void Update()
    {
        if (_isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, _target, _speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _target) <= 0.0001f)
            {
                transform.position = _target;
                _isMoving = false;
                TryStartNextMove();
            }
        }

        if (_isRotating)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, _rotationSpeed * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, _targetRotation) <= 0.1f)
            {
                transform.rotation = _targetRotation;
                _isRotating = false;
                TryStartNextRotate();
            }
        }
    }

    private void StartMove(Vector3 target, float duration)
    {
        float safeDuration = Mathf.Max(duration, 0.0001f);
        float distance = Vector3.Distance(transform.position, target);
        _speed = distance / safeDuration;
        _target = target;
        _isMoving = true;
    }

    private void StartRotate(Vector3 targetEuler, float duration)
    {
        float safeDuration = Mathf.Max(duration, 0.0001f);
        _targetRotation = Quaternion.Euler(targetEuler);
        float angle = Quaternion.Angle(transform.rotation, _targetRotation);
        _rotationSpeed = angle / safeDuration;
        _isRotating = true;
    }

    private void TryStartNextMove()
    {
        if (_isMoving || _moveQueue.Count == 0) return;

        MoveCommand next = _moveQueue.Dequeue();
        StartMove(next.Target, next.Duration);
    }

    private void TryStartNextRotate()
    {
        if (_isRotating || _rotateQueue.Count == 0) return;

        RotateCommand next = _rotateQueue.Dequeue();
        StartRotate(next.TargetEuler, next.Duration);
    }
}
