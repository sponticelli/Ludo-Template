using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ludo.Core.Services
{
    /// <summary>
    /// Default implementation of <see cref="IAudioService"/> that internally manages
    /// one-shot and looping <see cref="AudioSource"/> instances.
    /// </summary>
    public sealed class AudioService : IAudioService, IDisposable
    {
        readonly Transform _root;
        readonly AudioSource _oneShotSource;

        sealed class LoopInstance
        {
            public AudioSource Source;
            public float Volume;
        }

        readonly List<LoopInstance> _loops = new();

        float _sfxVolume = 1f;
        float _loopVolume = 1f;

        /// <summary>Volume applied to one-shot sounds.</summary>
        public float SfxVolume
        {
            get => _sfxVolume;
            set => _sfxVolume = Mathf.Clamp01(value);
        }

        /// <summary>Volume applied to looping sounds.</summary>
        public float LoopVolume
        {
            get => _loopVolume;
            set
            {
                _loopVolume = Mathf.Clamp01(value);
                foreach (var l in _loops)
                    if (l.Source != null) l.Source.volume = l.Volume * _loopVolume;
            }
        }

        public AudioService(float sfxVolume = 1f, float loopVolume = 1f, bool hideInHierarchy = true)
        {
            var go = new GameObject("Audio Service");
            if (hideInHierarchy) go.hideFlags = HideFlags.HideInHierarchy;
            Object.DontDestroyOnLoad(go);
            _root = go.transform;

            _oneShotSource = go.AddComponent<AudioSource>();
            _oneShotSource.playOnAwake = false;
            _oneShotSource.loop = false;

            SfxVolume = sfxVolume;
            LoopVolume = loopVolume;
        }

        /// <inheritdoc />
        public void PlayOneShot(AudioClip clip, float vol = 1)
        {
            if (clip == null) return;
            _oneShotSource.PlayOneShot(clip, vol * _sfxVolume);
        }

        /// <inheritdoc />
        public IAudioHandle PlayLoop(AudioClip clip, float vol = 1)
        {
            if (clip == null) return DummyHandle.Instance;

            var inst = new GameObject($"Loop_{clip.name}");
            inst.transform.SetParent(_root, false);
            var src = inst.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = true;
            src.clip = clip;
            src.volume = vol * _loopVolume;
            src.Play();

            var loop = new LoopInstance { Source = src, Volume = Mathf.Clamp01(vol) };
            _loops.Add(loop);
            return new LoopHandle(this, loop);
        }

        void Release(LoopInstance inst)
        {
            if (inst == null) return;
            if (inst.Source != null)
            {
                inst.Source.Stop();
                Object.Destroy(inst.Source.gameObject);
            }
            _loops.Remove(inst);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (var l in _loops)
                if (l.Source != null)
                    Object.Destroy(l.Source.gameObject);
            _loops.Clear();
            Object.Destroy(_root.gameObject);
        }

        sealed class LoopHandle : IAudioHandle
        {
            AudioService _svc;
            LoopInstance _inst;

            public LoopHandle(AudioService svc, LoopInstance inst)
            {
                _svc = svc;
                _inst = inst;
            }

            public bool IsPlaying => _inst?.Source != null && _inst.Source.isPlaying;

            public void Stop()
            {
                if (_inst == null) return;
                _svc.Release(_inst);
                _svc = null;
                _inst = null;
            }
        }

        sealed class DummyHandle : IAudioHandle
        {
            public static readonly DummyHandle Instance = new DummyHandle();
            public bool IsPlaying => false;
            public void Stop() { }
        }
    }
}

