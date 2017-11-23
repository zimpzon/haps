using DG.Tweening;
using UnityEngine;

namespace Assets.Script
{
    public class Bootstrap : MonoBehaviour
    {
        public bool IsStandAloneScene;
        public GameState StandAloneStartState;

        void Awake()
        {
            // Handle multiple instances of bootstrap so it can be added to every scene for easy debugging
            if (ServiceManager.GameServices != null)
            {
                ServiceManager.GameServices.GetAppLog().LogDebug("Bootstrap already run, skipping");
                CheckForCameras();
                return;
            }

            DOTween.Init(false, true, LogBehaviour.ErrorsOnly);

            InitializeServices(this.gameObject);

            var gameManager = ServiceManager.GameServices.GetGameManager();
            gameManager.Init(IsStandAloneScene);
            if (!IsStandAloneScene)
            {
                gameManager.SetState(GameState.Splash);
            }
            else
            {
                gameManager.SetState(StandAloneStartState, silent: true);
            }

            CheckForCameras();
        }

        public static void InitializeServices(GameObject owningComponent)
        {
            var serviceManager = ServiceManager.Instantiate(new AppLog());
            ServiceManager.ServiceObjects services = new ServiceManager.ServiceObjects()
            {
                GameManager = (IGameManager)owningComponent.AddComponent<GameManager>(),
                Localization = new LocalizationManager("i8n/i8n"),
            };

            serviceManager.SetServices(services);
        }

        void CheckForCameras()
        {
            // Handle multiple instances of Camera so it can be added to every scene for easy debugging
            var camera = GetComponentInChildren<Camera>();
            var audioListener = GetComponentInChildren<AudioListener>();

            var mainCamera = Camera.main;
            if (mainCamera == null || !mainCamera.enabled || mainCamera == camera)
            {
                if (mainCamera == camera)
                    ServiceManager.GameServices.GetAppLog().LogDebug("This is the main camera, make sure it is enabled");
                else
                    ServiceManager.GameServices.GetAppLog().LogDebug("No active camera present, enabling this one");

                camera.enabled = true;
                audioListener.enabled = true;
            }
            else
            {
                ServiceManager.GameServices.GetAppLog().LogDebug("Active camera found. Make sure this one is disabled");
                camera.enabled = false;
                audioListener.enabled = false;
            }
        }
    }
}
