using log4net;
using System;
using UnityEngine;

public class Chase : MonoBehaviour {

    public Transform player;
    public Transform head;
    static Animator anim;
    bool focused = false;

    private static readonly ILog Logger = LogManager.GetLogger("Chase");

    // Use this for initialization
    void Start ()
    {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        try
        {
            float terrainHeight = Terrain.activeTerrain.SampleHeight(this.transform.position);

            Vector3 direction = player.position - this.transform.position;
            direction.y = 0;
            float angle = Vector3.Angle(direction, head.up);
            //Debug.Log(angle);

            if (Vector3.Distance(player.position, this.transform.position) < 10 && (angle < 35 || focused))
            {
                focused = true;

                anim.SetBool("isIdle", false);
                if (direction.magnitude > 2.5)
                {
                    anim.SetBool("isWalking", true);
                    anim.SetBool("isAttacking", false);

                    this.transform.Translate(0, 0, 0.02f);
                    this.transform.position = new Vector3(this.transform.position.x, terrainHeight, this.transform.position.z);
                    this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction), 0.1f);

                    /*
                    if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                    {
                        // do something
                    }
                    */
                }
                else
                {
                    anim.SetBool("isAttacking", true);
                    anim.SetBool("isWalking", false);
                }
            }
            else
            {
                focused = false;
                /*
                anim.SetBool("isIdle", true);
                anim.SetBool("isWalking", false);
                anim.SetBool("isAttacking", false);
                */
            }
        }
        catch (Exception e)
        {
            Logger.Error(Logger.Logger.Name + " " + e.Message);
        }
	}
}
