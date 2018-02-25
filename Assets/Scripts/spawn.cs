using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class spawn : MonoBehaviour {

    public int numbers;
	
	public GameObject[] enemies;
    public float spawnWait;
    public float spawnMostWait;
    public float spawnLeastWait;
    public int startWait;
    public bool stop;

    int randEnnemy;

    /*
    //Au Runtime
    void Start()
    {
        while (!stop)
        {
            if (numbers <= 0)
                stop = true;
            randEnnemy = Random.Range(0, enemies.Length);

            Vector3 spawnPosition = new Vector3(this.transform.position.x + Random.Range(-50, 50), this.transform.position.y, this.transform.position.z + Random.Range(-10, 10));

            Instantiate(enemies[randEnnemy], spawnPosition, this.transform.rotation, this.transform.parent);
            numbers--;
        }
    }

    private void Update()
    {

    }
    */


    //Avec timer
    
    void Start()
	{
        StartCoroutine(waitSpawner());
	}

    private void Update()
    {
        spawnWait = Random.Range(spawnLeastWait, spawnMostWait);
    }

    IEnumerator waitSpawner ()
    {
        yield return new WaitForSeconds(startWait);

        while(!stop)
        {
            if (numbers <= 0)
                stop = true;
            randEnnemy = Random.Range(0, enemies.Length);

            Vector3 spawnPosition = new Vector3(this.transform.position.x + Random.Range(-50, 50), this.transform.position.y+5, this.transform.position.z + Random.Range(-10, 10));


            //to avoid : "Failed to create agent because it is not close enough to the NavMesh"
            NavMeshHit closestHit;
            if (NavMesh.SamplePosition(spawnPosition, out closestHit, 500, 1))
            {
                Instantiate(enemies[randEnnemy], closestHit.position, this.transform.rotation, this.transform.parent);
            }

            //Instantiate(enemies[randEnnemy], spawnPosition, this.transform.rotation, this.transform.parent);

            numbers--;

            yield return new WaitForSeconds(spawnWait);
        }
    }
    
}
