using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameNoteCtrl : MonoBehaviour
{
    private Transform _shooterTr;
    private Transform _checkerTr;

    private float _timeToGetChecker;

    private float _noteSpeed;
    private Vector3 _noteDirection;

    void Awake()
    {
        InitNoteInfo();
    }

    void InitNoteInfo()
    {
        _timeToGetChecker = 400f / PlayerPrefs.GetFloat("bpm");

        _shooterTr = GameObject.FindGameObjectWithTag("SHOOTER").GetComponent<Transform>();
        _checkerTr = GameObject.FindGameObjectWithTag("CHECKER").GetComponent<Transform>();
        
        _noteSpeed = Vector2.Distance(_shooterTr.position, _checkerTr.position) / _timeToGetChecker;
        _noteDirection = (_checkerTr.position - _shooterTr.position).normalized;
    }

    void Update()
    {
        transform.position += _noteDirection * _noteSpeed * Time.deltaTime;
    }
}