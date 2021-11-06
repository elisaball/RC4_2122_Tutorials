using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
   
    [SerializeField]
    private Vector3Int _gridDimensions = new Vector3Int(10, 1, 10);

    private VoxelGrid _grid;
    // Start is called before the first frame update
    void Start()
    {   
        // we create the voxelgrid:
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
                    if (Random.value < 0.3f)                                //set a chance for this voxel to die:
                    {
                        currentVoxel.Alive = false;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        PerformRaycast();
        PerformGameOfLifeIteration();
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

    private void PerformGameOfLifeIteration()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DoGameOfLifeIteration();
        }
    }

    private void DoGameOfLifeIteration()
    { 
        //for each voxel in the grid
        for (int x = 0; x < _gridDimensions.x; x++)
        {
            for (int y = 0; y < _gridDimensions.y; y++)
            {
                for (int z = 0; z < _gridDimensions.z; z++)
                {
                    Vector3Int voxelIndex = new Vector3Int(x, y, z);            //unity takes the coordinates of the current iteration made from the loop section and creates a vector
                    Voxel currentVoxel = _grid.GetVoxelByIndex(voxelIndex);     //in the variable typed Voxel find the voxel in those specific coordinates

                    List<Voxel> neighbours = currentVoxel.GetNeighbourList();   //for the specific voxel get the neighbours and create a list where to store them
                    int numberOfLivingNeighbours = 0;
                    foreach (Voxel neighbour in neighbours)                     //for each voxel in the list of neighbours:
                    {                                                       
                        if (neighbour.Alive)                                    //if the neighbour is alive add it to the list of neighbours.
                        {
                            numberOfLivingNeighbours++;
                        }
                    }

                    if (currentVoxel.Alive)                                     //IF the current voxel is alive and if the number of neighbour is 2 or 3 make it alive, otherwise make it dead.
                    {
                        if (numberOfLivingNeighbours == 2 || numberOfLivingNeighbours == 3)
                        {
                            currentVoxel.Alive = true;
                        }
                        else
                        {
                            currentVoxel.Alive = false;
                        }

                    }                                                           //OTHERWISE (the current voxel is dead) and if the number of neighbours is 3, make the current voxel alive.
                    else
                    {
                        if (numberOfLivingNeighbours == 3)
                        {
                            currentVoxel.Alive = true;                          //in all the other cases the voxel stays DEAD.
                        }
                    }
                }
            }
        }
    }
}
