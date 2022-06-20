using System.Collections;
using TMPro;
using UnityEngine;

public class BirdScript : MonoBehaviour
{
    [SerializeField] protected GameObject gameMaster;
    [SerializeField] protected TextMeshPro gameText;
    [SerializeField] protected TextMeshPro scoreText;
    [SerializeField] protected TextMeshPro pauseText;
    [SerializeField] protected SpriteRenderer birdSpriteRenderer;
    [SerializeField] protected Sprite jumpSprite;
    [SerializeField] protected Sprite standardSprite;
    [SerializeField] protected Sprite deathSprite;
    private GameMasterFBScript _gameMasterFbScript;
    private Rigidbody2D _rb2D;
    private const float JumpSpeed = 4f;
    public bool hasStarted = false;
    private bool _isAlive = true;
    private int _score = 0;
    private bool _isPaused = false;
    
    // Gets some components by code at the beginning
    private void Start()
    {
        _gameMasterFbScript = gameMaster.GetComponent<GameMasterFBScript>();
        _rb2D = this.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && _isAlive && hasStarted)  // Toggles the pause if the bird is alive
        {
            TogglePause();
        }
        if (this.transform.position.y < -6) OnDeath();  // if the bird falls below the threshold, dies
        if (Input.GetKeyDown(KeyCode.UpArrow) && _isAlive && !_isPaused) // if the player hits up arrow
        {
            StopAllCoroutines();
            StartCoroutine(ToJumpSprite()); // changes the sprite 
            if (hasStarted)
            {
                _rb2D.velocity = new Vector2(0f, JumpSpeed);
            }
            else
            {
                _rb2D.velocity = new Vector2(0f, JumpSpeed);
                hasStarted = true;
                _gameMasterFbScript.PipeCreator();
                gameText.transform.position = new Vector3(0f, 0f, 15f);
                ChangeGravity(1f);
            }
        }

        if (!_isAlive && Input.GetKeyDown(KeyCode.Space))
        {
            OnRestart();
        } 
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1;
            StartCoroutine(TitleScript.LoadScene("Menu"));
        }
    }

    private void ToStandardSprite()
    {
        birdSpriteRenderer.sprite = standardSprite;
    }

    private IEnumerator ToJumpSprite()
    {
        birdSpriteRenderer.sprite = jumpSprite;
        yield return new WaitForSeconds(.2f);
        ToStandardSprite();
    }

    private void ToDeathSprite()
    {
        this.StopAllCoroutines();
        birdSpriteRenderer.sprite = deathSprite;
    }

    private void TogglePause()
    {
        if (!_isPaused)
        {
            pauseText.transform.position = new Vector3(0, 0, -3);
            Time.timeScale = 0;
        }
        else
        {
            pauseText.transform.position = new Vector3(0, 0, 20);
            Time.timeScale = 1;
        }
        _isPaused = !_isPaused;
    }

    private void OnRestart()
    {
        ToStandardSprite();
        Time.timeScale = 1;
        SetScore(0);
        this._isAlive = true;
        _gameMasterFbScript.isAlive = true;
        _rb2D.velocity = Vector2.zero;
        gameText.transform.position = new Vector3(0f, -3f, 0f);
        gameText.text = "PRESS [↑] TO START";
        this.transform.position = new Vector3(-5f, 0f, 0f);
        hasStarted = false;
        _gameMasterFbScript.DestroyPipes();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("pipe"))
        {
            OnDeath();
        }
    }

    private void OnDeath()
    {
        StopAllCoroutines();
        ToDeathSprite();
        gameMaster.GetComponent<GameMasterFBScript>().StopAllCoroutines();
        Time.timeScale = 0;
        this._isAlive = false;
        _gameMasterFbScript.isAlive = false;
        ChangeGravity(0f);
        _rb2D.velocity = Vector2.zero;
        gameText.transform.position = new Vector3(0f, 0f, -5f);
        gameText.text = "GAME OVER\n<i><size=7>press [_] to play again or [esc] to quit</i>";
    }

    public void AddToScore(int n)
    {
        SetScore(_score + n);
        scoreText.text = _score.ToString();
    }
    private void SetScore(int n)
    {
        _score = n;
        scoreText.text = _score.ToString();
    }

    private void ChangeGravity(float gravity)
    {
        _rb2D.gravityScale = gravity;
    }
}
