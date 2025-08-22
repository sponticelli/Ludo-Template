namespace Ludo.Core.Services
{
    public interface ISaveStore
    {
        void Save<T>(string key, T data);
        T Load<T>(string key, T defaultValue = default);
        void Delete(string key);
        void Flush();
    }
}