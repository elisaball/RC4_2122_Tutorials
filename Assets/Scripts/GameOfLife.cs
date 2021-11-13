using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{

    [SerializeField]
    private Vector3Int _gridDimensions = new Vector3Int(10, 10, 10);       //declaring and initializing the DIMENSIONS for the grid

    private VoxelGrid _grid;                                               //declaring the GRID ITSELF

    private int _currentYLayer;
    
    // Start is called before the first frame update
    void Start()
    {
        // we create the voxelgrid-initializing the _grid:
        _grid = new VoxelGrid(_gridDimensions);

        // for every voxel in the grid:
        for (int x = 0; x < _gridDimensions.x; x++)
        {
            for (int y = 0; y < _gridDimensions.y; y++)
            {
                for (int z = 0; z < _gridDimensions.z; z++)
                {
                    // we randomly turn some voxels off (one by one):
                    Vector3Int voxelIndex = new Vector3Int(x, y, z);        //unity takes the coordinates of the current iteration made from the loop section (for:lines 19-21-23)
                    Voxel currentVoxel = _grid.GetVoxelByIndex(voxelIndex); //in the variable typed Voxel find the voxel in those specific coordinates
                    currentVoxel.Alive = false;                                                 //setting the voxel as dead so that I can turn a random number alive
                }
            }
        }


        for (int x = 0; x < _gridDimensions.x; x++)                                             //since we want to work layer by layer, we set a for loop to create sets of 2 coordinates (instead of 3)
        {
            for (int z = 0; z < _gridDimensions.z; z++)
            {
                Vector3Int voxelIndex = new Vector3Int(x, 0, z);                                //we create the sets of coordinates, y=0 because we want to work layer by layer
                Voxel currentVoxel = _grid.GetVoxelByIndex(voxelIndex);
                if (Random.value < 0.8f)                                                        //setting a random number of voxels as alive
                {
                    currentVoxel.Alive = true;
                }
            }
        }



        StartCoroutine("AutomaticGOL");                                     //coroutine implies that it works over the time, so even if it is called once (in start) it keeps running
    }

    // Update is called once per frame
    void Update()
    {
        PerformRaycast();
    }

    private void PerformRaycast()
    {
        //whenever we click the left mouse button:
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Voxel")
                {
                    GameObject hitObject = hit.transform.gameObject;
                    var voxel = hitObject.GetComponent<VoxelTrigger>().AttachedVoxel;

                    //if the voxel is alive make it dead, if it's dead make it alive:
                    voxel.Alive = !voxel.Alive;
                }
            }
        }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 70, 50, 30), "Play !"))
        {
            DoGameOfLifeIteration(_currentYLayer);
        }
    }

    private void DoGameOfLifeIteration(int y)
    {
        //for each voxel in the grid
        for (int x = 0; x < _gridDimensions.x; x++)                                                 //we deleted the for loop in y since we are copying the bottom one
        {
            for (int z = 0; z < _gridDimensions.z; z++)
            {
                Vector3Int voxelIndex = new Vector3Int(x, y, z);                     //unity takes the coordinates of the current iteration made from the loop section and creates a vector
                Voxel currentVoxel = _grid.GetVoxelByIndex(voxelIndex);              //in the variable typed Voxel find the voxel in those specific coordinates

                List<Voxel> neighbours = currentVoxel.GetNeighbourList();            //for the specific voxel get the neighbours and create a list where to store them
                int numberOfLivingNeighbours = 0;
                foreach (Voxel neighbour in neighbours)                              //for each voxel in the list of neighbours:
                {
                    if (neighbour.Alive && neighbour.Index.y == y)                                   //if the neighbour is alive add it to the list of neighbours.
                    {                                                                                //if the y value of neighbor is the same of the current voxel the neighbor is kept
                        numberOfLivingNeighbours++;
                    }
                }

                if (currentVoxel.Alive)                                              //IF the current voxel is alive and if the number of neighbour is 2 or 3 make it alive, otherwise make it dead.
                {
                    if (numberOfLivingNeighbours == 2 || numberOfLivingNeighbours == 3)
                    {
                        currentVoxel.Alive = true;
                    }
                    else
                    {
                        currentVoxel.Alive = false;
                    }

                }                                                                   //OTHERWISE (the current voxel is dead) and if the number of neighbours is 3, make the current voxel alive.
                else
                {
                    if (numberOfLivingNeighbours == 3)
                    {
                        currentVoxel.Alive = true;                                  //in all the other cases the voxel stays DEAD.
                    }
                }
            }
        }

    }



    IEnumerator AutomaticGOL()                                                          //coroutine creation 
    {
        for (int y = 1; y < _gridDimensions.y; y++)                                   //for every y layer:
        {
            _currentYLayer = y;
            CopyTheBottomLayerToTopOne(y-1);                                              //we copy the bottom one,
            DoGameOfLifeIteration(y);                                                    //we play GOL just on the layer above not to change the previous layer,
            yield return new WaitForSeconds(1f);                                        //every second.
        }
    }


    void CopyTheBottomLayerToTopOne(int y)
    {
        for (int x = 0; x < _gridDimensions.x; x++)                                     //for each set of coordinates x,z:
        {
            for (int z = 0; z < _gridDimensions.z; z++)
            {
                Vector3Int voxelIndex = new Vector3Int(x, y, z);                   //we take the coordinates considering that specific y layer
                Voxel currentVoxel = _grid.GetVoxelByIndex(voxelIndex);                 //we create the voxel with those coordinates

                Vector3Int voxelAboveCoordinates = new Vector3Int(x, y + 1, z);      //we move to the layer above, consider the new specific y layer
                Voxel voxelAbove = _grid.GetVoxelByIndex(voxelAboveCoordinates);        //we create the voxel with the new coordinates

                voxelAbove.Alive = currentVoxel.Alive;                                  //we copy the status of the currentvoxel to the one above
            }
        }
    }
}
