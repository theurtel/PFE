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

    public ReadCSV readCSV;
    public DataHandler dataHandler;

    private int streak=0;
    private string currentDate = DateTime.Now.ToShortDateString();

    [SerializeField]
    GameObject camera;

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
    GameObject button; 

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
    public Texture2D imageRight;
    [SerializeField]
    public Texture2D imageWrong;
    
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
        prefabID = UnityEngine.Random.Range(0, readCSV.getSize());
        spawnablePrefab = prefabs[prefabID];
    }

    
    //:COMMENT:16/03/2022:PIET: récupère la position du toucher - retourne false si on ne touche pas, true sinon
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



    private void Update()
    {
        //:COMMENT:20/03/2022:HEURTEL: si l'animation doit s'activer, on rapproche ou éloigne l'objet de la caméra selon si on le mange ou non
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


        //COMMENT:17/03/2022:HEURTEL: si le bouton est pressé et qu'un objet est placé
        if(buttonHandler.isPressed() && isSpawned)
        {   
            //COMMENT:17/03/2022:HEURTEL: si c'est la première frame ou le bouton est pressé, on stocke la position de départ de la caméra, la distance et l'axe de départ entre l'objet et la caméra
            if(!buttonPressed)
            {
                origCamPos = camera.transform.position;
                prevCamPos = origCamPos;
                origDist = Vector3.Distance(spawnPos, origCamPos);
                axis = Vector3.Normalize(spawnPos - origCamPos);
                buttonPressed = true;
            }            

            eat = false ;

            //COMMENT:18/03/2022:HEURTEL: on cherche à maintenir la distance de départ entre la caméra et l'objet : si la distance actuelle est inférieure, c'est qu'on se rapproche de l'objet donc va l'éloigner le long de l'axe (et inversement)
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

            //COMMENT:19/03/2022:HEURTEL: si l'objet a parcouru une certaine distance, on active la routine de validation et on prépare l'animation de rapprochement/éloignement de l'objet (cf début de Update) sur le référentiel caméra objet actuel
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

        //COMMENT:17/03/2022:HEURTEL: si on relâche le bouton, l'objet retourne à sa position de départ
        if(!buttonHandler.isPressed() && isSpawned && buttonPressed)        
        {
            spawnedObject.transform.position = spawnPos;
            buttonPressed = false;
        }

        if(!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        //:COMMENT:17/03/2022:HEURTEL: si on touche le bas de l'écran, on retourne directement pour éviter un conflit avec le bouton
        if(touchPosition.y<button.transform.position.y*2)
            return;

        //:COMMENT:16/03/2022:PIET: si on n'a pas d'objet affiché, on l'instancie à la position de toucher, sinon on déplace l'objet existant
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

    //:COMMENT:16/03/2022:HEURTEL: si on fait le bon ou le mauvais choix, change le booléen correspondant pour activer l'affichage d'image dans OnGui pendant le temps défini par imageStayTime
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

    //:COMMENT:16/03/2022:HEURTEL: affiche une image selon le résultat right/wrong
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
