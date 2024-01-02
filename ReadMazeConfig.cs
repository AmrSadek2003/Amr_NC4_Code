using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;
using MessagePack;
using MessagePack.Formatters;


public struct PathConfig
{
    public ushort[] cellNum;
    public ushort[] wallConfig;
    public ushort[] checkIncluded;
}

public class ReadMazeConfig : MonoBehaviour
{
    public static readonly string uniqueIdentifier1 = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mmss");

    string binaryDataPath;

    /*public static readonly string binaryDataPath = "C:\\Users\\amrsa\\Desktop\\GameData\\" + uniqueIdentifier1 + "_continousdata.dat";*/

    // Path to file containing list of paths
    public static string pathListFile = "Assets/Paths/PathFolder2.csv";
    // List of paths
    public static string[] pathList;

    public static string path;
    public bool pathInitialized;

    public static int currentPathIdx = 0;
    public static int filePathIdx = 800;
    public static int currentState;
    private string pathI;
    private string pathIplusOne;
    private string pathIminusOne;
    private string[] reading;
    private int colOne;
    private int colTwo;
    private int colThree;
    private int rowOne;
    private int rowTwo;
    private int rowThree;
    private int rowOneOne;
    private int colOneOne;

    private float timer = 0f;
    public float samplingInterval = 1f;

    private Vector3 currentPosition;
    private Quaternion currentRotation;

    private string dataFileName;


    private ushort[] myCellFirst;
    private ushort[] myCellSecond;
    private ushort[] myCellThird;

    public static string currentPathFile;
    private string filePathCopy;

    public static List<PathConfig> pathConfigs = new List<PathConfig>();


    public static int checkmark = 1;

    // Wall
    float widthCell = 300f;
    float thickWall = 20f;
    float heightWall = 50f;
    Vector3 wall;
    Vector3 scaleWall;
    Vector3 scaleWindow;
    Vector3 positionWindow;
    Quaternion rotationWindow;

    int[] angleRot = new int[] { 0, -45, -90, -135, 180, 135, 90, 45 };
    public static int numWalls = 8; // Number of walls per cell
    public static int numRows = 7;
    public static int numCols = 7;
    public static GameObject[,,] walls = new GameObject[7, 7, 8]; // [numRows, numCols, numWalls+2]
    public static GameObject[,,] windows = new GameObject[7, 7, 8];
    List<GameObject> duplicatedWalls = new List<GameObject>();
    public static GameObject reward, mageTower, commonHouse, commonHouseOne, turbine, house, lightHouse, startDoor, endDoor, roof, squareRoof, popUp;
    public static ArrayList wallsToBeRaised = new ArrayList(); //Each element is (row, col, wall_index)
    int count_CellNum = 0;
    int cellPositionNum = 0;
    int squareX = 0;
    int squareZ = 0;
    int sum = 0;
    public ushort startCell = 0;
    ushort rewardCell = 0;
    ushort popUpCell = 0;
    ushort currentCell = 0;
    int shiftLeftStart = 0;
    int shiftUpStart = 0;
    private GameObject player;
    Vector3 positionPlayer;
    Quaternion rotationPlayer;

    Vector3 scaleSquareRoof;
    Vector3 positionSquareRoof;
    Quaternion rotationSquareRoof;

    Vector3 scaleRoof;
    Vector3 positionRoof;
    Quaternion rotationRoof;

    Vector3 positionRoom;

    float widthReward = 50f;
    float thickReward = 50f;
    float heightReward = 50f;
    Vector3 scalePopUp;
    Vector3 scaleReward;
    Vector3 positionReward;
    Quaternion rotationReward;

    Vector3 positionPopUp;
    Quaternion rotationPopUp;

    //public GameObject textInstruct;
    public GameObject textScore;
    public GameObject textReward;
    public GameObject arrow;
    private GameObject wallPrefab;  // Resources.Load<GameObject>("Wall1_C1_02");
    private GameObject windowPrefab;
    public static GameObject textStart;
    public static GameObject textAllocentric;
    public static GameObject bgTextScore;

    public GameObject redCone;

    public float scoreStart;

    public float scoreOne;

    public float scoreTwo;

    private void Awake()
    {
        wallPrefab = Resources.Load<GameObject>("Wall2_C2_05");
        windowPrefab = Resources.Load<GameObject>("Window5_01L");
    }

    void Start()
    {
        pathInitialized = false;
        binaryDataPath = Path.Combine(Application.persistentDataPath, uniqueIdentifier1 + "_continousdataMessagePack.dat");

        /*selectLandmarks = false;

        selectLH = false;

        selectCabin = false;

        moveOn = false;*/ 

        scaleWall = new Vector3(50, 50, 100);
        scaleWindow = new Vector3(160, 105, 70);
        scaleReward = new Vector3(thickReward, heightReward, widthReward);

        scalePopUp = new Vector3(50, 50, 300);

        scaleRoof = new Vector3(310, 8, 310);
        scaleSquareRoof = new Vector3(150, 8, 112);

        textScore = GameObject.FindWithTag("Text_Score");

        textStart = GameObject.FindWithTag("Text_Start");

        textAllocentric = GameObject.FindWithTag("Text_Allocentric");

        redCone = GameObject.FindWithTag("RedCone");
        lightHouse = GameObject.FindWithTag("LightHouse");
        house = GameObject.FindWithTag("House");

        startDoor = GameObject.FindWithTag("StartDoor");
        endDoor = GameObject.FindWithTag("EndDoor");

        bgTextScore = GameObject.FindWithTag("BGText_Score");

        bgTextScore.SetActive(false);

        startDoor.GetComponent<MeshRenderer>().enabled = false;
        endDoor.GetComponent<MeshRenderer>().enabled = false;

        redCone.GetComponent<MeshRenderer>().enabled = false;

        player = GameObject.FindGameObjectWithTag("Player");
        positionPlayer = new Vector3(0, 0, 0);
        rotationPlayer = Quaternion.Euler(0, 0, 0);

        currentPosition = player.transform.position;
        currentRotation = player.transform.rotation;

        // Generate all MAZE walls
        for (int i_wall = 0; i_wall < numWalls; i_wall++)
        {
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    GenerateWall(row, col, i_wall);
                    GenerateWindow(row, col, i_wall);
                }
            }
        }

        LoadPathConfigs();

        BinaryData();

    }

    void LoadPathConfigs()
    {
        pathList = File.ReadAllLines(pathListFile); //

        for (int k = 0; k < pathList.Length; k++)//
        {
            string[] cuts = pathList[k].Split(',');//
            pathList[k] = cuts[0];//
            pathConfigs.Add(ReadCSV(pathList[k])); //
        }
    }

    void Update()
    {
        scoreStart = CollisionReward.score_start;
        scoreOne = CollisionReward.score1;
        scoreTwo = CollisionReward.score2;

        currentPosition = player.transform.position;
        currentRotation = player.transform.rotation;

        textStart.GetComponent<TextMeshProUGUI>().enabled = true;
        textScore.GetComponent<TextMeshProUGUI>().enabled = true;
        

        if (currentPathIdx == 0 && CollisionReward.rewardState == 6)
        {

            bgTextScore.SetActive(true);
            textScore.GetComponent<TextMeshProUGUI>().text = "\nAverage Score for this Path: " + (scoreStart + scoreOne + scoreTwo) / 3 + "\nPrevious Path Average: Not Applicable";
        }

        if (currentPathIdx != 0 && CollisionReward.rewardState == 6)
        {
            bgTextScore.SetActive(true);
            textScore.GetComponent<TextMeshProUGUI>().text = "\nAverage Score for this Path: " + (scoreStart + scoreOne + scoreTwo) / 3 + "\nPrevious Path Average: " + CollisionReward.previousAverage;
        }


        if (currentPathIdx != filePathIdx)
        {
            textStart.GetComponent<TextMeshProUGUI>().enabled = true;

            if(currentPathIdx == 0)
            {
                textStart.GetComponent<TextMeshProUGUI>().text = "Look for the LightHouse and the Cabin!";
            }
            else
            {
                textStart.GetComponent<TextMeshProUGUI>().text = "Walk towards the end of the Path and touch the Cone!";
            }
            

            RaiseWalls(pathConfigs[currentPathIdx]);

            GenerateRoofs(pathConfigs[currentPathIdx], 643);
            GenerateSquareRoofs(pathConfigs[currentPathIdx], 643);
            GenerateRoofs(pathConfigs[currentPathIdx], 489);
            GenerateSquareRoofs(pathConfigs[currentPathIdx], 493);
            GenerateDoors();
            GenerateReward(pathConfigs[currentPathIdx]);
            //GeneratePopUp(pathConfigs[currentPathIdx]);
            filePathIdx = currentPathIdx;

        }

        //BinaryData(); 

    }


    PathConfig ReadCSV(string fileName) //correct 
    {
        string[] lines = File.ReadAllLines(fileName);

        int L = lines.Length;

        ushort[] cellNum = new ushort[L];
        ushort[] wallConfig = new ushort[L];
        ushort[] checkIncluded = new ushort[L];

        for (int i = 0; i < L; i++)
        {
            string[] values = lines[i].Split(',');
            cellNum[i] = ushort.Parse(values[0]);
            wallConfig[i] = ushort.Parse(values[1]);
            checkIncluded[i] = ushort.Parse(values[2]);
        }
        startCell = cellNum[0];

        if (checkIncluded[L - 1] == 1)
        {
            rewardCell = (ushort)(cellNum[L - 1] + 1) ; //Adjustment Factor
        }
        else
        {
            rewardCell = (ushort)(cellNum[L - 2] + 1); //Adjustment Factor
        }


        PathConfig p_config;
        p_config.cellNum = cellNum;
        p_config.wallConfig = wallConfig;
        p_config.checkIncluded = checkIncluded;

        return p_config;
    }

    void GenerateRoofs(PathConfig p_config, float height) //correct 
    {
        ushort[] checkIncluded = p_config.checkIncluded;
        ushort[] cellNum = p_config.cellNum;

        for (int i = 0; i < checkIncluded.Length; i++)
        {

            if (checkIncluded[i] == 1)
            {
                currentCell = (ushort)(cellNum[i] +1); //Adjustment Factor

                roof = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                roof.name = "Roof";
                roof.GetComponent<MeshRenderer>().material.color = Color.white;
                roof.transform.localScale = scaleRoof;
                positionRoof = new Vector3(-((currentCell) - 1) % numCols * widthCell, height, (int)(((currentCell) - 1) / numCols) * widthCell); //Adjustment Factor
                rotationRoof = Quaternion.Euler(0, 0, 0);
                roof.transform.SetPositionAndRotation(positionRoof, rotationRoof);
                roof.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                roof.GetComponent<MeshRenderer>().enabled = true;
                roof.GetComponent<Collider>().enabled = false;
                roof.tag = "RoofPath";

            }

        }

    }

    void GenerateSquareRoofs(PathConfig p_config, float height) //correct 
    {
        List<int> list = new List<int>();
        List<int> sortedList = new List<int>();
        List<int> holdVal = new List<int>();
        

        for (int i = 0; i < p_config.checkIncluded.Length; i++)
        {
            if (p_config.checkIncluded[i] == 0)
            {
                list.Add(p_config.cellNum[i]);
            }
        }

        /*list.Sort();

        for (int m = 0; m <= list.Count - 2; m++)
        {
            if (Math.Abs((list[m]+1) - (list[m + 1]+1)) == 8 || Math.Abs((list[m]+1) - (list[m + 1]+1)) == 6)
            {
                sum = (list[m]+1) + (list[m + 1]+1);
                holdVal.Add(sum);
            }
        }*/



        for (int i = 0; i < list.Count; i++)
        {
            for (int j = i + 1; j < list.Count; j++)
            {
                if (Math.Abs((list[j] + 1) - (list[i] + 1)) == 8 || Math.Abs((list[j] + 1) - (list[i] + 1)) == 6) //Adjustment Factor
                {
                    sum = (list[j] + 1) + (list[i] + 1); //Adjustment Factor
                    holdVal.Add(sum);
                }
            }
        }

        for (int k = 0; k < holdVal.Count; k++)
        {

            cellPositionNum = (int)(holdVal[k] / 2) - ((holdVal[k] - 10) / 14 + 4) + 49;

            squareX = -((int)(cellPositionNum - 50) % 6 * 300 + 150);

            squareZ = (int)(cellPositionNum - 50) / 6 * 300 + 150;

            squareRoof = GameObject.CreatePrimitive(PrimitiveType.Cube);
            squareRoof.name = "SquareRoof";
            squareRoof.GetComponent<MeshRenderer>().material.color = Color.white;
            squareRoof.transform.localScale = scaleSquareRoof;
            positionSquareRoof = new Vector3(squareX, height, squareZ);
            rotationSquareRoof = Quaternion.Euler(0, 45, 0);
            squareRoof.transform.SetPositionAndRotation(positionSquareRoof, rotationSquareRoof);
            squareRoof.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            squareRoof.GetComponent<MeshRenderer>().enabled = true;
            squareRoof.GetComponent<Collider>().enabled = false;

            squareRoof.tag = "RoofPath";

        }

    }

    Vector3 wallPosition(int row, int col, int i_wall)
    {
        Vector3 positionWall = new Vector3(widthCell / 2 * Mathf.Cos(Mathf.Deg2Rad * angleRot[i_wall]) + (shiftLeftStart - col) * widthCell,
                                    (heightWall / 2 + 475),
                                    widthCell / 2 * Mathf.Sin(Mathf.Deg2Rad * angleRot[i_wall]) + (-shiftUpStart + row) * widthCell);
        return positionWall;
    }

    Quaternion wallRotation(int row, int col, int i_wall)
    {
        Quaternion rotationWall = Quaternion.Euler(0, -angleRot[i_wall] + 90, 0);
        return rotationWall;
    }

    void GenerateWall(int row, int col, int i_wall)
    {
        Vector3 positionWall = wallPosition(row, col, i_wall);
        Quaternion rotationWall = wallRotation(row, col, i_wall);
        string wall_name = "Wall" + "_" + row + "_" + col + "_" + i_wall;
        if (GameObject.Find(wall_name) == null)
        {
            walls[row, col, i_wall] = Instantiate(wallPrefab, positionWall, rotationWall);
            walls[row, col, i_wall].name = wall_name;
            walls[row, col, i_wall].transform.localScale = scaleWall;
            walls[row, col, i_wall].tag = "Wall";
        }
    }

    void GenerateWindow(int row, int col, int i_wall)
    {
        string window_name = "Window" + "_" + row + "_" + col + "_" + i_wall;
        if (GameObject.Find(window_name) == null)
        {
            positionWindow = new Vector3(widthCell / 2 * Mathf.Cos(Mathf.Deg2Rad * angleRot[i_wall]) + (shiftLeftStart - col) * widthCell,
                                        (heightWall / 2 + 385.0f),
                                        widthCell / 2 * Mathf.Sin(Mathf.Deg2Rad * angleRot[i_wall]) + (-shiftUpStart + row) * widthCell + 2.90f);
            rotationWindow = Quaternion.Euler(0, -angleRot[i_wall] + 90, 0);
            windows[row, col, i_wall] = Instantiate(windowPrefab, positionWindow, rotationWindow);
            windows[row, col, i_wall].name = window_name;
            windows[row, col, i_wall].transform.localScale = scaleWindow;
            windows[row, col, i_wall].tag = "Window";
        }
    }


    void ActivateWall(int row, int col, int i_wall)
    {
        string wall_name = "Wall" + "_" + row + "_" + col + "_" + i_wall;
        if (GameObject.Find(wall_name) == null)
        {
            GenerateWall(row, col, i_wall);
        }
        else
        {
            walls[row, col, i_wall].GetComponent<MeshRenderer>().enabled = true;
            walls[row, col, i_wall].GetComponent<BoxCollider>().enabled = true;
        }
    }

    void ActivateWindow(int row, int col, int i_wall)
    {
        string window_name = "Window" + "_" + row + "_" + col + "_" + i_wall;
        if (GameObject.Find(window_name) == null)
        {
            GenerateWindow(row, col, i_wall);
        }
        else
        {
            windows[row, col, i_wall].SetActive(true);
            //windows[row, col, i_wall].GetComponent<MeshRenderer>().enabled = true;
            windows[row, col, i_wall].GetComponent<BoxCollider>().enabled = true;
        }
    }

    public static void DeactivateWindow(int row, int col, int i_wall)
    {
        string window_name = "Window" + "_" + row + "_" + col + "_" + i_wall;
        if (GameObject.Find(window_name) != null)
        {
            windows[row, col, i_wall].SetActive(false);
            windows[row, col, i_wall].GetComponent<BoxCollider>().enabled = false;
        }
    }

    public static void DeactivateWall(int row, int col, int i_wall)
    {
        string wall_name = "Wall" + "_" + row + "_" + col + "_" + i_wall;
        if (GameObject.Find(wall_name) != null)
        {
            walls[row, col, i_wall].GetComponent<MeshRenderer>().enabled = false;
            walls[row, col, i_wall].GetComponent<BoxCollider>().enabled = false;
        }
    }

    void RaiseWalls(PathConfig p_config) //READ FROM HERE
    {
        ushort[] cellNum = p_config.cellNum;
        ushort[] wallConfig = p_config.wallConfig;

        wallsToBeRaised.Clear();
        count_CellNum = 0;
        foreach (int cell in cellNum)
        {
            string binary_cell = Convert.ToString(wallConfig[count_CellNum], 2); // Convert int to binary, e.g., 239 -> 11101111, "1" = "wall up"
            for (int i = binary_cell.Length - 1; i >= 0; i--)
            {
                wall = new Vector3((int)(((cell+1) - 1) / numCols), ((cell+1) - 1) % numCols, (binary_cell.Length - 1) - i); //Adjustment Factor
                if (binary_cell[i].Equals('1'))
                {
                    wallsToBeRaised.Add(wall);
                }
            }
            count_CellNum = count_CellNum + 1;
        }

        // Deactivate all walls (down)
        for (int i_wall = 0; i_wall < numWalls; i_wall++)
        {
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    DeactivateWall(row, col, i_wall);
                    DeactivateWindow(row, col, i_wall);
                }
            }
        }

        // Activate walls (up) based on Maze Configuration
        foreach (Vector3 wall in wallsToBeRaised)
        {
            ActivateWall((int)wall.x, (int)wall.y, (int)wall.z);
            ActivateWindow((int)wall.x, (int)wall.y, (int)wall.z);

        }
    }

    void GenerateReward(PathConfig p_config) //READ FROM HERE
    {
        rewardCell = (ushort)(p_config.cellNum[p_config.cellNum.Length - 1] + 1); //Adjustment Factor
        reward = GameObject.CreatePrimitive(PrimitiveType.Cube);
        reward.name = "Reward";
        reward.GetComponent<MeshRenderer>().material.color = Color.red;
        reward.transform.localScale = scaleReward;
        positionReward = new Vector3(-(rewardCell - 1) % numCols * widthCell, heightReward / 2 + 500, (int)((rewardCell - 1) / numCols) * widthCell);
        rotationReward = Quaternion.Euler(0, 0, 0);
        reward.transform.SetPositionAndRotation(positionReward, rotationReward);
        redCone.transform.SetPositionAndRotation(positionReward + new Vector3(0.0f, -25.0f, 0f), rotationReward);
        redCone.GetComponent<MeshRenderer>().enabled = true;
        reward.AddComponent<CollisionReward>();
        reward.tag = "Reward";
        reward.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        reward.GetComponent<MeshRenderer>().enabled = false;
    }

    // void GeneratePopUp(PathConfig p_config)
    // {
    //     if (p_config.checkIncluded[p_config.cellNum.Length - 2] == 1)
    //     {
    //         popUpCell = (ushort)(p_config.cellNum[p_config.cellNum.Length - 2]+1); //Adjustment Factor
    //     }

    //     if (p_config.checkIncluded[p_config.cellNum.Length - 3] == 1)
    //     {
    //         popUpCell = (ushort)(p_config.cellNum[p_config.cellNum.Length - 3]+1); //Adjustment Factor
    //     }
    //     else
    //     {
    //         popUpCell = (ushort)(p_config.cellNum[p_config.cellNum.Length - 4]+1); //Adjustment Factor
    //     }

    //     popUp = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //     popUp.name = "popUp";
    //     popUp.GetComponent<MeshRenderer>().material.color = Color.red;
    //     popUp.transform.localScale = scalePopUp;
    //     positionPopUp = new Vector3(-(popUpCell - 1) % numCols * widthCell, heightReward / 2 + 500, (int)((popUpCell - 1) / numCols) * widthCell);
    //     rotationPopUp = Quaternion.Euler(0, 0, 0);
    //     popUp.transform.SetPositionAndRotation(positionPopUp, rotationPopUp);
    //     popUp.AddComponent<PopUpMessage>();
    //     popUp.tag = "PopUp";
    //     popUp.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    //     popUp.GetComponent<MeshRenderer>().enabled = false;
    // }

    void MoveReward() //READ FROM HERE
    {
        positionReward = new Vector3(-(rewardCell+1 - 1) % numCols * widthCell, heightReward / 2, (int)((rewardCell+1 - 1) / numCols) * widthCell); //Adjustment Factor
        reward.transform.SetPositionAndRotation(positionReward, rotationReward);
    }

    public void GenerateDoors() //READ FROM HERE
    {
        if (currentPathIdx != 0)
        {
            GenerateStartDoor(pathConfigs[currentPathIdx - 1], pathConfigs[currentPathIdx]);
        }

        if (currentPathIdx != (pathConfigs.Count - 1))
        {
            GenerateEndDoor(pathConfigs[currentPathIdx], pathConfigs[currentPathIdx + 1]);
        }
    }

    public int rowFromCellNum(ushort cellNum) //READ FROM HERE
    {
        int row = cellNum / 7;

        if (cellNum % 7 == 0)
        {
            row = cellNum / 7 - 1;
        }

        return row;
    }

    public int colFromCellNum(ushort cellNum) //READ FROM HERE
    {
        int col = cellNum % 7 - 1;
        if (cellNum % 7 == 0)
        {
            col = 6;
        }

        return col;
    }

    public void GenerateStartDoor(PathConfig c1, PathConfig c2) //READ FROM HERE
    {
        ushort cell1 = (ushort)(c1.cellNum[c1.cellNum.Length - 1]+1);//Adjustment Factor
        ushort cell2 = (ushort)(c2.cellNum[0]+1);//Adjustment Factor

        int wallNum = 0;
        int cellDiff = cell2 - cell1;

        if (cellDiff == -7)
        {
            wallNum = 7 - 1;
        }
        else if (cellDiff == 1)
        {
            wallNum = 1 - 1;
        }
        else if (cellDiff == 7)
        {
            wallNum = 3 - 1;
        }
        else if (cellDiff == -1)
        {
            wallNum = 5 - 1;
        }
        //Dectivate c2 wallNum
        int row = rowFromCellNum((ushort)(c2.cellNum[0]+1)); //Adjustment Factor

        int col = colFromCellNum((ushort)(c2.cellNum[0]+1)); //Adjustment Factor

        DeactivateWall(row, col, wallNum);
        DeactivateWindow(row, col, wallNum);

        //Move start door here

        startDoor.GetComponent<MeshRenderer>().enabled = true;

        if (cellDiff == 7)
        {
            startDoor.transform.position = wallPosition(row, col, wallNum) + new Vector3(60.0f, 0f, 0f);
        }

        else if (cellDiff == -7)
        {
            startDoor.transform.position = wallPosition(row, col, wallNum) + new Vector3(-60.0f, 0f, 0f);
        }

        else if (cellDiff == -1)
        {
            startDoor.transform.position = wallPosition(row, col, wallNum) + new Vector3(0f, 0f, -60.0f);
        }

        else if (cellDiff == 1)
        {
            startDoor.transform.position = wallPosition(row, col, wallNum) + new Vector3(0f, 0f, 60.0f);
        }

        startDoor.transform.rotation = wallRotation(row, col, wallNum);

        Debug.Log("The row, col and wallNum for startDoor is: " + row + ", " + col + ", " + wallNum);


    }
    public void GenerateEndDoor(PathConfig c1, PathConfig c2) //READ FROM HERE
    {
        ushort cell1 = (ushort)(c1.cellNum[c1.cellNum.Length - 1]+1); //Adjustment Factor
        ushort cell2 = (ushort)(c2.cellNum[0]+1); //Adjustment Factor

        int wallNum = 0;
        int cellDiff = cell2 - cell1;

        if (cellDiff == -7)
        {
            wallNum = 3 - 1;
        }
        else if (cellDiff == 1)
        {
            wallNum = 5 - 1;
        }
        else if (cellDiff == 7)
        {
            wallNum = 7 - 1;
        }
        else if (cellDiff == -1)
        {
            wallNum = 1 - 1;
        }

        //Dectivate c1 wallNum

        int row = rowFromCellNum((ushort)(c1.cellNum[c1.cellNum.Length - 1]+1)); //Adjustment Factor

        int col = colFromCellNum((ushort)(c1.cellNum[c1.cellNum.Length - 1]+1)); //Adjustment Factor
        //Move end door here

        DeactivateWall(row, col, wallNum); // correct
        DeactivateWindow(row, col, wallNum);  // correct 

        //Move start door here

        endDoor.GetComponent<MeshRenderer>().enabled = true;

        if (cellDiff == 7)
        {
            endDoor.transform.position = wallPosition(row, col, wallNum) + new Vector3(-60.0f, 0f, 0f);
        }

        else if (cellDiff == -7)
        {
            endDoor.transform.position = wallPosition(row, col, wallNum) + new Vector3(60.0f, 0f, 0f);
        }

        else if (cellDiff == -1)
        {
            endDoor.transform.position = wallPosition(row, col, wallNum) + new Vector3(0f, 0f, 60.0f);
        }

        else if (cellDiff == 1)
        {
            endDoor.transform.position = wallPosition(row, col, wallNum) + new Vector3(0f, 0f, -60.0f);
        }

        endDoor.transform.rotation = wallRotation(row, col, wallNum);

        Debug.Log("The row, col and wallNum for endDoor is: " + row + ", " + col + ", " + wallNum);

    }

    public void BinaryData()
    {
        PositionData data2 = new PositionData();

        data2.CurrentPosition = currentPosition;
        data2.CurrentRotation = currentRotation;

        using (FileStream fileStream = new FileStream(binaryDataPath, FileMode.Create))
        {
            byte[] bytes = MessagePackSerializer.Serialize(data2);
            fileStream.Write(bytes);
        }
    }

}

/*[MessagePackObject]
public class PositionData
{

    [Key(0)]
    public Vector3 position { get; set; }

    [Key(1)]
    public Quaternion rotation { get; set; }

    [SerializationConstructor]
    public PositionData(Vector3 currentPosition, Quaternion currentRotation)
    {
        this.position = currentPosition;

        this.rotation = currentRotation;
    }

}
*/

[MessagePackObject]
public class PositionData
{
    [IgnoreMember]
    private Vector3 currentPosition;

    [IgnoreMember]
    private Quaternion currentRotation;

    [Key(0)]
    public float[] PositionComponents
    {
        get
        {
            return new float[] { currentPosition.x, currentPosition.y, currentPosition.z };
        }
        set
        {
            if (value.Length >= 3)
            {
                currentPosition = new Vector3(value[0], value[1], value[2]);
            }
        }
    }

    [Key(1)]
    public float[] RotationComponents
    {
        get
        {
            return new float[] { currentRotation.x, currentRotation.y, currentRotation.z, currentRotation.w };
        }
        set
        {
            if (value.Length >= 4)
            {
                currentRotation = new Quaternion(value[0], value[1], value[2], value[3]);
            }
        }
    }

    [IgnoreMember]
    public Vector3 CurrentPosition
    {
        get { return currentPosition; }
        set { currentPosition = value; }
    }
    [IgnoreMember]
    public Quaternion CurrentRotation
    {
        get { return currentRotation; }
        set { currentRotation = value; }
    }
}

/*[Serializable]
public class ContinousData
{

    float[] positionComponents = new float[3];
    float[] rotationComponents = new float[4];


    public ContinousData(Vector3 currentPosition, Quaternion currentRotation)
    {

        positionComponents[0] = currentPosition.x;
        positionComponents[1] = currentPosition.y;
        positionComponents[2] = currentPosition.z;

        rotationComponents[0] = currentRotation.x;
        rotationComponents[1] = currentRotation.y;
        rotationComponents[2] = currentRotation.z;
        rotationComponents[3] = currentRotation.w;

    }

}*/





/*



if (rewardState == 10)
{

    Move.verticalSpeed = 0.0f;

    //textReward.GetComponent<TextMeshProUGUI>().text = "Turn towards the actual lighthouse and press the trigger";

    if (Input.anyKeyDown)
    {
        rewardState = 100;

        Vector3 lightHouseLocation = new Vector3(lightHouse.transform.position.x, lightHouse.transform.position.y, lightHouse.transform.position.z);
        Vector3 realLightHouseLocation = (lightHouseLocation - playerPosition);
        angleToLH = Vector3.Angle(realLightHouseLocation, player.transform.rotation * Vector3.forward);

    }

}

if (rewardState == 100)
{

    if (angleToLH > 10)
    {

        popUpMessage.GetComponent<TextMeshProUGUI>().text = "Try Again";
        popUpMessage.enabled = true;
        Invoke("Delay", 2);
        popUpMessage.enabled = false;
        rewardState = 10;
    }
    else
    {
        rewardState = 11;
    }
}

if (rewardState == 11)
{
    //textReward.GetComponent<TextMeshProUGUI>().text = "Turn towards the actual Cabin and press the trigger";

    if (Input.anyKeyDown)
    {
        rewardState = 101;

        Vector3 cabinLocation = new Vector3(house.transform.position.x, house.transform.position.y, house.transform.position.z);
        Vector3 realCabinLocation = (cabinLocation - playerPosition);
        angleToCabin = Vector3.Angle(realCabinLocation, player.transform.rotation * Vector3.forward);
    }

}

if (rewardState == 101)
{

    if (angleToCabin > 10)
    {

        popUpMessage.GetComponent<TextMeshProUGUI>().text = "Try Again";
        popUpMessage.enabled = true;
        Invoke("Delay", 2);
        popUpMessage.enabled = false;
        rewardState = 11;
    }
    else
    {

        Move.verticalSpeed = 150.0f;
        rewardState = 15;
        selectedLandmarks = true;
    }
}*/