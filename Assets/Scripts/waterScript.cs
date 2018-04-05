using log4net;
using System;
using UnityEngine;

public class waterScript : MonoBehaviour {

    private static readonly ILog Logger = LogManager.GetLogger("waterScript");

    private Terrain parentTerrain;
    private GameObject water;

    public float splatHeigth;


	// Use this for initialization
	void Start ()
    {
        try
        {
            Logger.Info("Début - Placement de l'eau");
            water = this.gameObject;
            // Parent du gameObject
            parentTerrain = Terrain.activeTerrain;
            var width = parentTerrain.terrainData.size.x;
            var lenght = parentTerrain.terrainData.size.z;
            var heigth = parentTerrain.terrainData.size.y;

            var finalheigth = heigth * (splatHeigth * 100) / 100;


            //DiamondSquare.splatHeights
            water.transform.localScale = new Vector3(width, 1, lenght);
            water.transform.position = new Vector3(water.transform.position.x + width / 2, finalheigth, water.transform.position.z + lenght / 2);
            Logger.Info("Fin - Placement de l'eau");
        }
        catch (Exception e)
        {
            Logger.Error(Logger.Logger.Name + " " + e.Message);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
