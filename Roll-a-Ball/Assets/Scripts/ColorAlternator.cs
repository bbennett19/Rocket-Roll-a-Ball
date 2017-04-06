using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that alternates between to colors of a light
public class ColorAlternator : MonoBehaviour
{
    // Interval between color changes
    public float interval;
    // The color to alternate between
    public Color alternatingColor;

    private Light _light;
    private float _currentTime = 0.0f;
    private Color _startingColor;
    private bool _enabled = false;
    private bool _alt = false;

    // Use this for initialization
    void Awake ()
    {
        _light = GetComponent<Light>();
        _startingColor = _light.color;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (_enabled)
        {
            _currentTime += Time.deltaTime;

            if (_currentTime >= interval)
            {
                if (_alt)
                {
                    _light.color = _startingColor;
                    _alt = false;
                }
                else
                {
                    _light.color = alternatingColor;
                    _alt = true;
                }

                _currentTime = 0.0f;
            }
        }
    }

    public void SetEnabled(bool enabled)
    {
        _enabled = enabled;

        // Make sure to set it back to the origional color when disabling
        if (!_enabled)
            _light.color = _startingColor;
        else
        {
            _light.color = alternatingColor;
            _alt = true;
        }
    }
}
