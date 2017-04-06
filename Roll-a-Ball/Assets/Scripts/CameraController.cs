using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Camera controller, allows for smooth zooming and camera shake
public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float zoomSpeed;
    public float zoomFactor;

    private Vector3 _offset;
    private float _startTime = 0.0f;
    private Vector3 _startMarker;
    private Vector3 _endMarker;
    private float _journeyLength;
    private bool _zooming = false;
    private bool _shaking = false;
    private float _shakingTime;
    private float _shakingDuration = 5.0f;

	// Use this for initialization
	void Start ()
    {
        _offset = transform.position - player.transform.position;	
	}
	
	// Update is called once per frame
	void LateUpdate ()
    {
        float shakeX = 0.0f;
        float shakeY = 0.0f;
        float shakeZ = 0.0f;

        if (_zooming)
        {
            float distCovered = (Time.time - _startTime) * zoomSpeed;
            float fracJourney = distCovered / _journeyLength;
            _offset = Vector3.Lerp(_startMarker, _endMarker, fracJourney);

            if (fracJourney >= 1.0f)
                _zooming = false;
        }

        if(_shaking)
        {
            if (_shakingTime < _shakingDuration)
            {
                _shakingTime += Time.deltaTime;
                shakeX = Random.value * 2.0f - 1.0f;
                shakeY = Random.value * 2.0f - 1.0f;
                shakeZ = Random.value * 2.0f - 1.0f;
            }
            else
                _shaking = false;
        }

        transform.position = player.transform.position + _offset + new Vector3(shakeX, shakeY, shakeZ);	
	}

    public void StartShake()
    {
        _shaking = true;
        _shakingTime = 0.0f;
    }

    public void ZoomOut()
    {
        _zooming = true;
        _startTime = Time.time;
        _startMarker = _offset;
        _endMarker = _offset * zoomFactor;
        _journeyLength = Vector3.Distance(_startMarker, _endMarker);
    }
}
