using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Player controller, also handles pretty much all the game logic and audio switching
public class PlayerController : MonoBehaviour
{
    public float speed;
    public Text countText;
    public Text winText;
    public AudioSource happyAudio;
    public AudioSource actionAudio;
    public AudioSource jumpAudio;
    public AudioSource pickupAudio;
    public GameObject warningLights;
    public GameObject mainLight;
    public GameObject bossPickup;
    public GameObject mainCamera;

    private Rigidbody _rigidbody;
    private bool _canJump = true;
    private bool _jump = false;
    private int _count = 0;
    private bool _bigScore = false;
    private float _bigScoreTime = 2.0f;
    private float _bigScoreElapsed = 0.0f;
    private bool _canQuit = false;
    private bool _decreaseFont = false;
    private float _fontIncreaseSpeed = 95.0f/2.0f;
    private float _fontIncrease = 14.0f;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        SetCountText();
        winText.text = string.Empty;
    }

    // Handling keyboard input for jump in Update, it felt like it wasn't registering all the time in FixedUpdate
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _canJump)
            _jump = true;
    }

    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (_canJump)
        {
            _rigidbody.AddForce(new Vector3(moveHorizontal, 0.0f, moveVertical) * speed);
        }

        if(_jump)
        {
            _jump = false;
            _rigidbody.AddForce(Vector3.up * 400);
            _canJump = false;
            jumpAudio.Play();
        }

        if(_bigScore && _bigScoreElapsed < _bigScoreTime)
        {
            _bigScoreElapsed += Time.deltaTime;
            _count += (int)(Random.value * 100000.0f + 100000.0f);
            SetCountText();
            _fontIncrease += _fontIncreaseSpeed * Time.deltaTime;
            countText.fontSize = (int)_fontIncrease;

            if (_bigScoreElapsed >= _bigScoreTime)
            {
                _bigScore = false;
                countText.text = "Count: INF";
                _decreaseFont = true;
            }
        }

        if (_decreaseFont)
        {
            _bigScoreElapsed -= Time.deltaTime;
            _fontIncrease -= _fontIncreaseSpeed * Time.deltaTime;
            countText.fontSize = (int)_fontIncrease;
            if (_bigScoreElapsed < 0.0f)
            {
                countText.fontSize = 14;
                _bigScoreElapsed = 0.0f;
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                    winText.text = "You did it!";
                else
                    winText.text = "You did it! Press Q to quit";
                _canQuit = true;
                _decreaseFont = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && _canQuit)
            Application.Quit();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            _canJump = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Pickup"))
        {         
            other.gameObject.SetActive(false);
            _count++;
            SetCountText();
            pickupAudio.Play();
        }

        if(other.gameObject.CompareTag("Boss"))
        {
            other.gameObject.SendMessage("Damage");
        }
    }

    private void SetCountText()
    {
        countText.text = "Count: " + _count.ToString();

        if(_count == 12)
        {
            winText.text = "You Win!";
            StartCoroutine(BossTransition());
        }
    }

    public void BossDefeated()
    {
        _bigScore = true;
        warningLights.SetActive(false);
        mainLight.SendMessage("SetEnabled", false);
    }

    IEnumerator BossTransition()
    {
        yield return new WaitForSeconds(2.0f);
        happyAudio.Stop();
        winText.text = "???";
        warningLights.SetActive(true);
        warningLights.BroadcastMessage("SetEnabled", true);
        mainLight.SendMessage("SetEnabled", true);
        yield return new WaitForSeconds(2.0f);
        mainCamera.SendMessage("ZoomOut");
        bossPickup.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        winText.text = string.Empty;
        actionAudio.Play();
        yield return null;
    }
}
