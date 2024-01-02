using Lightbug.CharacterControllerPro.Demo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Text;
using TMPro;
using UnityEngine; 
using UnityEngine.UI;
using System.IO;

public class CollisionReward : MonoBehaviour
{
    public static readonly string uniqueIdentifier2 = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mmss");

    public static string readableDataPath;

    //public static readonly string readableDataPath = "C:\\Users\\amrsa\\Desktop\\GameData\\" + uniqueIdentifier2 + "_readabledata.txt";

    public static bool selectLandmarks;

    public static bool selectLH;

    public static bool selectCabin;

    public static bool moveOn;

    public float angleToLH;
    public float angleToCabin;

    public static float previousAverage;
    public static float averageCurrentPathScore;
    public static bool saveNow = false;
    public static int rewardState;
    public static GameObject lighthouseIsland;
    private float shiftMark = 50f;
    private float shiftMarky = 30f;
    private Vector3 markPosition;
    private Quaternion markRotation;
    private Vector3 playerPosition;
    private Vector3 playerPositionShift;
    private Vector3 playerPositionFinal;
    private Vector3 startArrowDirection;
    private Vector3 markOnePosition;
    private Quaternion markOneRotation;
    private Vector3 markTwoPosition;
    private Quaternion markTwoRotation;
    private Vector3 startPosition;
    private Vector3 startArrowPosition;
    private Quaternion startArrowRotation;
    private Vector3 lightHousePosition;
    private Vector3 realLightHouseDirection;
    private Vector3 userLightHouseRotationVector;
    private Vector3 castlePosition;
    private Vector3 housePosition;
    private Vector3 realCastleDirection;
    private Vector3 realHouseDirection;
    private Vector3 userCastleRotationVector;
    private Vector3 lighthouseIconVector;
    private Vector3 houseIconVector;
    private Vector3 startIconVector;
    private Vector3 houseRewardPosition;
    private Quaternion lighthouseIconRotation;
    private Quaternion houseRewardRotation;
    private Quaternion startIconRotation;
    public static float score_start, score1, score2, angleAccuracy, angleOffset;

    public static int enableAllocentric = 0;

    float moveInY = 0;

    int pathIteration = 0;
    int numCols = 7;
    float widthCell = 300f;
    Quaternion alpha;
    private ReadMazeConfig mazeConfig;
    private int startCell;

    public GameObject reward, textReward, textAllocentric, textScore, arrow, startArrow, mark, markOne, markTwo, player, mageTower, house, commonHouseOne, turbine, lightHouse, bgTextReward, bgTextAllocentric, bgTextStart, flagReward, lighthouseReward, houseReward, textStart;

    private GameObject startDoor, endDoor, cabinIsland, redCone;

    public GameObject cabinIcon, lhouseIcon, mountain1, mountain2, mountain3, mountain4, mountain5, mountain6, mountain7, mountain;

    public GameObject[] trees;

    public List<EventData> eventList = new List<EventData>();

    State state1;
    State state2;

    enum State
    {
        ExperimentStart,
        LocateLighthouse,
        LocateCabin,
        FoundLighthouse,
        FoundCabin,
        NotFoundLighthouse,
        NotFoundCabin,
        GoingThroughDoor,
        MovingToCone,
        TestStartLocation,
        TestLighthouse,
        TestCabin,
    }

    void Start()
    {

        readableDataPath = Path.Combine(Application.persistentDataPath, uniqueIdentifier2 + "_EventData.json");

        Debug.Log("The path for 'readableDataPath' is: ");
        Debug.Log(readableDataPath);

        trees = GameObject.FindGameObjectsWithTag("Tree");
        mountain1 = GameObject.FindWithTag("MountainOne");
        mountain2 = GameObject.FindWithTag("MountainTwo");
        mountain3 = GameObject.FindWithTag("MountainThree");
        mountain4 = GameObject.FindWithTag("MountainFour");
        mountain5 = GameObject.FindWithTag("Mountain5");
        mountain6 = GameObject.FindWithTag("Mountain6");
        mountain7 = GameObject.FindWithTag("Mountain7");
        mountain = GameObject.FindWithTag("Mountain");

        lighthouseIsland = GameObject.FindWithTag("LighthouseIsland");
        cabinIcon = GameObject.FindWithTag("CabinIcon");
        lhouseIcon = GameObject.FindWithTag("LhouseIcon");
        cabinIcon.GetComponent<MeshRenderer>().enabled = false;
        lhouseIcon.GetComponent<MeshRenderer>().enabled = false;
        cabinIsland = GameObject.FindWithTag("CabinIsland");

        redCone = GameObject.FindWithTag("RedCone");
        endDoor = GameObject.FindWithTag("EndDoor");
        startDoor = GameObject.FindWithTag("StartDoor");
        rewardState = 10;
        mageTower = GameObject.FindWithTag("MageTower");


        house = GameObject.FindWithTag("House");
        commonHouseOne = GameObject.FindWithTag("CommonHouseOne");
        turbine = GameObject.FindWithTag("Turbine");
        lightHouse = GameObject.FindWithTag("LightHouse");
        player = GameObject.FindWithTag("Player");

        bgTextReward = GameObject.FindWithTag("BGText_Reward");
        bgTextReward.GetComponent<Image>().enabled = false;
        textReward = GameObject.FindWithTag("Text_Reward");
        textReward.GetComponent<TextMeshProUGUI>().enabled = false;
        textStart = GameObject.FindWithTag("Text_Start");
        textStart.GetComponent<TextMeshProUGUI>().enabled = true;
        textScore = GameObject.FindWithTag("Text_Score");

        bgTextAllocentric = GameObject.FindWithTag("BGTextAllocentric");
        bgTextAllocentric.GetComponent<Image>().enabled = false;
        bgTextStart = GameObject.FindWithTag("BGText_Start");
        textAllocentric = GameObject.FindWithTag("Text_Allocentric");
        textAllocentric.GetComponent<TextMeshProUGUI>().enabled = false;

        arrow = GameObject.FindWithTag("Arrow");
        arrow.GetComponent<MeshRenderer>().enabled = false;
        startArrow = GameObject.FindWithTag("StartArrow");
        startArrow.GetComponent<MeshRenderer>().enabled = false;
        mazeConfig = FindObjectOfType<ReadMazeConfig>();

        startCell = ReadMazeConfig.pathConfigs[ReadMazeConfig.currentPathIdx].cellNum[0];
        startPosition = new Vector3(-(startCell - 1) % numCols * widthCell, player.transform.position.y + shiftMarky, (int)((startCell - 1) / numCols) * widthCell);
        reward = GameObject.FindWithTag("Reward");
        flagReward = GameObject.FindWithTag("flagReward");
        houseReward = GameObject.FindWithTag("houseReward");
        lighthouseReward = GameObject.FindWithTag("lighthouseReward");
        flagReward.GetComponent<MeshRenderer>().enabled = false;
        houseReward.GetComponent<MeshRenderer>().enabled = false;
        lighthouseReward.GetComponent<MeshRenderer>().enabled = false;


        alpha = Quaternion.Euler(0, 0, 0);

        score_start = 0;
        score1 = 0;
        score2 = 0;

        selectLandmarks = false;

        selectLH = false;

        selectCabin = false;

        moveOn = false;

        State state = State.LocateLighthouse;

        // Start JSON file
        using (StreamWriter sw = File.AppendText(readableDataPath))
        {
            sw.WriteLine("[");
        }
    }

    void onDestroy()
    {
        using (StreamWriter sw = File.AppendText(readableDataPath))
        {
            sw.WriteLine("]");
        }
    }

    void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.tag == "Player" && rewardState == 10)
        {
            rewardState = 1;
            Move.verticalSpeed = 0.0f;
            startDoor.GetComponent<MeshRenderer>().enabled = false;
        }

    }

    void Update()
    {

        playerPosition = new Vector3(player.transform.position.x, player.transform.position.y + shiftMarky + 10, player.transform.position.z);
        playerPositionShift = new Vector3(player.transform.position.x - 20, player.transform.position.y + shiftMarky + 10, player.transform.position.z - 30);
        playerPositionFinal = player.transform.rotation * Vector3.forward + playerPositionShift;

        if (rewardState == 1)
        {

            for (int i_wall = 0; i_wall < ReadMazeConfig.numWalls; i_wall++)
            {
                for (int row = 0; row < ReadMazeConfig.numRows; row++)
                {
                    for (int col = 0; col < ReadMazeConfig.numCols; col++)
                    {
                        ReadMazeConfig.DeactivateWall(row, col, i_wall);
                        ReadMazeConfig.DeactivateWindow(row, col, i_wall);
                    }
                }
            }

            GameObject[] roofs1 = GameObject.FindGameObjectsWithTag("RoofPath");

            foreach (GameObject roof in roofs1)
            {
                roof.GetComponent<MeshRenderer>().enabled = false;
            }


            foreach (GameObject tree in trees)
            {
                tree.SetActive(false);
                /*
                tree.GetComponent<MeshRenderer>().enabled = false;*/
            }

            redCone.GetComponent<MeshRenderer>().enabled = false;
            enableAllocentric = 1;
            /*house.SetActive(false);
            lightHouse.SetActive(false);*/

            lightHouse.GetComponent<MeshRenderer>().enabled = false;
            house.GetComponent<MeshRenderer>().enabled = false;

            turbine.SetActive(false);
            lighthouseIsland.GetComponent<MeshRenderer>().enabled = false;
            cabinIsland.GetComponent<MeshRenderer>().enabled = false;

            bgTextReward.GetComponent<Image>().enabled = true;
            textReward.GetComponent<TextMeshProUGUI>().enabled = true;
            arrow.GetComponent<MeshRenderer>().enabled = true;

            endDoor.GetComponent<MeshRenderer>().enabled = false;
            textReward.GetComponent<TextMeshProUGUI>().text = "Please point to the start location!";
            //bgTextAllocentric.GetComponent<Image>().enabled = true;
            //textAllocentric.GetComponent<TextMeshProUGUI>().enabled = true;
            bgTextStart.GetComponent<Image>().enabled = false;
            textStart.GetComponent<TextMeshProUGUI>().enabled = false;

            mountain1.SetActive(false);
            mountain2.SetActive(false);
            mountain3.SetActive(false);
            mountain4.SetActive(false);
            mountain5.SetActive(false);
            mountain6.SetActive(false);
            mountain7.SetActive(false);
            mountain.SetActive(false);

            rewardState = 2;    // INITIAL PROMPT IS DISPLAYED

        }
        else if (rewardState == 2)
        {
            if (Input.anyKeyDown)
            {
                textReward.GetComponent<TextMeshProUGUI>().text = "Please press the trigger to see where you actually started!";
                rewardState = 3; // START LOCATION PICKED
            }
        }
        else if (rewardState == 3)
        {
            if (Input.anyKeyDown)
            {

                startPosition = new Vector3(-(startCell - 1) % numCols * widthCell, player.transform.position.y + shiftMarky, (int)((startCell - 1) / numCols) * widthCell);
                Vector3 startArrowDirection = (startPosition - playerPosition);
                startArrowDirection.y = 0;
                startArrowDirection.Normalize();
                float startAngularError = Vector3.Angle(startArrowDirection, player.transform.rotation * Vector3.forward); 

                Quaternion targetRotation = Quaternion.LookRotation(startArrowDirection); 

                Quaternion offsetRotation = Quaternion.Euler(0f, 90f, 0f);
                targetRotation *= Quaternion.Euler(90, 0, 0) * offsetRotation;

                startArrow.transform.rotation = targetRotation;

                startArrow.transform.position = playerPosition;
                startArrow.GetComponent<MeshRenderer>().enabled = true;

                Debug.Log("Start error: " + startAngularError + " degrees");
                score_start += Mathf.Round(100 - (startAngularError / 180 * 100));
                textReward.GetComponent<TextMeshProUGUI>().text = "This purple arrow points to where you started!\nNow please point to the Lighthouse!";
                rewardState = 4;

                EventData eventData = new EventData();
                eventData.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                eventData.pathNumber = ReadMazeConfig.currentPathIdx;
                eventData.startScore = score_start;
                eventData.scoreLH = float.NaN;
                eventData.scoreC = float.NaN;
                eventData.allocentricAccuracyPercent = float.NaN;
                eventData.position = player.transform.position;
                eventData.rotation = player.transform.rotation;
                eventData.lighthousePosition = lightHouse.transform.position;
                eventData.cabinPosition = house.transform.position;
                eventData.currentState = "selectedStartPosition";
                eventData.startPosition = startPosition;

                string eventDataJson = JsonUtility.ToJson(eventData, true) + ",";

                using (StreamWriter sw = File.AppendText(readableDataPath))
                {
                    sw.WriteLine(eventDataJson);
                }


            }
        }

        else if (rewardState == 4)
        {
            // if (Input.GetKeyDown(KeyCode.R)) {
            if (Input.anyKeyDown)
            {
                // Compute lighthouse angular error
                lightHousePosition = new Vector3(lightHouse.transform.position.x, lightHouse.transform.position.y, lightHouse.transform.position.z);
                Vector3 realLightHouseDirection = (lightHousePosition - playerPosition);
                realLightHouseDirection.y = 0;
                realLightHouseDirection.Normalize();

                float lightHouseAngularError = Vector3.Angle(realLightHouseDirection, player.transform.rotation * Vector3.forward);

                float radius = 500f;

                Quaternion playerRotation = player.transform.rotation; // Get the player's rotation
                Vector3 forwardDirection = player.transform.forward; // Get the player's forward direction

                Vector3 displacement = forwardDirection * radius; // Calculate the displacement vector

                Vector3 targetPosition = player.transform.position + displacement; // Calculate the target position

                startArrow.GetComponent<MeshRenderer>().enabled = false;
                lhouseIcon.transform.position = targetPosition;
                lhouseIcon.GetComponent<MeshRenderer>().enabled = true;

                Debug.Log("Lighhouse error: " + lightHouseAngularError + " degrees");
                score1 += Mathf.Round(100 - (lightHouseAngularError / 180 * 100));
                textReward.GetComponent<TextMeshProUGUI>().text = "Now please point to the Cabin!";

                EventData eventData = new EventData();
                eventData.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                eventData.pathNumber = ReadMazeConfig.currentPathIdx;
                eventData.startScore = score_start;
                eventData.scoreLH = score1;
                eventData.scoreC = float.NaN;
                eventData.allocentricAccuracyPercent = float.NaN;
                eventData.position = player.transform.position;
                eventData.rotation = player.transform.rotation;
                eventData.lighthousePosition = lightHouse.transform.position;
                eventData.cabinPosition = house.transform.position; 
                eventData.currentState = "selectedLightHouse";
                eventData.startPosition = startPosition; 

                string eventDataJson = JsonUtility.ToJson(eventData, true) + ",";

                using (StreamWriter sw = File.AppendText(readableDataPath))
                {
                    sw.WriteLine(eventDataJson);
                }


                rewardState = 5;
            }
        }


        else if (rewardState == 5)
        {
            if (Input.anyKeyDown)
            {

                float radius = 400f;

                Quaternion playerRotation = player.transform.rotation; // Get the player's rotation
                Vector3 forwardDirection = player.transform.forward; // Get the player's forward direction

                Vector3 displacement = forwardDirection * radius; // Calculate the displacement vector

                Vector3 targetPosition = player.transform.position + displacement; // Calculate the target position

                cabinIcon.transform.position = targetPosition;
                cabinIcon.GetComponent<MeshRenderer>().enabled = true;

                Invoke("Delay", 1);

                housePosition = new Vector3(house.transform.position.x, house.transform.position.y, house.transform.position.z);
                Vector3 realHouseDirection = (housePosition - playerPosition /*houseIconVector*/);
                realHouseDirection.Normalize();
                float houseAngularError = Vector3.Angle(realHouseDirection, player.transform.rotation * Vector3.forward);

                Vector3 playerToLightHouse = (lightHousePosition - playerPosition);

                float LhouseCabinAngleActual = Vector3.Angle(playerToLightHouse, realHouseDirection); //Accurate 

                Debug.Log("ActualAngle: " + LhouseCabinAngleActual);

                Vector3 cabinIconVector = (cabinIcon.transform.position - playerPosition);
                Vector3 lhouseIconVector = (lhouseIcon.transform.position - playerPosition);


                float LhouseCabinAngleEstimate = Vector3.Angle(cabinIconVector, lhouseIconVector);

                angleOffset = Math.Abs(LhouseCabinAngleActual - LhouseCabinAngleEstimate);

                Debug.Log("Angle Estimate: " + LhouseCabinAngleEstimate);

                angleAccuracy = Mathf.Round(100 - angleOffset / (LhouseCabinAngleActual) * 100);


                Debug.Log("House error: " + houseAngularError + " degrees");
                score2 += Mathf.Round(100 - (houseAngularError / 180 * 100));

                textReward.GetComponent<TextMeshProUGUI>().text = "Now Look towards the LightHouse and press the trigger "; 

                averageCurrentPathScore = (score_start + score1 + score2) / 3;

                Debug.Log("This did work indeed");

                EventData eventData = new EventData();
                eventData.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); 
                eventData.pathNumber = ReadMazeConfig.currentPathIdx;
                eventData.startScore = score_start;
                eventData.scoreLH = score1;
                eventData.scoreC = score2;
                eventData.allocentricAccuracyPercent = angleAccuracy; 
                eventData.position = player.transform.position;
                eventData.rotation = player.transform.rotation;
                eventData.lighthousePosition = lightHouse.transform.position;
                eventData.cabinPosition = house.transform.position; 
                eventData.currentState = "SelectedCabin";
                eventData.startPosition = startPosition; 

                string eventDataJson = JsonUtility.ToJson(eventData, true) + ",";

                using (StreamWriter sw = File.AppendText(readableDataPath))
                {
                    sw.WriteLine(eventDataJson);
                }

            }
        }

        else if (rewardState == 6)
        {

            
            if (Input.anyKeyDown)
            {

                Vector3 lightHouseLocation = new Vector3(lightHouse.transform.position.x, lightHouse.transform.position.y, lightHouse.transform.position.z);
                Vector3 realLightHouseLocation = (lightHouseLocation - playerPosition);
                angleToLH = Vector3.Angle(realLightHouseLocation, player.transform.rotation * Vector3.forward);

                if(angleToLH <= 15)
                {
                    state1 = State.FoundLighthouse;

                    EventData eventData = new EventData();
                    eventData.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    eventData.pathNumber = ReadMazeConfig.currentPathIdx;
                    eventData.startScore = score_start;
                    eventData.scoreLH = score1;
                    eventData.scoreC = float.NaN;
                    eventData.allocentricAccuracyPercent = float.NaN;
                    eventData.position = player.transform.position;
                    eventData.rotation = player.transform.rotation;
                    eventData.lighthousePosition = lightHouse.transform.position;
                    eventData.cabinPosition = house.transform.position; 
                    eventData.currentState = "LighthouseFound";
                    eventData.startPosition = startPosition; 

                    string eventDataJson = JsonUtility.ToJson(eventData, true) + ",";

                    using (StreamWriter sw = File.AppendText(readableDataPath))
                    {
                        sw.WriteLine(eventDataJson);
                    }



                    rewardState = 7;
                    textReward.GetComponent<TextMeshProUGUI>().text = "Look to the Cabin and press the Trigger";

                }
                else
                {
                    state1 = State.NotFoundLighthouse;

                    EventData eventData = new EventData();
                    eventData.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    eventData.pathNumber = ReadMazeConfig.currentPathIdx;
                    eventData.startScore = score_start;
                    eventData.scoreLH = score1;
                    eventData.scoreC = float.NaN;
                    eventData.allocentricAccuracyPercent = float.NaN;
                    eventData.position = player.transform.position;
                    eventData.rotation = player.transform.rotation;
                    eventData.lighthousePosition = lightHouse.transform.position;
                    eventData.cabinPosition = house.transform.position; 
                    eventData.currentState = "LightHouseNotFound";
                    eventData.startPosition = startPosition; 

                    string eventDataJson = JsonUtility.ToJson(eventData, true) + ",";

                    using (StreamWriter sw = File.AppendText(readableDataPath))
                    {
                        sw.WriteLine(eventDataJson);
                    }
                    rewardState = 7;
                    textReward.GetComponent<TextMeshProUGUI>().text = "Press The Trigger and Try Again!";
                }

            }

        } 
        

        else if (rewardState == 7)
        {
            if(state1 == State.NotFoundLighthouse)
            {
                rewardState = 6; 
            }

            else if (Input.anyKeyDown)
            {
                Vector3 cabinLocation = new Vector3(house.transform.position.x, house.transform.position.y, house.transform.position.z);
                Vector3 realCabinLocation = (cabinLocation - playerPosition);
                angleToCabin = Vector3.Angle(realCabinLocation, player.transform.rotation * Vector3.forward);

                if (angleToCabin <= 15)
                {
                    state2 = State.FoundCabin;
                    rewardState = 8;

                    EventData eventData = new EventData();
                    eventData.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    eventData.pathNumber = ReadMazeConfig.currentPathIdx;
                    eventData.startScore = score_start;
                    eventData.scoreLH = score1;
                    eventData.scoreC = float.NaN;
                    eventData.allocentricAccuracyPercent = float.NaN;
                    eventData.position = player.transform.position;
                    eventData.rotation = player.transform.rotation;
                    eventData.lighthousePosition = lightHouse.transform.position;
                    eventData.cabinPosition = house.transform.position; 
                    eventData.currentState = "CabinFound";
                    eventData.startPosition = startPosition; 

                    string eventDataJson = JsonUtility.ToJson(eventData, true) + ",";

                    using (StreamWriter sw = File.AppendText(readableDataPath))
                    {
                        sw.WriteLine(eventDataJson);
                    }

                }

                else
                {

                    EventData eventData = new EventData();
                    eventData.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    eventData.pathNumber = ReadMazeConfig.currentPathIdx;
                    eventData.startScore = score_start;
                    eventData.scoreLH = score1;
                    eventData.scoreC = float.NaN;
                    eventData.allocentricAccuracyPercent = float.NaN;
                    eventData.position = player.transform.position;
                    eventData.rotation = player.transform.rotation;
                    eventData.lighthousePosition = lightHouse.transform.position;
                    eventData.cabinPosition = house.transform.position; 
                    eventData.currentState = "CabinNotFound";
                    eventData.startPosition = startPosition; 

                    string eventDataJson = JsonUtility.ToJson(eventData, true) + ",";

                    using (StreamWriter sw = File.AppendText(readableDataPath))
                    {
                        sw.WriteLine(eventDataJson);
                    }

                    state2 = State.NotFoundCabin;
                    rewardState = 8;
                  
                }

            }

        }

        else if(rewardState == 8)
        {
            if (state2 == State.NotFoundCabin)
            {
                textReward.GetComponent<TextMeshProUGUI>().text = "Press The Trigger and Try Again!";
                rewardState = 7;
            }

            else
            {
                textStart.GetComponent<TextMeshProUGUI>().text = "Walk through the door";

                selectLandmarks = true;
                selectCabin = true;

                reward.SetActive(false);

                enableAllocentric = 2;

                turbine.SetActive(true);
                rewardState = 8;
                Move.verticalSpeed = 150.0f;

                textReward.GetComponent<TextMeshProUGUI>().enabled = false;
                bgTextReward.GetComponent<Image>().enabled = false;
                bgTextStart.GetComponent<Image>().enabled = true;

                previousAverage = averageCurrentPathScore;

                score_start = 0;
                score1 = 0;
                score2 = 0;
                angleAccuracy = 0;
                angleOffset = 0;

                cabinIcon.GetComponent<MeshRenderer>().enabled = false;
                lhouseIcon.GetComponent<MeshRenderer>().enabled = false;
                startDoor.GetComponent<MeshRenderer>().enabled = false;
                endDoor.GetComponent<MeshRenderer>().enabled = true;
                startDoor.GetComponent<BoxCollider>().isTrigger = true;

                selectLH = false;

                EventData eventData = new EventData();
                eventData.timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                eventData.pathNumber = ReadMazeConfig.currentPathIdx;
                eventData.startScore = score_start;
                eventData.scoreLH = score1;
                eventData.scoreC = float.NaN;
                eventData.allocentricAccuracyPercent = float.NaN;
                eventData.position = player.transform.position;
                eventData.rotation = player.transform.rotation;
                eventData.lighthousePosition = lightHouse.transform.position;
                eventData.cabinPosition = house.transform.position; 
                eventData.currentState = "WalkingThroughDoor";
                eventData.startPosition = startPosition; 

                string eventDataJson = JsonUtility.ToJson(eventData, true) + ",";

                using (StreamWriter sw = File.AppendText(readableDataPath))
                {
                    sw.WriteLine(eventDataJson);
                }
            }

        }

    }

    private void Delay()
    {
        lightHouse.GetComponent<MeshRenderer>().enabled = true;

        house.GetComponent<MeshRenderer>().enabled = true;

        lighthouseIsland.GetComponent<MeshRenderer>().enabled = true;

        cabinIsland.GetComponent<MeshRenderer>().enabled = true;

        saveNow = false;

        mountain1.SetActive(true);
        mountain2.SetActive(true);
        mountain3.SetActive(true);
        mountain4.SetActive(true);
        mountain5.SetActive(true);
        mountain6.SetActive(true);
        mountain7.SetActive(true);
        mountain.SetActive(true);

        foreach (GameObject tree in trees)
        {
            tree.SetActive(true);

        }

        lhouseIcon.GetComponent<MeshRenderer>().enabled = false;
        cabinIcon.GetComponent<MeshRenderer>().enabled = false;

        rewardState = 6;
    }

    public static void ReadableData(float score)
    {

        using (StreamWriter writer = new StreamWriter(readableDataPath, true))
        {
            writer.WriteLine(score);
        }

    }

    public static void ReadableData(string text)
    {

        using (StreamWriter writer = new StreamWriter(readableDataPath, true))
        {
            writer.WriteLine(text);
        }

    }

}


[System.Serializable]
public class EventData
{
    public string timestamp;
    public int pathNumber;
    public float startScore;
    public float scoreLH;
    public float scoreC; 
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 lighthousePosition;
    public Vector3 cabinPosition;
    public Vector3 startPosition;
    public float allocentricAccuracyPercent;
    public string currentState; 
}

