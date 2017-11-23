using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Script
{
    public interface IGameManager
    {
        void Init(bool isStandAloneScene);
        void SetState(GameState newState, bool silent = false);
    }

    public enum GameState { None, Restart, Splash, MainGame };

    public class GameManager : MonoBehaviour, IGameManager
    {
        static class SceneNames
        {
            public static string RootScene = "RootScene";
            public static string SplashScene = "SplashScene";
            public static string GameScene = "GameScene";
        }

        private ServiceManager.ServiceObjects services_;
        private IEnumerator stateExitCo_;
        private GameState currentGameState_;
        private bool isStandAloneScene_;

        public void Init(bool isStandAloneScene)
        {
            isStandAloneScene_ = isStandAloneScene;
            services_ = ServiceManager.GameServices.GetAllServices();
        }

        public void SetState(GameState newState, bool silent)
        {
            if (silent)
                currentGameState_ = newState;
            else
                StartCoroutine(SetStateCo(newState));
        }

        public IEnumerator SetStateCo(GameState newState)
        {
            services_.AppLog.LogDebug("GameManager: Setting game state: {0}, was: {1}", newState, currentGameState_);

            if (stateExitCo_ != null)
            {
                services_.AppLog.LogDebug("GameManager: Executing state exit coroutine");

                yield return stateExitCo_;
                services_.AppLog.LogDebug("GameManager: State exit coroutine complete");
                stateExitCo_ = null;
            }

            currentGameState_ = newState;

            switch (newState)
            {
                case GameState.Restart:
                    RestartGame();
                    break;

                case GameState.Splash:
                    StartCoroutine(ShowSplash());
                    break;

                case GameState.MainGame:
                    StartCoroutine(ShowMainGame());
                    break;
            }

            services_.AppLog.LogDebug("GameManager: State changed to {0}", currentGameState_);
        }

        public IEnumerator ShowSplash()
        {
            SceneManager.LoadScene(SceneNames.SplashScene, LoadSceneMode.Additive);
            yield return null;
        }

        public IEnumerator ShowMainGame()
        {
            SceneManager.LoadScene(SceneNames.GameScene, LoadSceneMode.Additive);
            yield return null;
        }

        public void RestartGame()
        {
            int sceneCount = SceneManager.sceneCount;
            for (int i = 0; i < sceneCount; ++i)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == SceneNames.RootScene)
                    continue;

                services_.AppLog.LogDebug("Unloading scene: {0}", scene.name);
                SceneManager.UnloadSceneAsync(scene);
            }

            SceneManager.LoadSceneAsync(SceneNames.SplashScene);
        }

        void OnApplicationQuit()
        {
            // TODO: Flush roll batch
            services_.AppLog.LogDebug("Main: OnApplicationQuit");
        }

        // http://answers.unity3d.com/questions/496290/can-somebody-explain-the-onapplicationpausefocus-s.html
        //App initially starts:
        //OnApplicationFocus(true) is called
        //App is soft closed:
        //OnApplicationFocus(false) is called
        //OnApplicationPause(true) is called
        //App is brought forward after soft closing:
        //OnApplicationPause(false) is called
        //OnApplicationFocus(true) is called
        private void OnApplicationFocus(bool focus)
        {
            if (services_ == null)
                return;

            services_.AppLog.LogDebug("Main: OnApplicationFocus, focus = {0}", focus);
        }

        private void OnApplicationPause(bool pause)
        {
            // TODO: Flush roll batch
            // This gets called when running in editor before FB is initialized for the first time.
            // This would trigger a game restart because FB is not initialized. Prevent that.
            if (services_ == null || currentGameState_ == GameState.Splash || isStandAloneScene_)
                return;

            AppLog.StaticLogInfo("Main: OnApplicationPause, pause = {0}", pause);

            bool resumed = !pause;
            if (resumed)
            {
                // TODO: Playfab login
            }
        }
    }
}
