using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMasterBB : MonoBehaviour
{
    private readonly Vector2 _brickOrigin = new Vector2(-5.25f, 3.4f); // origin on the top left of the first brick
    private readonly Vector2 _brickSize = new Vector2(1.5f, .7f); // size of a brick (width,height)
    private const float SpawnRate = .75f; // rate of spawning a brick
    [SerializeField] protected GameObject refBrick; // refers to the brick prefab
    [SerializeField] protected GameObject refBall; // refers to the ball
    private Ball _ballScript;
    [SerializeField] protected TextMeshPro refLevelText; // refers to the ball
    [SerializeField] protected TextMeshPro refNextLevelText; // refers to the ball
    [SerializeField] protected TextMeshPro refPauseText;
    public int brickNb; // number of bricks on the game
    private int _level = 1;
    private bool _paused = false;
    
    private void Start()
    {
        _ballScript = refBall.GetComponent<Ball>();
        GenerateBricks(SpawnRate); // Generates a grid of bricks randomly at the start
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1;
            StartCoroutine(TitleScript.LoadScene("Menu")); // goes back to the menu
        }
        if (_ballScript.lives > 0)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                _paused = _paused ? ResumeGame() : PauseGame();
            } 
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Time.timeScale = 1;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reloads the whole scene to restart the game
            }
        }
        
        
    }

    private bool PauseGame()
    {
        refPauseText.transform.position = new Vector3(0, 0, -5);
        Time.timeScale = 0;
        return true;
    }

    private bool ResumeGame()
    {
        refPauseText.transform.position = new Vector3(0, 0, 10);
        Time.timeScale = 1;
        return false;
    }
    /**
     * Restarts the game by making a new random grid and respawning the ball
     */
    public void RestartGame()
    {
        StartCoroutine(DisplayNextLevel());
        _level++;
        refLevelText.text = "Level : " + _level;
        StartCoroutine(refBall.GetComponent<Ball>().Respawn(false)); // respawns the ball
        GenerateBricks(SpawnRate); // generates a new random grid of bricks
    }
    /**
     * Displays next level text and image   
     */
    private IEnumerator DisplayNextLevel()
    {
        refNextLevelText.transform.position = new Vector3(0,0,-5);
        yield return new WaitForSeconds(1);
        refNextLevelText.transform.position = new Vector3(0,0,5);
    }
    /**
     * Generates a random grid of bricks
     * @param rate : rate of spawning a brick from 0 to 1
     */
    private void GenerateBricks(float rate)
    {
        for (var i = 0; i < 4; i++) // goes through the whole "grid" of bricks
        {
            for (var j = 0; j < 5; j++)
            {
                if (Random.Range(0f, 1f) <= rate) // Spawns a brick randomly if the random.range is in the [0,rate] interval
                {
                    var newBrick = Instantiate(refBrick, new Vector3(_brickOrigin.x + _brickSize.x * i, _brickOrigin.y - _brickSize.y * j, 0), Quaternion.identity); // instantiates a new brick on the left
                    var newBrickMirrored = Instantiate(refBrick, new Vector3(_brickOrigin.x + _brickSize.x * (7 - i), _brickOrigin.y - _brickSize.y * j, 0), Quaternion.identity); // instantiates another brick mirrored on the left
                    var wear = Random.Range(0, 3); // picks a random wear state
                    //var color = new Color(Random.Range(.7f, 1f), Random.Range(.7f, 1f), Random.Range(.7f, 1f), 1f); // sets random colors for the bricks
                    var color = newBrick.GetComponent<SpriteRenderer>().color;
                    SetBrickAttributes(newBrick,color,wear); // sets all the attributes of the brick 
                    SetBrickAttributes(newBrickMirrored,color,wear); // sets the same attributes than the other, just on the other side
                    brickNb += 2;
                }
            }
        }
    }
    /**
     * Sets all the attributes to a brick
     * @param brick : brick to update
     * @param color : the color to apply
     * @param wear : value of wear
     */
    private void SetBrickAttributes(GameObject brick, Color color, int wear) // sets all the attributes of a brick
    {
        var brickScript = brick.GetComponent<Brick>(); // Gets the Brick script of the brick
        var brickSprite = brick.GetComponent<SpriteRenderer>(); // Gets the sprite renderer of the brick
        brickScript.wear = wear; // sets the wear of the brick to the parameter
        brickSprite.sprite = brickScript.bricksSprites[wear]; // sets the sprite of the brick according to the wear
        brickSprite.color = color; // sets the color of the brick
        brickScript.refMaster = this.gameObject; // refers to the gameMaster
    }
}
