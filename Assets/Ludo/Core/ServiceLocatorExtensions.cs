namespace Ludo.Core
{
    public static class ServiceLocatorExtensions
    {
        public static T GetService<T>(this object _) => ServiceLocator.Get<T>();
    }


    public static class ServiceExecutionOrder
    {
        public const int Installer = -1000;
    }
}