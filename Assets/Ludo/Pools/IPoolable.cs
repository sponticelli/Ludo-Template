namespace Ludo.Pools
{
    /// <summary>
    /// Optional hooks for pooled objects/components
    /// </summary>
    public interface IPoolable
    {
        // Called right before the object is given out
        void OnBeforeRent();

        // Called right before the object is returned to the pool
        void OnBeforeReturn();

        // Put your object back to a clean state (called on return)
        void ResetState();
    }
}