namespace Assets.Script
{
    public interface IGameServices
    {
        IAppLog GetAppLog();
        IGameManager GetGameManager();
        ILocalization GetLocalization();
        ServiceManager.ServiceObjects GetAllServices();
    }

    public class ServiceManager : IGameServices
    {
        public static IGameServices GameServices { get; private set; }

        // AppLog is always first service added so other services can log initialization errors
        public static ServiceManager Instantiate(IAppLog appLog)
        {
            if (GameServices != null)
            {
                throw new System.Exception("Trying to instantiate ServiceManager more than once");
            }

            GameServices = new ServiceManager(appLog);
            return (ServiceManager)GameServices;
        }

        public class ServiceObjects
        {
            public IAppLog AppLog;
            public IGameManager GameManager;
            public ILocalization Localization;
        }

        private ServiceObjects services_;
        private IAppLog appLog_;

        public void SetServices(ServiceObjects services)
        {
            services_ = services;
            services_.AppLog = appLog_;
        }

        private ServiceManager(IAppLog appLog)
        {
            appLog_ = appLog;
        }

        public IGameManager GetGameManager()
        {
            return services_.GameManager;
        }

        public IAppLog GetAppLog()
        {
            return appLog_;
        }

        public ILocalization GetLocalization()
        {
            return services_.Localization;
        }

        public ServiceObjects GetAllServices()
        {
            return services_;
        }
    }
}
