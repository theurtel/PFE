using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(ARRaycastManager))]
public class SpawnableManager : MonoBehaviour
{
    private TextMeshProUGUI txtScore;
    private TextMeshProUGUI txtGold;
    private TextMeshProUGUI txtStreak;

    public CSVHandler readCSV;
    public DataHandler dataHandler;

    private int streak=0;
    private string currentDate = DateTime.Now.ToShortDateString();

    [SerializeField]
    private GameObject camera;

    private Vector3 origCamPos;
    private Vector3 prevCamPos;
    private float prevDist;
    private float origDist;

    private Vector3 spawnPos;
    private Quaternion spawnRot;
    private Vector3 origObjPos;
    private Vector3 validPos;

    private Vector3 axis;

    private bool eat = false;
    private bool anim = false;

    private ARRaycastManager m_RaycastManager;
    private GameObject spawnedObject;
    private bool isSpawned = false;

    private Vector2 touchPosition;

    [SerializeField]
    private GameObject button; 

    private GameObject spawnablePrefab;
    private int prefabID=0;

    private List<GameObject> prefabs = new List<GameObject>();
    private int nbPrefabs = 0;

    public ButtonHandler buttonHandler;
    private bool buttonPressed;

    private List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();

    private bool isAppearingRight = false;
    private bool isAppearingWrong = false;
     
    [SerializeField]
    private Texture2D imageRight;
    [SerializeField]
    private Texture2D imageWrong;
    
    private float imageStayTime = 2.0f;
    private bool _swipeRight;
    private string name;
    

    private void Awake()
    {
        dataHandler.loadData();

        string skinPath = ("images/skins/" + dataHandler.getSkin());
        buttonHandler.displaySkin(skinPath);

        txtScore = GameObject.Find("/Canvas/score").GetComponent<TextMeshProUGUI>();
        txtGold = GameObject.Find("/Canvas/gold").GetComponent<TextMeshProUGUI>();
        txtStreak = GameObject.Find("/Canvas/streak").GetComponent<TextMeshProUGUI>();

        txtScore.text = dataHandler.getScore().ToString();
        txtGold.text = dataHandler.getGold().ToString();
        txtStreak.text = streak.ToString();

        buttonPressed = false;

        readCSV.read("list");
        nbPrefabs = readCSV.getSize();

        string path;
        for(int i=0 ; i<nbPrefabs ; i++){
            path = "prefabs/" + readCSV.getName(i);
            prefabs.Add(Resources.Load<GameObject>(path));
        }

        m_RaycastManager = GetComponent<ARRaycastManager>();
    }



    private void Update()
    {
        //:COMMENT:20/03/2022:HEURTEL: if the animation has to be enabled, brings the object closer to or farther from the camera depending on if it is getting eaten or not
        if(anim && eat) 
        {
            if(spawnedObject.transform.position != validPos - axis*origDist) 
            {
                spawnedObject.transform.position -= axis * origDist/15.0f;
            }
            else
            {
                Destroy(spawnedObject);
                anim = false;
            }
            return;
        }
        
        if(anim)
        {
            if(spawnedObject.transform.position != validPos + axis*origDist) 
            {
                spawnedObject.transform.position += axis * origDist/15.0f;
            }
            else
            {
                Destroy(spawnedObject);
                anim = false;
            }
            return;
        }


        //COMMENT:17/03/2022:HEURTEL: if the button is pressed and an object is already placed
        if(buttonHandler.isPressed() && isSpawned)
        {   
            //COMMENT:17/03/2022:HEURTEL: if it is the first frame where the button is pressed, stores the original position of the camera, the distance and the axis between the object and the camera
            if(!buttonPressed)
            {
                origCamPos = camera.transform.position;
                prevCamPos = origCamPos;
                origDist = Vector3.Distance(spawnPos, origCamPos);
                axis = Vector3.Normalize(spawnPos - origCamPos);
                buttonPressed = true;
            }            

            eat = false ;

            //COMMENT:18/03/2022:HEURTEL: we're looking to maintain the original distance between the camera and the object : if the current distance is shorter, we're getting closer to the object so we're going to move it farther along the axis and vice versa
            if(origDist>Vector3.Distance(spawnedObject.transform.position, camera.transform.position)) 
            {
                spawnedObject.transform.position += axis * Vector3.Distance(prevCamPos, camera.transform.position);
            }
            else if(origDist<Vector3.Distance(spawnedObject.transform.position, camera.transform.position)) 
            {
                eat = true;
                spawnedObject.transform.position -= axis * Vector3.Distance(prevCamPos, camera.transform.position);
            }

            prevCamPos = camera.transform.position;

            //COMMENT:19/03/2022:HEURTEL: if the object reached a certain distance, we anable the validation coroutine and set up the movement of the object (cf beginning of Update) according to the current camera and object positions
            if(Vector3.Distance(spawnedObject.transform.position, spawnPos) > Vector3.Distance(spawnPos, origCamPos)/1.5f)
            {
                anim = true;
                validPos = spawnedObject.transform.position;
                origDist = Vector3.Distance(validPos, camera.transform.position);
                axis = Vector3.Normalize(validPos - camera.transform.position);
                isSpawned=false;
                StartCoroutine("displayRightOrWrong");
                dataHandler.saveScore();
            }
        }

        //COMMENT:17/03/2022:HEURTEL: if the user releases the button, the object goes back to his original position
        if(!buttonHandler.isPressed() && isSpawned && buttonPressed)        
        {
            spawnedObject.transform.position = spawnPos;
            buttonPressed = false;
        }

        if(!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        //:COMMENT:17/03/2022:HEURTEL: if the user touches the bottom of the screen, returns to avoid any conflict with the button
        if(touchPosition.y<button.transform.position.y*2)
            return;

        //:COMMENT:16/03/2022:PIET: if there is no displayed object, instantiates it on the touch position, else moves the existing object
        if(m_RaycastManager.Raycast(touchPosition, m_Hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPosition = m_Hits[0].pose;

            if(!isSpawned)
            {
                prefabID = UnityEngine.Random.Range(0, nbPrefabs);
                spawnablePrefab = prefabs[prefabID];
                spawnedObject = Instantiate(spawnablePrefab, hitPosition.position, hitPosition.rotation);
                isSpawned=true;
            }
            else
            {
                spawnedObject.transform.position = hitPosition.position;
                spawnedObject.transform.rotation = hitPosition.rotation;
            }
            spawnPos = spawnedObject.transform.position;
            spawnRot = spawnedObject.transform.rotation;
        }
    }


    
    //:COMMENT:16/03/2022:PIET: loads the touch position - returns false si the screen is not being touched, true else
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if(Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }


    //:COMMENT:16/03/2022:HEURTEL: if the user makes the good or bad choice, changes the boolean to activate the display of the image in OnGui during the time defined by imageStayTime
    public IEnumerator displayRightOrWrong()
    {
        if((readCSV.isHealthy(prefabID) && eat) || (!readCSV.isHealthy(prefabID) && !eat))
        {
            isAppearingRight = true;
            dataHandler.winScore();
            dataHandler.addGold(10);
            dataHandler.saveGold();
            streak += 1;
            if(streak == 10 && dataHandler.isNewDay(currentDate))
            {
                dataHandler.saveDate(currentDate);
                dataHandler.addGold(1000);
                dataHandler.saveGold();
            }
            txtScore.text = dataHandler.getScore().ToString();
            txtGold.text = dataHandler.getGold().ToString();
            txtStreak.text = streak.ToString();
            yield return new WaitForSeconds(imageStayTime); 
            isAppearingRight = false;
        }
        else
        {
            isAppearingWrong = true;
            dataHandler.loseScore();
            streak = 0;
            txtScore.text = dataHandler.getScore().ToString();
            txtStreak.text = streak.ToString();
            yield return new WaitForSeconds(imageStayTime); 
            isAppearingWrong = false;
        }

        if(!isSpawned)
        {
            prefabID = UnityEngine.Random.Range(0, nbPrefabs);
            spawnablePrefab = prefabs[prefabID];
            spawnedObject = Instantiate(spawnablePrefab, spawnPos, spawnRot);
            isSpawned=true;
        }

        yield return null;
    }


    //:COMMENT:16/03/2022:HEURTEL: displays an image depending on the result right/wrong
    void OnGUI() 
    { 
        if(isAppearingRight){
            GUI.DrawTexture ( new Rect (0,300, Screen.width, Screen.width), imageRight);
        }
        if(isAppearingWrong){
            GUI.DrawTexture ( new Rect (0,300, Screen.width, Screen.width), imageWrong);
        }
    }
}
