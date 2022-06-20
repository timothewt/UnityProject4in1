using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class BoxScript : MonoBehaviour {
    private const float ScreenXLimit = 7.5f; // horizontal screen limit for the box to move
    private int score; // score of the game (apple caught)
    public int lives = 3; // number of lives (removes 1 for each apple not caught)
    private const float Speed = 18f; // speed of the box
    private float timerDuration = 120f; // timer before the game is over 
    private const float BoxRotation = 15f; // rotation of the box during the movement
    [SerializeField] protected TextMeshPro textScore; // refers to the 'score : xx' text field
    [SerializeField] protected TextMeshPro textTimer; // refers to the text of the time remaining
    [SerializeField] protected TextMeshPro textGameOver; // refers to the 'Game Over' text
    [SerializeField] protected TextMeshPro textGameOverSub; // refers to the 'Press Space [..] Quit' text
    [SerializeField] protected GameObject[] hearts; // refers to the hearts indicating the lives remaining
    [SerializeField] protected GameObject prefExplosion; // refers to the explosion image prefab
    [SerializeField] protected AudioClip bombSound; // refers to the bomb sound
    [SerializeField] protected AudioClip collectSound; // refers to the collect sound
    private AudioSource sfxPlayer; // sound player
    [SerializeField] protected Sprite jesusSprite; // refers to the image of jesus
    [SerializeField] protected SpriteRenderer spriteRenderer; // refers to the sprite of the box
    private Animator _animator;

    private void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
        sfxPlayer = gameObject.AddComponent<AudioSource>(); // creates the audio source
        if (TitleScript.GameMode == 0) return; // if the game mode is lives, does nothing
        textTimer.transform.position = new Vector3(4.2f, 4.5f, -.1f); // else, makes 'time remaining' visible
        for (var i = 0; i < 3; i++) // and destroys the 3 hearts
        {
            Destroy(hearts[i]);
        }
        
    }
    private void Update()
    {
        if (TitleScript.GameMode == 0) // if the game mode is with 3 lives
        {
            switch (lives)
            {
                case 1: case 2: case 3: // if the player still has lives checks if the player moves the box
                    CheckMovement();
                    break;
                case 0: // if no lives left, game over method
                    OnGameOver();
                    break;
                case -1: // the game over method has already been called, checks if the player restarts the game or leaves
                    CheckRestartGame();
                    break;
            }
        } else  // if the game mode is time (gm=1)
        {
            if (lives == -1) // the game over method has already been called, checks if the player restarts the game or leaves
            {
                CheckRestartGame();
            }
            else
            {
                UpdateTimer(); // Updates the timer
                CheckMovement(); // checks for box movements from the player
                if (timerDuration <= 0) // if there is no time left calls the game over method
                {
                    OnGameOver();
                }
            }
        }

        if (score != 25) return; // if score isn't 25 does nothing
        spriteRenderer.sprite = jesusSprite; // if the player reaches 25 of score, jesus will collect the apples
    }

    private void OnCollisionEnter2D(Collision2D collision) // executes when there's a collision 
    {
        if (collision.gameObject.CompareTag("pomme")) // if the collision is with an apple
        {
            sfxPlayer.clip = collectSound; // changes the sound to the collect
            sfxPlayer.Play(); // plays
            score++; // adds 1 to the score
            textScore.text = "Score : " + score; // updates the score display
        } else if (collision.gameObject.CompareTag("bomb")) // if the collision is with a bomb
        {
            sfxPlayer.clip = bombSound; // changes the sound to the bomb
            sfxPlayer.Play(); // plays
            Explode(); // displays an explosion image on the box to indicate it exploded
            if (TitleScript.GameMode == 0) // if the game mode is lives
            {
                RemoveLife(); // removes one life
            }
            else // if the game mode is with time
            {
                score -= 5; // removes 5 from the score
                textScore.text = "Score : " + score; // updates the score display
            }
        }
    }

    private void UpdateTimer() // updates the game timer
    {
        timerDuration -= Time.deltaTime; // removes the time elapsed since the last time it was called
        var seconds = (int) timerDuration % 60; // gets the n° of seconds left in the timer
        var minutes = (int)((timerDuration - seconds)/60); // gets the n° of minutes left in the timer
        var time = seconds < 10 ? "0" + minutes + ":0" + seconds : "0" + minutes + ":" + seconds; // time to display (adds 0 before the seconds if there are less than 10 seconds left)
        textTimer.text = "Time remaining : " + time; // updates the display
    }
    private void CheckRestartGame() // checks for player interactions on the game over screen
    {
        if (Input.GetKey(KeyCode.Escape)) // if user presses escape, leaves the app
        {
            StartCoroutine(TitleScript.LoadScene("Menu"));
        }
        else if (Input.GetKey(KeyCode.Space)) // if the user presses space, goes back to the menu to start a new game
        {
            StartCoroutine(TitleScript.LoadScene("ACMenu"));
        }
    }

    private void OnGameOver() // when the game is over
    {
        textGameOver.transform.position = new Vector3(.3f, 0f, 0f); // displays 'Game Over'
        textGameOverSub.transform.position = new Vector3(0f, -3.5f, 0f); //displays 'Press Space [..] Quit'
        transform.position = new Vector3(0f, 0f, 2f); // puts the box behind the background so the player can't see now
        lives = -1; // lives goes to -1 to detect player interaction in the Update()
    }

    private void CheckMovement() // checks if the user wants to move the box
    {
        if (Input.GetKey(KeyCode.RightArrow) && transform.position.x < 7.5f) // if right arrow's pressed and the box isn't out of the screen
        {
            _animator.SetTrigger("T_Walk");
            transform.eulerAngles = new Vector3(0f, 0f, -BoxRotation * Time.timeScale); // rotates the box to the right
            transform.Translate(Speed * Time.deltaTime, 0, 0, Space.World); // translates the box to the right
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && transform.position.x > -7.5f) // if left arrow's pressed and the box isn't out of the screen
        {
            _animator.SetTrigger("T_Rear");
            transform.eulerAngles = new Vector3(0f, 0f, BoxRotation * Time.timeScale); // rotates the box to the left
            transform.Translate(-Speed * Time.deltaTime, 0, 0, Space.World); // translates the box to the left
        }
        else
        {
            _animator.SetTrigger("T_Idle");
            transform.eulerAngles = new Vector3(0f, 0f, 0f); // if there are no movements, sets back the rotation of the box to 0
        }
    }

    private void Explode() // displays an explosion image on the screen
    {
        var explosion = Instantiate(prefExplosion); // instantiate a new explosion game object
        var pos = transform.position; // retrieves the position of the box
        explosion.transform.position = new Vector3(pos.x, pos.y,-1f); // sets the explosion above the box
        Destroy(explosion, .5f); // destroys the explosion object after 0.5seconds.

    }

    public void RemoveLife() // removes life from the player
    {
        switch (lives)
        {
            case 3: // destroys the first heart
                Destroy(hearts[0]);
                break;
            case 2: // destroys the second heart
                Destroy(hearts[1]);
                break;
            case 1: // destroys the last heart
                Destroy(hearts[2]);
                break;
        }
        lives--; // removes one life
    }
    /**
     * Substracts n to the score and displays it
     * @param n : amount of score to substract
     */
    public void SubtractToScore(int n)
    {
        score -= n;
        textScore.text = "Score : " + score; // updates the score display
    }
}
