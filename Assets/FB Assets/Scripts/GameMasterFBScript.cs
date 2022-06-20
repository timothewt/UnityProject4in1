using System.Collections;
using UnityEngine;

public class GameMasterFBScript : MonoBehaviour
{
    [SerializeField] protected GameObject pipePrefab;
    [SerializeField] protected GameObject bird;
    private GameObject[] _pipes = new GameObject[3]{null,null,null};
    public bool isAlive = true;
    private const float MinPipeY = -3f;
    private const float MaxPipeY = 3f;

    private IEnumerator CreatePipe()
    {
        var newPipe = Instantiate(pipePrefab);
        var newY = Random.Range(MinPipeY, MaxPipeY);
        newPipe.transform.position = new Vector3(12f, newY, 0);
        var newPipeScript = newPipe.GetComponent<PipeScript>();
        newPipeScript.gameMaster = this.gameObject;
        newPipeScript.bird = this.bird;
        newPipeScript.startingY = newY;
        _pipes[0] = _pipes[1];
        _pipes[1] = _pipes[2];
        _pipes[2] = newPipe;
        yield return new WaitForSeconds(2);
        PipeCreator();
    }

    public void PipeCreator()
    {
        StartCoroutine(CreatePipe());
    }

    public void DestroyPipes()
    {
        foreach (var p in _pipes)
        {
            Destroy(p);
        }
    }
}
