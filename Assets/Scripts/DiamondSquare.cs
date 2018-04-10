using Assets.Scripts;
using Boo.Lang;
using log4net;
using UnityEngine;
using UnityEngine.AI;

public class DiamondSquare : MonoBehaviour {
    Terrain my_terrain;
    public int Details;//Must be 0 or 1, TODO: Changer ça en valeur restreinte
    public float dimension_fractale;
    public float minstart, maxstart;
    private int heighmap_width;
    private int heighmap_height;
    private float[,] toNorm;

    public GameObject player;
    public GameObject chateau;

    [System.Serializable]
    public class Envmt
    {
        public float startingHeight;
        public float endingHeight;
        public int nbElement;
        public GameObject obj;
    }

    public Envmt[] listEnvmt;

    //A faire une fois seulement, sert pour configurer le logger
    Log log;
    
    //A écrire dans chaque classe ou l'on veut Logger un event
    private static readonly ILog Logger = LogManager.GetLogger("DiamondSquare");

    // Terrain painting ------------------
    [System.Serializable]
    public class SplatHeights
    {
        public int textureIndex;
        public float startingHeight;
    }

    public SplatHeights[] splatHeights;
    // ------------------------------------

    int find_pow(int num)
    {
        int res = 0;
        while(num > 1)
        {
            num /= 2;
            res++;
        }
        return res;
    }

    void my_normalize(int widthsize, int heighsize)
    {
        float max = toNorm[0, 0], min = toNorm[1, 0], tmp;
        
        if (max < min)
        {
            tmp = max;
            max = min;
            min = tmp;
        }

        //Trouve le minimum et le maximum d'un tableau
        for (int x = 0; x < widthsize; x++)
        {
            for (int y = 0; y < heighsize; y++)
            {
                if (toNorm[y,x] > max)
                {
                    max = toNorm[y, x];
                }
                else if(toNorm[y,x] < min)
                {
                    min = toNorm[y, x];
                }
            }
        }

        for (int x = 0; x < widthsize; x++)
        {
            for(int y = 0; y < heighsize; y++)
            {
                toNorm[y, x] = ((toNorm[y,x] - min)/(max - min));
            }
        }

    }

    void paintTerrain()
    {
        //float amplitude = 1.0f;
        TerrainData terrainData = my_terrain.terrainData;
        float[,,] splatmapData = new float[terrainData.alphamapWidth,
                                            terrainData.alphamapHeight,
                                            terrainData.alphamapLayers];
        float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                //print("Height test: " + terrainData.GetHeight(y, x) + " | " + heights[y, x]);
                float terrainHeight = heights[y, x];

                float[] splat = new float[splatHeights.Length];
                for (int i = 0; i < splat.Length; i++)
                {
                    splat[i] = 0;
                }
                
                for (int i = 0; i < splatHeights.Length; i++)
                {
                    //Ici il faudra calculer l'amplitude ...

                    if (i == splatHeights.Length - 1 && terrainHeight >= splatHeights[i].startingHeight)
                        splat[i] = 1;
                    //superposition des textures
                    /*
                    else if (terrainHeight <= (splatHeights[i+1].startingHeight + (0.05 * amplitude)) &&
                        terrainHeight >= (splatHeights[i+1].startingHeight - (0.05 * amplitude)) && i+1 < splatHeights.Length-1)
                    {
                        splat[i] = 0.5f;
                        splat[i + 1] = 0.5f;
                    }
                    */
                    else if (terrainHeight >= splatHeights[i].startingHeight &&
                        terrainHeight <= splatHeights[i + 1].startingHeight)
                        splat[i] = 1;
                }

                for (int j = 0; j < splatHeights.Length; j++)
                {
                    splatmapData[y, x, j] = splat[j];
                }
            }
        }

        //print("Height test: "+ terrainData.GetHeight(0,0) + " | "+ heights[0,0]);
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    //Compute heights
    void Compheights(int widthsize, int heightsize)
    {

        toNorm = my_terrain.terrainData.GetHeights(0, 0, heighmap_width, heighmap_height);
        //Init Corners
        toNorm[0, 0]
            = Random.Range(minstart, maxstart);

        toNorm[0, widthsize-1]
            = Random.Range(minstart, maxstart);

        toNorm[heightsize-1, 0]
            = Random.Range(minstart, maxstart);

        toNorm[heightsize-1, widthsize-1]
            = Random.Range(minstart, maxstart);

        //Diamond
        int step = widthsize - 1;
        float moyenne, sum;
        int profondeur = 0;
        int jump = 0;
        int fois;
        int semi_step;
        while (step > 1)
        {
            semi_step = step / 2;
            //Diamond phase (Square selon Belhadj)
            for(int x = semi_step; x  < widthsize; x+= step)
            {
                for (int y = semi_step; y < heightsize; y += step)
                {
                    moyenne 
                        =(toNorm[y - semi_step, x - semi_step] 
                        + toNorm[y + semi_step, x - semi_step] 
                        + toNorm[y + semi_step, x + semi_step] 
                        + toNorm[y - semi_step, x + semi_step]) / 4;
                    
                    toNorm[y, x] = moyenne + ((Random.Range(0.0f, 1.0f) * 2.0f - 1.0f) * Mathf.Sqrt(2) * (widthsize / Mathf.Pow(2, dimension_fractale * profondeur)));
                         
                    //print("First phase :" + toNorm[y, x]);
                }
            }

            //Square phase (Diamond selon Belhadj)
            for(int x = 0; x < widthsize; x += semi_step)
            {
                if( x % step == 0)
                {
                    jump = semi_step;
                }
                else
                {
                    jump = 0;
                }
                for(int y = jump; y < widthsize; y+= step)
                {
                    sum = 0;
                    fois = 0;
                    if (x >= semi_step)
                    {
                        sum += toNorm[y, x - semi_step];
                        fois++;
                    }
                    if (x + semi_step < widthsize)
                    {
                        sum += toNorm[y, x + semi_step];
                        fois++;
                    }
                    if (y >= semi_step)
                    {
                        sum += toNorm[y - semi_step, x];
                        fois++;
                    }
                    if (y + semi_step < widthsize)
                    {
                        sum += toNorm[y + semi_step, x];
                        fois++;
                    }
                    toNorm[y,x] = (sum / fois) + ((Random.Range(0.0f, 1.0f) * 2.0f - 1.0f) * (widthsize / Mathf.Pow(2, dimension_fractale * profondeur)));

                    //print("Second phase :" + heights[y, x]);
                }
            }
            profondeur++;
            step = semi_step;
        }
        my_normalize(widthsize, heightsize);
        my_terrain.terrainData.SetHeights(0, 0, toNorm);
    }

    void init_chatacter ()
    {
        /*
         * //debut de map
        Vector3 newPosition = new Vector3(character.transform.position.x + 5,
                                                    character.transform.position.y,
                                                    character.transform.position.z + 5);

        float terrainHeight = Terrain.activeTerrain.SampleHeight(newPosition);

        character.transform.position = new Vector3(character.transform.position.x + 5,
                                                            terrainHeight + 5,
                                                            character.transform.position.z + 5);
        */

        //au dessus des portes du chateau
        player.transform.position = new Vector3(chateau.transform.position.x,
                                                            chateau.transform.position.y+20,
                                                            chateau.transform.position.z+28);
    }

    Vector3 get_emplacement_chateau(float chateau_x, float chateau_y, float chateau_z, int ecart)
    {
        bool trouver;

        TerrainData terrainData = my_terrain.terrainData;
        //float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight);

        //Debug.Log(terrainData.GetHeight(0, 0));
        //Debug.Log("terrainData.size.x : " + terrainData.size.x);
        //Debug.Log("terrainData.size.z : " + terrainData.size.z);

        //Debug.Log("chateau_x : " + chateau_x);
        //Debug.Log("chateau_z : " + chateau_z);
        //Debug.Log("chateau_y : " + chateau_y);
        float hauteur_min, pente_max, pente_init, pente_actuel;

        hauteur_min = terrainData.size.y * (0.4f * 100) / 100;
        pente_max = terrainData.size.y * (0.1f * 100) / 100;

        for (int j = (int)chateau_x/2 + ecart; j < terrainData.size.x - ecart - ((int)chateau_x / 2); j++)
        {
            for(int i = (int)chateau_z/2 + ecart; i < terrainData.size.z - ecart - ((int)chateau_z / 2); i++)
            {
                trouver = true;
                pente_init = my_terrain.SampleHeight(new Vector3(i, 0, j));
                //voir si l'emplacement choisi est sur l'eau, ou n'est pas assez plat
                for (int x = j; x < j + chateau_x; x++)
                {
                    for(int z = i; z < i + chateau_z; z++)
                    {
                        // valeur heightmap
                        //hauteur_min > heights[i, j]
                        if (hauteur_min >= my_terrain.SampleHeight(new Vector3(i, 0, j)))
                        //hauteur = terrainData.GetHeight(i, j); // valeur unity
                        {
                            i += z; // on avance pour ne pas perdre de temps
                            trouver = false;
                            break;
                        }
                        pente_actuel = my_terrain.SampleHeight(new Vector3(i, 0, j));
                        if (pente_actuel < 0) pente_actuel = -1 * pente_actuel;

                        if (pente_init - my_terrain.SampleHeight(new Vector3(i, 0, j)) >= pente_max)
                        {
                            i += z; // on avance pour ne pas perdre de temps
                            break;
                        }
                    }
                    if (trouver == false)
                        break;
                }

                if (trouver == true)
                {
                    Logger.Debug("x = " + (i + (int)chateau_x / 2));
                    Logger.Debug("z = " + (j + (int)chateau_z / 2));
                    return new Vector3(i + (int)chateau_x / 2 + ecart, my_terrain.SampleHeight(new Vector3(i, 0, j)) - 2, j + (int)chateau_z / 2 + ecart);
                }
            }
        }
        Logger.Debug("Aucune location trouvée pour le chateau...");

        return new Vector3(0, 0, 0);
    }

    void init_chateau ()
    {
        float x, y, z;
        x = y = z = 0.0f;

        foreach (Transform child in chateau.GetComponentInChildren<Transform>())
        {
            if(child && child.GetComponent<Collider>())
            {
                //Debug.Log(child.GetComponent<Collider>().bounds.size.x);
                //Debug.Log(child.GetComponent<Collider>().bounds.size.y);
                //Debug.Log(child.GetComponent<Collider>().bounds.size.z);

                if (x < child.GetComponent<Collider>().bounds.size.x)
                    x = child.GetComponent<Collider>().bounds.size.x;

                if (y < child.GetComponent<Collider>().bounds.size.y)
                    y = child.GetComponent<Collider>().bounds.size.y;

                if (z < child.GetComponent<Collider>().bounds.size.z)
                    z = child.GetComponent<Collider>().bounds.size.z;
            }
        }
        chateau.transform.position = get_emplacement_chateau(x, y, z, 20); // 4 eme argument pour avoir un petit écart entre l'eau et le chateau
    }

    void init_environnement ()
    {
        //float[,] heights = my_terrain.terrainData.GetHeights(0, 0, my_terrain.terrainData.heightmapWidth, my_terrain.terrainData.heightmapHeight);
        //Logger.Debug("Height test: " + my_terrain.terrainData.GetHeight(100, 100) + " | " + heights[100, 100] + " | " + (my_terrain.terrainData.size.y * (heights[100, 100] * 100) / 100));
        List<Tuple> listRdmBusy = new List<Tuple>();
        Tuple positions = new Tuple(0,0);
        int rdmx = 0;
        int rdmz = 0;
        bool busy = true;
        bool keepGoing = true;

        while(keepGoing)
        {
            #region verif si le nombre d'element restant est de 0
            int totaltmp = 0;
            foreach (var elem in this.listEnvmt)
            {
                totaltmp += elem.nbElement;
            }
            if (totaltmp <= 0)
                keepGoing = false;
            #endregion

            while (busy == true)
            {
                rdmx = (int)Random.Range(0, my_terrain.terrainData.bounds.size.x);
                rdmz = (int)Random.Range(0, my_terrain.terrainData.bounds.size.z);
                positions = new Tuple(rdmx, rdmz);
                busy = false;
                foreach(var elem in listRdmBusy)
                {
                    if (elem.rdmx == rdmx && elem.rdmz == rdmz)
                    {
                        busy = true;
                        break;
                    }
                }
            }
            listRdmBusy.Push(positions);

            foreach (var elem in this.listEnvmt)
            {
                if (elem.nbElement > 0)
                {
                    var hauteurmin = my_terrain.terrainData.size.y * (elem.startingHeight * 100) / 100;
                    var hauteurmax = my_terrain.terrainData.size.y * (elem.endingHeight * 100) / 100;
                    var hauteurRdm = my_terrain.SampleHeight(new Vector3(rdmx, 0, rdmz));
                    if (hauteurRdm >= hauteurmin && hauteurRdm <= hauteurmax)
                    {
                        Vector3 spawnPosition = new Vector3(rdmx, hauteurRdm, rdmz);
                        Instantiate(elem.obj, spawnPosition, this.transform.rotation, this.transform.parent);
                        elem.nbElement--;
                        break;
                    }
                }
            }
            busy = true;
        }
    }

    // Use this for initialization
    void Start()
    {
        log = new Log();
        if (log == null) Debug.Log("Le logger n'est pas correctement instancié");
        try {
            Logger.Info("Début - Génération Terrain.");
            my_terrain = Terrain.activeTerrain;
            heighmap_width = my_terrain.terrainData.heightmapWidth;
            heighmap_height = my_terrain.terrainData.heightmapHeight;
            my_terrain.heightmapMaximumLOD = Details; // Minimum details: 1 , max details: 0
            Compheights(heighmap_width, heighmap_height);
            paintTerrain();
            Logger.Info("Fin - Génération Terrain");
            Logger.Info("Début - Placement des objets / environnement / player");
            init_chateau();
            init_chatacter();
            //init_ennemies();
            init_environnement();
            Logger.Info("Fin - Placement des objets / environnement / player");

        }
        catch(System.Exception e)
        {
            Logger.Error(Logger.Logger.Name + " " + e.Message);
        }
    }

    // Update is called once per frame
    void Update()
    {
             
    }
}