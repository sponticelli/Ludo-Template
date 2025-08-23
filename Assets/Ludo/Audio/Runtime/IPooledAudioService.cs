using System;
using Ludo.Core.Services;
using UnityEngine;

namespace Ludo.Audio
{
    /// <summary>
    /// Interface for pooled audio service that provides AudioSource pooling capabilities
    /// for both one-shot and looping audio with performance monitoring.
    /// </summary>
    public interface IPooledAudioService : IAudioService, IDisposable
    {
        /// <summary>
        /// Gets current pool statistics for debugging and monitoring.
        /// </summary>
        /// <returns>Tuple containing (available sources, total created sources, active loops, active one-shots)</returns>
        (int available, int total, int activeLoops, int activeOneShots) GetPoolStats();
    }
}
