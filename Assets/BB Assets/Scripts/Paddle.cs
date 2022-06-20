using UnityEngine;

public class Paddle : MonoBehaviour
{
    private const float Speed = 10f; // horizontal speed of the paddle
    private int _direction; // direction of the ball, -1 is left 0 doesn't move 1 is right
    
    private void Update()
    {
        _direction = 0; // resets the direction to 0
        if (Input.GetKey(KeyCode.RightArrow) && transform.position.x < 5) // if the right arrow is pressed and paddle isn't out of bounds
        {
            _direction = 1; // sets direction to the right
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && transform.position.x > -5) // if the left arrow is pressed and paddle isn't out of bounds
        {
            _direction = -1; // sets direction to the left
        }
        transform.Translate(_direction * Speed * Time.deltaTime, 0, 0); // Translates the paddle to the right or left according to the user input
    }
}
