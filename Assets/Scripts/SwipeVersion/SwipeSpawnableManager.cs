using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class SwipeSpawnableManager : MonoBehaviour
{

    public ReadCSV readCSV;
    ARRaycastManager m_RaycastManager;
    GameObject spawnedObject;
    bool isSpawned = false;
    int iter=0;

    private Vector2 touchPosition;
    private Vector3 spawnPos;
    private Quaternion spawnRot;

    [SerializeField]
    GameObject button; 

    GameObject spawnablePrefab;
    private int prefabID=0;

    private List<GameObject> prefabs = new List<GameObject>();
    public SwipeEffect swipeEffect;
    bool _isSwiped = false;

    List<ARRaycastHit> m_Hits = new List<ARRaycastHit>();
    private int nbPrefabs = 0;

    /////////////////////////////////////////////////////////
    private bool isAppearingRight = false; //switch on/off the image (if true is showing, if false is hidden)
    private bool isAppearingWrong = false; //switch on/off the image (if true is showing, if false is hidden)
     
    [SerializeField]
    public Texture2D imageRight; //Texture of the image to show
    [SerializeField]
    public Texture2D imageWrong; //Texture of the image to show
    
     public float imageStayTime = 2.0f; //Time the image should stay on screen    
     private bool _swipeRight;
     private string name;

    //////////////////////////////////////////////////////
    
    private void Awake()
    {     
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
        _swipeRight = swipeEffect.swipeRight();   

        if (swipeEffect.hasSwiped())
        {
            isAppearingRight = false;
            isAppearingWrong = false;
            swipeEffect.unSwipe();
            name = spawnedObject.name;
            Destroy(spawnedObject);
            isSpawned = false;
            StartCoroutine("displayRightOrWrong");
        }


        if(!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if(touchPosition.y<button.transform.position.y*2)
            return;

        if(m_RaycastManager.Raycast(touchPosition, m_Hits, TrackableType.PlaneWithinPolygon))
        {
            var hitPosition = m_Hits[0].pose;
            

            if(!isSpawned)
            {
                prefabID = Random.Range(0, nbPrefabs);
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


    public IEnumerator displayRightOrWrong()
    {
        if((readCSV.isHealthy(prefabID) && _swipeRight) || (!readCSV.isHealthy(prefabID) && !_swipeRight))
        {
            isAppearingRight = true;
            yield return new WaitForSeconds(imageStayTime); 
            isAppearingRight = false;
        }
        else
        {
            isAppearingWrong = true;
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
