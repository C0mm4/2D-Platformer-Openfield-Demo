using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager gm_Instance;
    public static GameManager Instance
    {
        get
        {
            // if instance is NULL create instance
            if (!gm_Instance)
            {
                gm_Instance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (gm_Instance == null)
                    Debug.Log("instance is NULL_GameManager");
            }
            return gm_Instance;
        }
    }

    public enum UIState
    {
        InPlay, CutScene, Loading, Title
    }

    ResourceManager _ResourceManager = new ResourceManager();
    public static ResourceManager Resource { get { return gm_Instance._ResourceManager; } }

    ScriptManager _scriptManager = new ScriptManager();
    public static ScriptManager Script { get { return gm_Instance._scriptManager; } }

    StageController _stageController = new StageController();
    public static StageController Stage { get {  return gm_Instance._stageController; } }

    Spawnner _spawnner = new Spawnner();
    public static Spawnner Spawnner { get {  return gm_Instance._spawnner; } }

    UIManager _uiManager = new UIManager();
    public static UIManager UIManager { get { return gm_Instance._uiManager; } }

    TriggerManager _trigger = new TriggerManager();
    public static TriggerManager Trigger { get {  return gm_Instance._trigger; } }

    SceneManager _sceneManager = new SceneManager();
    public static SceneManager Scene { get { return gm_Instance._sceneManager; } }

    CameraManager _cameraManager;
    public static CameraManager CameraManager { get { return gm_Instance._cameraManager; } }
    [SerializeField]
    GameProgress _gameProgress;
    public static GameProgress Progress { get { return gm_Instance._gameProgress; } }


    public static GameObject player;


    public static UIState uistate;

    private async void Awake()
    {
        // Set Don't Destroy Object
        DontDestroyOnLoad(gameObject);

        // Set Static Instance
        gm_Instance = this;

        // Set Game Frame 60
        Application.targetFrameRate = 60;

        // Set Time Scale normally
        Time.timeScale = 1.0f;

        // If not first run
        if (PlayerPrefs.HasKey("FirstRun"))
        {
            // Add your Game Setting after first Run

        }
        // If first run
        else
        {
            // Add your Game Setting first Run

        }

        // Data Initialize static Objects
        DataInit();

        if(_cameraManager == null)
        {
            CameraManager cm = GameObject.Find("Main Camera").GetComponent<CameraManager>();
            _cameraManager = cm;
        }

        // Create new gameProgress
        _gameProgress = new();
        _gameProgress.activateJunctions = new();
        _gameProgress.activeTrigs = new();

        // Set Resolution windows FHD
        Screen.SetResolution(1920, 1080, false);

        // Move First Map
        await Scene.MoveMap("20001010", "30001011");
    }

    private void DataInit()
    {
        UIManager.init();
        Script.init();
        // Add your Static Object Initialize

    }

    public void Update()
    {
        // Find Camera Manager
        if (_cameraManager == null)
        {
            CameraManager cm = GameObject.Find("Main Camera").GetComponent<CameraManager>();
            _cameraManager = cm;
        }
        ManagerUpdate();
    }

    private void ManagerUpdate()
    {
        UIManager.Update();
    }

    // Player Object Spawn
    public static void CharactorSpawn(Transform transform)
    {

        // Instantiate Player Object and Position Set
        var awaitObj = InstantiateAsync("Player", transform.position);
        player = awaitObj;

        player.GetComponent<PlayerController>().controlEnabled = true;

        // camera target is player object
        CameraManager.player = player.transform;
    }

    // Instantiate Game Object with Addressables path
    public static GameObject InstantiateAsync(string path, Vector3 pos = default, Quaternion rotation = default)
    {
        return Resource.InstantiateAsync(path, pos, rotation);
    }

    // return UI State
    public static UIState GetUIState()
    {
        return uistate;
    }

    // change UI State
    public static void ChangeUIState(UIState state)
    {
        uistate = state;
    }


    // Spawn Player Object In move map loading
    public static void CharactorSpawnInLoad(string doorId)
    {
        Door go = FindObjectsOfType<GameObject>().FirstOrDefault(go => go.GetComponent<Door>() != null && go.GetComponent<Door>().id == doorId).GetComponent<Door>();
        Transform pos = go.transform;
        CharactorSpawn(pos);
        player.GetComponent<PlayerController>().canMove = true;
    }
}
