using System.Collections;
using UnityEngine;

public class SimpleController : MonoBehaviour
{
    private Vector3 _target;
    private float _speed;
    private bool _isMoving;

    public void MoveTo(Vector3 target, float duration)
    {
        float safeDuration = Mathf.Max(duration, 0.0001f);
        float distance = Vector3.Distance(transform.position, target);
        _speed = distance / safeDuration;
        _target = target;
        _isMoving = true;
    }
    void Start()
    {
    }
    void Update()
    {
        if (!_isMoving) return;

        transform.position = Vector3.MoveTowards(transform.position, _target, _speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, _target) <= 0.0001f)
        {
            transform.position = _target;
            _isMoving = false;
        }
    }
}
