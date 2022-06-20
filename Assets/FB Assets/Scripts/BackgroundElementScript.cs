using UnityEngine;

public class BackgroundElementScript : MonoBehaviour
{
    public BackgroundScript bgScript;
    public string type;
    public float width;
    public float speed;
    private bool _hasRecursived = false;

    private void Update()
    {
        transform.Translate(-speed * Time.deltaTime, 0, 0);
        if (transform.position.x < -15 - width / 2)
        {
            Destroy(this.gameObject);
        }

        if (transform.position.x < 9 - width / 2 && !_hasRecursived)
        {
            _hasRecursived = true;
            bgScript.CreateNewBgElement(this.type);
        }
    }
}
