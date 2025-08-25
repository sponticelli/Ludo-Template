using UnityEngine;

namespace Ludo.Core.Commands
{
    
    
    public interface ICommand
    {
        void Execute();
    }
    
    public interface ICommand<in T>
    {
        void Execute(T arg);
    }
    
    public interface IAsyncCommand : ICommand
    {
        Awaitable ExecuteAsync();
    }
    
    public interface IAsyncCommand<in T>  : ICommand<T>
    {
        Awaitable ExecuteAsync(T arg);
    }
}