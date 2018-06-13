using UnityEngine;
using log4net;
using System;

public class walkTo : MonoBehaviour 
{
    private static readonly ILog Logger = LogManager.GetLogger("walkTo");

    private bool neverloop = true;

    static Animator anim;
    private UnityEngine.AI.NavMeshAgent agent;

   void Start () 
   {
        anim = GetComponent<Animator>();
        anim.SetBool("isWalking", true);
        anim.SetBool("isAttacking", false);
        anim.SetBool("isIdle", false);
    }

   void Update()
   {
        try
        {
            if (transform.parent.position != new Vector3(0, 0, 0) && neverloop)
            {
                agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
                Vector3 hauteur = transform.parent.position;
                hauteur.y += 5;
                agent.destination = transform.parent.position;
                neverloop = false;
            }
            
            //spécifique au skelette vérifie qu'il bouge, s'il bouge, l'animer
            /*
            if (this.transform.position == agent.destination)
            {
                anim.SetBool("isWalking", false);
                anim.SetBool("isAttacking", false);
                anim.SetBool("isIdle", true);
            }
            */
        }
        catch (Exception e)
        {
            Logger.Error(Logger.Logger.Name + " " + e.Message);
        }
   }
}
