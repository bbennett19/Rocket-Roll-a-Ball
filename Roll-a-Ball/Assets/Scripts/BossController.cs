using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Boss controller. Handles all things boss, and audio.
public class BossController : MonoBehaviour
{
    public AudioSource damageAudio;
    public AudioSource bossDeathAudio;
    public AudioSource bossBattleAudio;
    public AudioSource victoryAudio;
    public float invincibilityDuration;
    public float blinkRate;
    public GameObject mainCamera;
    public GameObject player;

    private int _health = 3;
    private bool _invincible = false;
    private float _desitationHeight = 6.0f;
    private float _startingHeight;
    private float _speed = 4.0f;
    private float _startTime;
    private float _journeyLength;
    private bool _inPosition = false;
    private bool _destroyed = false;
    private float _destroyTime = 5.0f;
    private float _elapsedTime = 0.0f;

    private void OnEnable()
    {
        _startTime = Time.time;
        _startingHeight = transform.position.y;
        _journeyLength = _startingHeight - _desitationHeight;
    }

    private void Update()
    {
        if (!_inPosition)
        {
            float distCovered = (Time.time - _startTime) * _speed;
            float fracJourney = distCovered / _journeyLength;
            float y = Mathf.Lerp(_startingHeight, _desitationHeight, fracJourney);
            Debug.Log(y);
            this.transform.position = new Vector3(this.transform.position.x, y, this.transform.position.z);

            if (fracJourney >= 1.0f)
                _inPosition = true;
        }

        if(_destroyed)
        {
            _elapsedTime += Time.deltaTime;

            if(_elapsedTime >= _destroyTime)
            { 
                this.gameObject.SetActive(false);
                victoryAudio.Play();
            }
        }
    }

    public void Damage()
    {
        if (!_invincible)
        {
            _health--;
            damageAudio.Play();

            if (_health > 0)
            {
                _invincible = true;
                StartCoroutine(Blink());
            }

            if (_health == 0)
                Destroy();
        }
    }

    private void Destroy()
    {
        player.SendMessage("BossDefeated");
        mainCamera.SendMessage("StartShake");
        bossBattleAudio.Stop();
        bossDeathAudio.Play();
        _invincible = true;
        _destroyed = true;
        _startTime = Time.time;
    }

    IEnumerator Blink()
    {
        for(int i = 0; i <= (int)invincibilityDuration/blinkRate; i++)
        {
            GetComponent<UnityEngine.Renderer>().enabled = false;
            yield return new WaitForSeconds(blinkRate/2.0f);
            GetComponent<UnityEngine.Renderer>().enabled = true;
            yield return new WaitForSeconds(blinkRate/2.0f);
        }
        _invincible = false;
    }
}
