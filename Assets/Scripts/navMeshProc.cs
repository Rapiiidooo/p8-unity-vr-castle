using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class navMeshProc : MonoBehaviour {

    private bool neverloop = true;

    public NavMeshSurface surface;
    public GameObject player;
    
    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
		
        if(player.transform.position != new Vector3(0,0,0) && neverloop)
        {
            //surface.BuildNavMesh();
            neverloop = false;
        }
    }
}
