using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using System;
using System.Text;
using TMPro;

public class Door1Trigger : MonoBehaviour
{
    private GameObject endDoor;
    private GameObject player;
    private GameObject startDoor;
    private GameObject bgTextStart;

    public static GameObject cabin;
    public static GameObject lightHouse;
    public static GameObject bgTextScore;

    public void OnTriggerEnter(Collider enter)
    {
        endDoor = GameObject.FindWithTag("EndDoor");

        player = GameObject.FindWithTag("Player"); 

        if (enter.CompareTag("Player"))
        {
            ReadMazeConfig.currentPathIdx += 1;
        }
    }

    public void OnTriggerExit(Collider enter)
    {
        endDoor = GameObject.FindWithTag("EndDoor");
        cabin = GameObject.FindWithTag("House");
        lightHouse = GameObject.FindWithTag("LightHouse");
        bgTextScore = GameObject.FindWithTag("BGText_Score");
        startDoor = GameObject.FindWithTag("StartDoor");

        if (enter.CompareTag("Player"))
        {
            startDoor.GetComponent<MeshRenderer>().enabled = true;
            endDoor.GetComponent<BoxCollider>().isTrigger = false;
            cabin.GetComponent<MeshRenderer>().enabled = false;
            lightHouse.GetComponent<MeshRenderer>().enabled = false;
            bgTextScore.SetActive(false);

        }
    }

}


