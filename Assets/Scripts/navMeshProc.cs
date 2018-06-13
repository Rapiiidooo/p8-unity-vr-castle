using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class navMeshProc : MonoBehaviour {

    public NavMeshSurface surface;
    public GameObject player;

    public static bool navMeshFinish = false;
    
    // Use this for initialization
    void Start () {
        StartCoroutine(WaitTerrain());
    }

    IEnumerator WaitTerrain()
    {
        yield return new WaitUntil(() => player.transform.position != new Vector3(0, 0, 0));

        surface.BuildNavMesh();
        navMeshFinish = true;
    }

    // Update is called once per frame
    void Update () {
    }
}
