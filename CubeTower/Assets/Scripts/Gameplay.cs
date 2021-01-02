using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class Gameplay : MonoBehaviour
{
    public Transform TransparentCube;
    private CubePos lastCube = new CubePos(0f, 0.75f, 0f); // set position of TowerCube
    public float ChangeCubePositionSpeed = 0.5f;
    public GameObject TowerCube, TowerHeap;
    private Rigidbody TowerHeapRB;

    private bool IsLoose=false;
    private bool FirstCube = false;
    private Coroutine showCubePlace;

    public GameObject[] CanvasStartPage; // Our menu buttons and logo

    private Transform CameraPosition;

    public float MoveCameraSpeed = 0.5f;
    private float MoveToY;

    public GameObject SpawnCubeEffect;

    public Text ScoreText;

    public Material[] AllMaterialsForCubes;
    private int AvailableSize;

    private List<Vector3> SpawnedPositions = new List<Vector3>
    {
        new Vector3(0,0,0),
        new Vector3(1,0,0),
        new Vector3(0,0,1),
        new Vector3(-1,0,0),
        new Vector3(0,0,-1),
        new Vector3(1,0,1),
        new Vector3(1,0,-1),
        new Vector3(-1,0,1),
        new Vector3(-1,0,-1),
        new Vector3(0,0.75f,0),
        new Vector3(0,-0.25f,0)
    };

    private bool IsPositionEmpty(Vector3 TryingPos)
    {
        if(TryingPos.y<=0) return false;
        foreach(Vector3 Spawned in SpawnedPositions)
        {
            if (Spawned.x == TryingPos.x && Spawned.y == TryingPos.y && Spawned.z == TryingPos.z) return false;
        }
        return true;
    }

    private void SpawnPositions() // Generates free positions for spawn cubes
    {
            List<Vector3> FreePositions = new List<Vector3>();
            if (IsPositionEmpty(new Vector3(lastCube.x + 1, lastCube.y, lastCube.z)) &&
            TransparentCube.position.x != lastCube.x + 1)
                FreePositions.Add(new Vector3(lastCube.x + 1, lastCube.y, lastCube.z));
            if (IsPositionEmpty(new Vector3(lastCube.x - 1, lastCube.y, lastCube.z)) &&
            TransparentCube.position.x != lastCube.x - 1)
                FreePositions.Add(new Vector3(lastCube.x - 1, lastCube.y, lastCube.z));

            if (IsPositionEmpty(new Vector3(lastCube.x, lastCube.y + 1f, lastCube.z)) &&
            TransparentCube.position.y != lastCube.y + 1f)
                FreePositions.Add(new Vector3(lastCube.x, lastCube.y + 1f, lastCube.z));
            if (IsPositionEmpty(new Vector3(lastCube.x, lastCube.y - 1f, lastCube.z)) &&
            TransparentCube.position.y != lastCube.y - 1f)
                FreePositions.Add(new Vector3(lastCube.x, lastCube.y - 1f, lastCube.z));

            if (IsPositionEmpty(new Vector3(lastCube.x, lastCube.y, lastCube.z + 1)) &&
            TransparentCube.position.z != lastCube.z + 1)
                FreePositions.Add(new Vector3(lastCube.x, lastCube.y, lastCube.z + 1));
            if (IsPositionEmpty(new Vector3(lastCube.x, lastCube.y, lastCube.z - 1)) &&
            TransparentCube.position.z != lastCube.z - 1)
                FreePositions.Add(new Vector3(lastCube.x, lastCube.y, lastCube.z - 1));

        if (FreePositions.Count > 1)
            TransparentCube.position = FreePositions[UnityEngine.Random.Range(0, FreePositions.Count)]; // Set random position amoung free positions
                                                                                                        // and spawn on it
        else if (FreePositions.Count == 0) IsLoose = true;
        else TransparentCube.position = FreePositions[0];
        
    }

    IEnumerator ShowCubePlace()
    {
        while (true)
        {
            SpawnPositions();

            yield return new WaitForSeconds(ChangeCubePositionSpeed);
        }
    }



    private void Start()
    {
        CurrentBackgroundColor = Camera.main.backgroundColor;
        CameraPosition = Camera.main.transform;
        MoveToY = CameraPosition.localPosition.y - lastCube.y;
        TowerHeapRB = TowerHeap.GetComponent<Rigidbody>();
        ScoreText.text = "<color=#27ABA7><size=25>Record:</size> "+PlayerPrefs.GetInt("score")+"</color>\n<size=25>Now:</size> 0";
        SetSizeForAvailableMaterialsArray();
        showCubePlace = StartCoroutine(ShowCubePlace());
    }

    private void SetSizeForAvailableMaterialsArray()
    {
        int UsersScore = PlayerPrefs.GetInt("score");
        if (UsersScore >= 60) AvailableSize = 9;
        else if (UsersScore >= 45) AvailableSize = 8;
        else if (UsersScore >= 35) AvailableSize = 7;
        else if (UsersScore >= 25) AvailableSize = 6;
        else if (UsersScore >= 20) AvailableSize = 5;
        else if (UsersScore >= 15) AvailableSize = 4;
        else if (UsersScore >= 10) AvailableSize = 3;
        else if (UsersScore >= 5) AvailableSize = 2;
        else AvailableSize = 1;
    }

    private void Update()
    {
        if((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && TransparentCube!=null && !EventSystem.current.IsPointerOverGameObject()&&!IsLoose)
        {
#if !UNITY_EDITOR // If Application isn't started at Unity Editor, then 
            if (Input.GetTouch(0).phase != TouchPhase.Began) return; // if it isn't a simple touch, then leave function
#endif
            if (EventSystem.current.IsPointerOverGameObject()) return; // if we pressed on GUI button, then we won't start game process

            if (!FirstCube)
            {
                FirstCube = true;
                foreach(GameObject obj in CanvasStartPage)
                Destroy(obj);
            }


            TowerCube.GetComponent<MeshRenderer>().material = AllMaterialsForCubes[UnityEngine.Random.Range(0, AvailableSize)];
            GameObject newCube = Instantiate(
                TowerCube,  // What object need to create
                TransparentCube.position, // Place for created object
                Quaternion.identity) as GameObject;

            // Sound Effect
            PlaySoundEffect();

            GameObject SpawnCubeEffectObject = Instantiate(SpawnCubeEffect,        // Create Spawn Effect
                      TransparentCube.position,
                      Quaternion.identity) as GameObject;
            Destroy(SpawnCubeEffectObject, 1f);     // Delete Spawn effect for optipization

            newCube.transform.SetParent(TowerHeap.transform); // Input new object in TowerHeap
            lastCube.setPosition(TransparentCube.position); // Set the position of last cube
            SpawnedPositions.Add(lastCube.getPosition()); // Mark position as spawned

            //Camera Change
            ChangeCameraPosition_Background();

            //

            TowerHeapRB.isKinematic = true;
            TowerHeapRB.isKinematic = false;    

            SpawnPositions();
        }

        CameraPosition.localPosition = Vector3.MoveTowards(CameraPosition.localPosition,
           new Vector3(CameraPosition.localPosition.x, lastCube.y + MoveToY, CameraPosition.localPosition.z),
           MoveCameraSpeed*Time.deltaTime); // Делает передвижение камеры плавны

        if (TowerHeapRB!=null && TowerHeapRB.velocity.magnitude>0.2f && !IsLoose)
        {
            Destroy(TransparentCube.gameObject);
            IsLoose = true;
            StopCoroutine(showCubePlace);
            Debug.Log("Game Over!");
        }

        if (Camera.main.backgroundColor != CurrentBackgroundColor)
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, CurrentBackgroundColor, Time.deltaTime);
    }

    private float prevMaxHoriz;
    public Color[] BackgroundColors;
    private Color CurrentBackgroundColor;

    private void ChangeCameraPosition_Background()
    {
        float maxX = 0f,maxY=0f, maxZ = 0f, maxHoriz;
        foreach(Vector3 pos in SpawnedPositions)
        {
            if (Mathf.Abs(pos.x) > maxX) maxX = pos.x;

            if (pos.y > maxY) maxY = pos.y;

            if (Mathf.Abs(pos.z) > maxZ) maxZ = pos.z;
        }

        maxHoriz = maxX > maxZ ? maxX : maxZ; // if(maxX>maxZ) maxHoriz = maxX; else maxHoriz = maxZ;
        if ((maxHoriz % 2 == 0 || maxHoriz%3==0) && prevMaxHoriz!=maxHoriz)
        {
            CameraPosition.localPosition -= new Vector3(0, 0, 2f);
            prevMaxHoriz = maxHoriz;
        }

        if (maxY >= 20) CurrentBackgroundColor = BackgroundColors[3];
        else if(maxY>=15) CurrentBackgroundColor = BackgroundColors[2];
        else if (maxY >= 10) CurrentBackgroundColor = BackgroundColors[1];
        else if (maxY >= 5) CurrentBackgroundColor = BackgroundColors[0];

        if (PlayerPrefs.GetInt("score") < Convert.ToInt32(maxY)) PlayerPrefs.SetInt("score", Convert.ToInt32(maxY));
        ScoreText.text= "<color=#27ABA7><size=25>Score:</size> " + PlayerPrefs.GetInt("score") + "</color>\n<size=25>Now:</size> "+Convert.ToInt32(maxY);
    }

    private void PlaySoundEffect()
    {
        if (PlayerPrefs.GetString("music") != "No") GetComponent<AudioSource>().Play();
    }

}


// Struct for cubes and their positions
struct CubePos
{
    public float x, y, z; // The coordinates of Cube

    public CubePos(float x,float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 getPosition()
    {
        return new Vector3(x, y, z);
    }

    public void setPosition(Vector3 pos)
    {
        x = pos.x;
        y = pos.y;
        z = pos.z;
    }
}
