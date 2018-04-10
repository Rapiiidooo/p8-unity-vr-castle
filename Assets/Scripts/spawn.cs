using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using log4net;

public class spawn : MonoBehaviour {

    private static readonly ILog Logger = LogManager.GetLogger("spawn");

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

            Vector3 spawnPosition = new Vector3(this.transform.position.x + Random.Range(-50, 50), this.transform.position.y + 5, this.transform.position.z + Random.Range(-10, 10));

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
        try
        {
            Logger.Info("Début - Génération ennemies");
            StartCoroutine(waitSpawner());
        }
        catch (System.Exception e)
        {
            Logger.Error(Logger.Logger.Name + " " + e.Message);
        }
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
        Logger.Info("Fin - Génération ennemies");
    }
}
