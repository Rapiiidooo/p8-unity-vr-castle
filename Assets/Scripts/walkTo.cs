using UnityEngine;
using System.Collections;
    
public class walkTo : MonoBehaviour 
{
    private bool neverloop = true;
    public GameObject player;
    public Transform goal;
   
   void Start () 
   {

   }

   void Update()
   {
        if (player.transform.position != new Vector3(0, 0, 0) && neverloop)
        {
            UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

            Vector3 test = goal.position;
            test.y += 5;
            agent.destination = goal.position;
            neverloop = false;
        }
   }
}
