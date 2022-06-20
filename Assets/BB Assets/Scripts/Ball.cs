using System.Collections;
using TMPro;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody2D _rb2D; // Rigidbody2D component of the ball
    private int _score; // score of the player
    public int lives = 5; // lives remaining (balls)
    public GameObject[] ballsLives; // array of gameObjects balls indicating the lives
    private readonly Vector2 _livesCoord = new Vector2(-1.2f, 4f);
    private const float LivesSize = .3f;
    private const float BaseSpeed = -7f;
    private const int DeviationRate = 5; // deviation rate of when the ball hits the side of the paddle
    private const float FallThreshold = -5.5f; // minimum y coordinate of the ball
    [SerializeField] protected GameObject refPaddle; // refers to the paddle
    [SerializeField] protected GameObject prefabBall; // refers to the prefab of the ball used to indicates the lives
    [SerializeField] protected TextMeshPro refScoreText; // refers to the text of the score
    [SerializeField] protected TextMeshPro refGameOverText; // refers to the text of the score
    [SerializeField] protected AudioClip[] clips; // {wallHit, paddleHit, scoreWin, scoreLoose, gameOver, brickHit, fall}
    private AudioSource _sfxPlayer; // audio player

    private void Start()
    {
        _rb2D = gameObject.GetComponent<Rigidbody2D>(); // gets the components
        _sfxPlayer = gameObject.GetComponent<AudioSource>();
        ballsLives = new GameObject[5];
        for (var i = 0; i < 5; i++)
        {
            ballsLives[i] = Instantiate(prefabBall);
            ballsLives[i].transform.position = (new Vector3(_livesCoord.x + LivesSize * 2 * i, _livesCoord.y, 0f));
        }
        StartCoroutine(Respawn(false)); // spawns the ball
    }

    private void Update()
    {
        if (this.transform.position.y < FallThreshold && lives > 0) // if the ball goes below the threshold, respawns it on the screen
        {
            StartCoroutine(Respawn(true)); // respawns the ball
        }
        if (Mathf.Abs(_rb2D.velocity.y) <= .2f && Mathf.Abs(_rb2D.velocity.y) > 0) // if the ball is stuck in an horizontal loop, its y speed is low (here we take ]0;.2]), adds a vertical force
        {
            var vel = _rb2D.velocity; // gets the current velocity of the ball
            _rb2D.velocity = new Vector2(vel.x, vel.y + Mathf.Sign(vel.y) * 10); // changes the velocity by adding a vertical force
        }
    }   
    /**
     * Resets the ball to the center of the screen and waits 2 seconds before launching it
     */
    public IEnumerator Respawn(bool fellOff)
    {
        if (fellOff)
        {
            _sfxPlayer.clip = clips[6]; // picks the audio clip of the fall
            _sfxPlayer.Play(); // plays the clip
            UpdateScore(-250); // removes 250 from the score
            lives--;
            Destroy(ballsLives[lives]);
        }
        if (lives > 0)
        {
            transform.position = new Vector3(0, -1, 0); // sets the ball in the center of the screen
            _rb2D.velocity = new Vector2(0, 0); // sets its velocity to 0
            yield return new WaitForSeconds(2); // waits for 2 seconds
            _rb2D.velocity = new Vector2(0, BaseSpeed); // launches it 
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        _sfxPlayer.clip = clips[4]; // plays the wallHit sound
        _sfxPlayer.Play();
        refGameOverText.text = "GAME OVER\n<size=3>Press [_] to play again or [esc] to quit";
        refGameOverText.transform.position = new Vector3(0f, 0f, 0f);
        Time.timeScale = 0;
    }
    /**
     * Called when there is a collision
     * @param col : component hit by the ball
     */
    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.name == "Paddle") // if there is a collision with the paddle
        {
            _sfxPlayer.clip = clips[1]; // picks the clip paddleHit
            _sfxPlayer.Play(); // plays it
            var diffX = this.transform.position.x - refPaddle.transform.position.x; // calculates the difference of x coordinates
            this._rb2D.velocity += new Vector2(diffX * DeviationRate, 0); // adds an horizontal force to deviate the ball according to the side of the paddle hit
        }
        else if (col.gameObject.CompareTag("brick")) // if a brick has been hit
        {
            _sfxPlayer.clip = clips[5]; // picks the clip brickHit
            _sfxPlayer.Play(); // plays it
            if (!col.gameObject.GetComponent<Brick>().OnHit()) return; // If the brick isn't destroyed does nothing
            _sfxPlayer.clip = clips[2]; // otherwise plays the sound of scoreWin and adds 100 pts to the score
            _sfxPlayer.Play();
            UpdateScore(100);
        } 
        else if (col.gameObject.CompareTag("wall")) // if a wall has been hit
        {
            _sfxPlayer.clip = clips[0]; // plays the wallHit sound
            _sfxPlayer.Play();
        }
    }
    /**
     * Adds a value to the score and updates the display
     * @param n : value to add to the score
     */
    private void UpdateScore(int n)
    {
        _score += n; // adds n to the score
        refScoreText.text = "Score : " + _score; // updates the text on the screen
    }
}
