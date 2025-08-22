using System;
using UnityEngine;

namespace Ludo.Core.Services
{
    public interface IInputService : IDisposable
    {
        event Action<Vector2> Move;
        event Action<Vector2> Look;
        event Action Attack;

        void EnablePlayer();
        void DisablePlayer();
    }
}