using UnityEngine;
using System.Collections;

public class K_SceneManager : SingletonInGame<K_SceneManager>
{
    private string currentSceneName;
    private string nextSceneName;
    private AsyncOperation resourceUnloadTask;
//    private AsyncOperation sceneLoadTask;
    private bool isLoadingLevel;
    private enum SceneState
    {
        Reset,
        Preload,
        Load,
        Unload,
        Postload,
        Ready,
        Run,
        Count }
    ;
    private SceneState sceneState;
    private delegate void UpdateDelegate();

    private UpdateDelegate[] updateDelegates;

    //--------------------------------------------------------------------------
    // public static methods
    //--------------------------------------------------------------------------
    public static void SwitchScene(string nextSceneName)
    {
        Debug.Log("<color=teal>[00] SwitchScene : " + K_SceneManager.Instance.currentSceneName + " => " + nextSceneName + "</color>");
        if (K_SceneManager.Instance.currentSceneName != nextSceneName)
        {
            K_SceneManager.Instance.nextSceneName = nextSceneName;
        }
    }
        
    #region 01
    //--------------------------------------------------------------------------
    // protected mono methods
    //--------------------------------------------------------------------------
    protected override void awake()
    {
        //Setup the array of updateDelegates
        updateDelegates = new UpdateDelegate[(int)SceneState.Count];
            
        //Set each updateDelegate
        updateDelegates [(int)SceneState.Reset] = UpdateSceneReset;
        updateDelegates [(int)SceneState.Preload] = UpdateScenePreload;
        updateDelegates [(int)SceneState.Load] = UpdateSceneLoad;
        updateDelegates [(int)SceneState.Unload] = UpdateSceneUnload;
        updateDelegates [(int)SceneState.Postload] = UpdateScenePostload;
        updateDelegates [(int)SceneState.Ready] = UpdateSceneReady;
        updateDelegates [(int)SceneState.Run] = UpdateSceneRun;
            
//        nextSceneName = "Title";
//        sceneState = SceneState.Reset;
//        camera.orthographicSize = Screen.height / 2;
    }
        
    protected void OnDisable()
    {
    }
        
    protected void OnEnable()
    {
    }
        
    protected void Start()
    {
    }
        
    protected void Update()
    {
        if (updateDelegates [(int)sceneState] != null)
        {
            updateDelegates [(int)sceneState]();
        }

        // sync level load
        if (Application.isLoadingLevel)
            isLoadingLevel = true;
    }
    #endregion
    //--------------------------------------------------------------------------
    // private methods
    //--------------------------------------------------------------------------
    // attach the new scene controller to start cascade of loading
    private void UpdateSceneReset()
    {
        // run a gc pass
        System.GC.Collect();
        sceneState = SceneState.Preload;
    }
        
    // handle anything that needs to happen before loading
    private void UpdateScenePreload()
    {
//        sceneLoadTask = Application.LoadLevelAsync(nextSceneName);
        Application.LoadLevel(nextSceneName);
        
        sceneState = SceneState.Load;
    }

    // show the loading screen until it's loaded
    private void UpdateSceneLoad()
    {
//        // done loading?
//        if (sceneLoadTask.isDone == true)
        if (isLoadingLevel)
        {
            isLoadingLevel = false;
            sceneState = SceneState.Unload;

            //
            FindObjectOfType<K_Scene>().OnInitialize();
        }
        else
        {
            // update scene loading progress
        }
    }
        
    // clean up unused resources by unloading them
    private void UpdateSceneUnload()
    {
        // cleaning up resources yet?
        if (resourceUnloadTask == null)
        {
            resourceUnloadTask = Resources.UnloadUnusedAssets();
        } else
        {
            // done cleaning up?
            if (resourceUnloadTask.isDone == true)
            {
                resourceUnloadTask = null;
                sceneState = SceneState.Postload;
            }

        }
    }
        
    // handle anything that needs to happen immediately after loading
    private void UpdateScenePostload()
    {
        currentSceneName = nextSceneName;
        sceneState = SceneState.Ready;
    }
        
    // handle anything that needs to happen immediately before running
    private void UpdateSceneReady()
    {
        // run a gc pass
        // if you have assets loaded in the scene that are
        // currently unused currently but may be used later
        // DON'T do this here
        System.GC.Collect();
        sceneState = SceneState.Run;
    }
        
    // wait for scene change
    private void UpdateSceneRun()
    {
        if (currentSceneName != nextSceneName)
        {
            sceneState = SceneState.Reset;
        }
    }

    void OnLevelWasLoaded(int scene)
    {
        Debug.Log("<color=cyan>[Level Loaded] " + Application.loadedLevelName + "</color>");
    }
}