using UnityEngine;

public class Brick : MonoBehaviour
{
    public int wear; // value of wear of the brick
    public Sprite[] bricksSprites; // 3 sprites of the brick
    public GameObject refMaster; // refers to the game master
    private GameMasterBB _gameMasterScript; // script of the game master referred
    private SpriteRenderer _gameMasterSprite; // sprite renderer of the game master referred

    private void Start()
    {
        _gameMasterScript = refMaster.GetComponent<GameMasterBB>();       //gets the two component
        _gameMasterSprite = gameObject.GetComponent<SpriteRenderer>();
    }
    /**
     * Called when the ball hits a brick, wears the brick and destroys it if the wear is at 0 already
     */
    public bool OnHit()
    {
        switch (wear)
        {
            case 2: case 1: // if the brick isn't broken
                _gameMasterSprite.sprite = bricksSprites[wear-1]; // changes the sprite to a more worn one
                wear--; // lowers the value of the wear
                return false; // returns false to indicate the brick is still here
            case 0: // the brick is destroyed
                _gameMasterScript.brickNb--; // lowers the number of bricks by one
                if (_gameMasterScript.brickNb <= 0) _gameMasterScript.RestartGame(); // if the number of bricks is at 0, restarts the game
                Destroy(this.gameObject); // destroys this brick
                return true; // returns true to indicate the brick has been destroyed
            default:
                return false;
        }
    }
}
