using UnityEngine;

public class PipeScript : MonoBehaviour
{
    [SerializeField] public GameObject gameMaster;
    [SerializeField] public GameObject bird;
    private const float PipeSpeed = 4f;
    private const float DespawnPosX = -12f;
    private bool _hasPastBird = false;
    private bool _isMoving;
    private int _direction; // +1 : up ; -1 : down
    public float startingY;

    private void Start()
    {
        int[] dir = new int[]{-1,1};
        _direction = dir[Random.Range(0,2)];
        _isMoving = Random.Range(0f, 1f) > .60;
    }
    private void Update()
    {
        if (_isMoving)
        {
            if (this.transform.position.y > startingY + .5 && _direction == 1)
            {
                _direction = -1;
            }
            else if (this.transform.position.y < startingY - .5 && _direction == -1)
            {
                _direction = 1;
            }
            this.transform.Translate(0,PipeSpeed / 2 * Time.deltaTime * _direction, 0);
        }
        if (!_hasPastBird && this.transform.position.x <= -5)
        {
            _hasPastBird = true;
            bird.GetComponent<BirdScript>().AddToScore(1);
        }
        transform.Translate(-PipeSpeed * Time.deltaTime, 0, 0);
        if (transform.position.x < DespawnPosX)
        {
            Destroy(gameObject);
        }
    }
}