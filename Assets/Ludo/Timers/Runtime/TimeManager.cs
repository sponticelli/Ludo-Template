using System.Collections;
using Ludo.Core;
using UnityEngine;

namespace Ludo.Timers
{
    /// <summary>
    /// Provides centralized control over custom time scaling for both per-frame (Update) and
    /// fixed-step (FixedUpdate) gameplay. It exposes scaled delta times, a fixed-step
    /// interpolation factor for smooth rendering, and utilities to switch between discrete
    /// fixed time scale levels. Time changes are applied safely at end-of-frame or at the
    /// next fixed step via coroutines.
    /// </summary>
    public class TimeManager : AModule
    {
        /// <summary>
        /// Pending logical time scale to be applied at the end of the current frame.
        /// </summary>
        private static float _targetTimeScale = 1f;

        /// <summary>
        /// Pending logical fixed time scale to be applied at the next fixed update.
        /// </summary>
        private static float _targetFixedTimeScale = 1f;

        /// <summary>
        /// Circular buffer storing the last two <see cref="Time.fixedTime"/> values
        /// to compute the interpolation factor between fixed steps.
        /// </summary>
        private readonly float[] _lastFixedTimes = new float[2];

        /// <summary>
        /// Index in the circular buffer where the next fixed timestamp will be written.
        /// </summary>
        private int _newTimeIndex;

        /// <summary>
        /// Number of fixed steps that compose a logical "tick".
        /// </summary>
        public const int FramesPerTick = 4;

        /// <summary>
        /// Discrete fixed time scale levels available for selection.
        /// </summary>
        public static readonly float[] FixedTimescaleLevels = new float[4] { 0.25f, 0.5f, 1f, 2f };

        /// <summary>
        /// Index of the base fixed time scale level (1x).
        /// </summary>
        public const int FixedTimescaleBaseLevel = 2;

        /// <summary>
        /// The currently selected index in <see cref="FixedTimescaleLevels"/>.
        /// </summary>
        public static int CurrentFixedTimescaleLevel = 2;

        /// <summary>
        /// Per-frame delta time scaled by <see cref="TimeScale"/>.
        /// </summary>
        public static float DeltaTime => Time.deltaTime * TimeScale;

        /// <summary>
        /// Fixed-step delta time scaled by <see cref="FixedTimeScale"/>.
        /// </summary>
        public static float FixedDeltaTime => Time.fixedDeltaTime * FixedTimeScale;

        /// <summary>
        /// Duration, in seconds, of a logical "tick" (i.e., <see cref="FramesPerTick"/> fixed steps).
        /// </summary>
        public static float TickLength => Time.fixedDeltaTime * 4f;

        /// <summary>
        /// Number of rendered frames that fit into one logical tick under the current fixed time scale.
        /// </summary>
        public static float CurrentFramesPerTick => 4f / FixedTimeScale;

        /// <summary>
        /// Logical time scale applied to frame-based gameplay code.
        /// This does not modify Unity's <see cref="Time.timeScale"/>.
        /// </summary>
        public static float TimeScale { get; private set; } = 1f;

        /// <summary>
        /// Logical fixed time scale applied to fixed-step gameplay code.
        /// This does not modify Unity's <see cref="Time.timeScale"/>.
        /// </summary>
        public static float FixedTimeScale { get; private set; } = 1f;

        /// <summary>
        /// Returns the index of the previously written fixed timestamp in the circular buffer.
        /// </summary>
        private int PrevTimeIndex => _newTimeIndex != 0 ? 0 : 1;

        /// <summary>
        /// Interpolation factor in the range [0, 1] between the last two fixed steps.
        /// Useful for smoothing render updates between discrete physics ticks.
        /// </summary>
        public static float InterpolationFactor { get; private set; }

        /// <summary>
        /// Starts background routines responsible for safely applying requested time scale changes.
        /// </summary>
        protected override void HandleInitialization()
        {
            StartCoroutine(HandleTimeScaleUpdate());
            StartCoroutine(HandleFixedTimeScaleUpdate());
        }

        /// <summary>
        /// Performs cleanup when the module is uninitialized.
        /// Currently no explicit cleanup is required.
        /// </summary>
        protected override void HandleUninitialization()
        {
        }

        /// <summary>
        /// Updates the <see cref="InterpolationFactor"/> each frame using the last two
        /// recorded <see cref="Time.fixedTime"/> values.
        /// </summary>
        private void Update()
        {
            float num = _lastFixedTimes[_newTimeIndex];
            float num2 = _lastFixedTimes[PrevTimeIndex];
            if (num != num2)
            {
                InterpolationFactor = (Time.time - num) / (num - num2);
            }
            else
            {
                InterpolationFactor = 1f;
            }
        }

        /// <summary>
        /// Rotates the fixed-time circular buffer and stores the current <see cref="Time.fixedTime"/>.
        /// </summary>
        private void FixedUpdate()
        {
            _newTimeIndex = PrevTimeIndex;
            _lastFixedTimes[_newTimeIndex] = Time.fixedTime;
        }

        /// <summary>
        /// Requests a new logical <see cref="TimeScale"/>.
        /// </summary>
        /// <param name="value">The desired time scale value.</param>
        /// <param name="force">If true, applies the change immediately instead of waiting for the end of the frame.</param>
        public static void SetTimeScale(float value, bool force = false)
        {
            _targetTimeScale = value;
            if (force)
            {
                TimeScale = value;
            }
        }

        /// <summary>
        /// Resets the logical <see cref="TimeScale"/> to 1.
        /// </summary>
        public static void ResetTimeScale()
        {
            SetTimeScale(1f);
        }

        /// <summary>
        /// Requests a new fixed time scale level from <see cref="FixedTimescaleLevels"/>.
        /// </summary>
        /// <param name="level">The index of the desired fixed time scale level.</param>
        /// <param name="force">If true, applies the change immediately instead of waiting for the next fixed update.</param>
        public static void SetFixedTimeScale(int level, bool force = false)
        {
            int num = Mathf.Clamp(level, 0, FixedTimescaleLevels.Length - 1);
            float fixedTimeScale = (_targetFixedTimeScale = FixedTimescaleLevels[num]);
            if (force)
            {
                FixedTimeScale = fixedTimeScale;
            }

            CurrentFixedTimescaleLevel = num;
        }

        /// <summary>
        /// Resets the fixed time scale level to the default base level (1x).
        /// </summary>
        public static void ResetFixedTimeScale()
        {
            SetFixedTimeScale(2);
        }

        /// <summary>
        /// Coroutine that waits until the end of the frame to apply any pending <see cref="_targetTimeScale"/> change.
        /// </summary>
        private IEnumerator HandleTimeScaleUpdate()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                if (TimeScale != _targetTimeScale)
                {
                    TimeScale = _targetTimeScale;
                }
            }
        }

        /// <summary>
        /// Coroutine that waits for the next fixed update to apply any pending <see cref="_targetFixedTimeScale"/> change.
        /// </summary>
        private IEnumerator HandleFixedTimeScaleUpdate()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                if (FixedTimeScale != _targetFixedTimeScale)
                {
                    FixedTimeScale = _targetFixedTimeScale;
                }
            }
        }
    }
}