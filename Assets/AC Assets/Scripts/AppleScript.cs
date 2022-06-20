using UnityEngine;

public class AppleScript : MonoBehaviour {

    private const float FallThreshold = -5f; // the apple is destroyed after reaching this point
    public BoxScript box; // refers to the box
    
    private void Update()
    {
        if (transform.position.y < FallThreshold) // if the apple falls below the threshold
        {
            if (CompareTag("pomme")) // if the game mode is with lives and this game object is an apple and not a bomb, removes a life
            {
                if (TitleScript.GameMode == 0)
                {
                    box.RemoveLife();
                }
                else
                {
                    box.SubtractToScore(5);
                }
            } 

            Destroy(gameObject); // destroys the apple
        }
        if (box.lives > 0) return; // if the player still has life does nothing and ends the Update method
        Destroy(gameObject); // else destroys the apple
    }

    private void OnCollisionEnter2D(Collision2D collision) // when there's a collision
    {
        if (collision.gameObject.name =="panier") // if it's with the box
        {
            Destroy(gameObject); // destroys the apple
        }
    }
}
