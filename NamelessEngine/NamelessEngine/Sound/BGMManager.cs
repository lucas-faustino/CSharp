using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace NamelessEngine.Sound
{
    public sealed class BGMManager
    {
        sealed class Songs
        {
            public readonly SoundEffectInstance[] Clips;
            public readonly double[] Durations;
            public readonly bool[] IsInits;
            public int Count { get; private set; }

            public Songs(int count)
            {
                Clips = new SoundEffectInstance[count];
                Durations = new double[count];
                IsInits = new bool[count];
                Count = 0;
            }

            public void Add(SoundEffect clip)
            {
                Clips[Count] = clip.CreateInstance();
                Clips[Count].IsLooped = true;
                Durations[Count] = clip.Duration.Seconds;
                Count++;
            }
        }

        readonly Songs _songs;

        Timer _fadeTimer = new Timer(0.25f);
        bool _isFadingOut = false;

        public int? SongIndex { get; private set; }
        public bool IsSongPlaying => SongIndex != null;

        public float Duration
        {
            get
            {
                if(SongIndex is int index)
                {
                    return (float)_songs.Durations[index];
                }

                return 0f;
            }
        }

        public BGMManager(SoundEffect[] bgms)
        {
            _songs = new Songs(bgms.Length);

            for (int i = 0; i < bgms.Length; i++)
                _songs.Add(bgms[i]);
        }

        internal void Update(float dt)
        {
            const float decrement = 0.1f;

            if (_isFadingOut)
            {
                Timer.Tick(ref _fadeTimer, dt);
                if (_fadeTimer.IsElapsed && SongIndex is int index)
                {
                    _songs.Clips[index].Volume = MathHelper.Clamp(_songs.Clips[index].Volume - decrement, 0f, 1f);
                    Timer.Reset(ref _fadeTimer);
                }
            }
        }

        public void Play(int index, float volume = 1f, bool isRepeating = false)
        {
            // don't restart a song that's already playing
            if (SongIndex is int id && id != index)
                _songs.Clips[id].Stop();

            var bgm = _songs.Clips[index];
            // keep this in order; once a song is playing, it's looping state can't be changed so we have a flag for that
            // game will literally crash if you try to change the flag more than once.
            if (_songs.IsInits[index] == false)
            {
                bgm.IsLooped = isRepeating;
                _songs.IsInits[index] = true;
            }

            bgm.Volume = volume;
            bgm.Play();
            SongIndex = index;
            _isFadingOut = false;
        }

        public void Stop()
        {
            if(SongIndex is int index)
                _songs.Clips[index].Stop(true);
        }

        public void Restart()
        {
            if(SongIndex is int index)
            {
                _songs.Clips[index].Stop(true);
                _songs.Clips[index].Play();
            }
        }

        public void FadeOut()
        {
            Timer.Reset(ref _fadeTimer);
            _isFadingOut = true;
        }

        internal void Unload()
        {
            foreach (var song in _songs.Clips)
                song.Dispose();
        }
    }
}
