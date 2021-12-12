using Microsoft.Xna.Framework.Audio;

namespace NamelessEngine.Sound
{
    public sealed class SoundClipManager
    {
        sealed class SoundClips
        {
            public readonly SoundEffectInstance[] Clips;
            public readonly double[] Durations;
            public int Count { get; private set; }

            public SoundClips(int count)
            {
                Clips = new SoundEffectInstance[count];
                Durations = new double[count];
                Count = 0;
            }

            public void Add(SoundEffect clip)
            {
                Clips[Count] = clip.CreateInstance();
                Clips[Count].IsLooped = false;
                Durations[Count] = clip.Duration.Seconds;
                Count++;
            }
        }

        sealed class ClipQueue
        {
            const int CAPACITY = 32;

            struct ClipData
            {
                public readonly int ID;
                public readonly float Volume;

                public ClipData(int id, float volume)
                {
                    ID = id;
                    Volume = volume;
                }
            }

            readonly ClipData[] _clips;

            int _count;

            public ClipQueue()
            {
                _clips = new ClipData[CAPACITY];
                _count = 0;
            }

            public void Queue(int index, float volume = 1.0f)
            {
                var isInQueue = false;
                for (int i = 0; i < _count; i++)
                {
                    if (_clips[i].ID == index)
                    {
                        isInQueue = true;
                        break;
                    }
                }
                // we don't want the same sound in the queue (sounds combine together when played at the same time which boosts the volume)
                if (isInQueue == false)
                    _clips[_count++] = new ClipData(index, volume);
            }

            public void PlayClips(SoundEffectInstance[] sounds)
            {
                for (int i = 0; i < _count; i++)
                {
                    var clip = sounds[_clips[i].ID];
                    clip.Volume = _clips[i].Volume;
                    clip.Play();
                }
                // reset our queue count for the next frame
                _count = 0;
            }
        }

        readonly SoundClips _soundClips;
        readonly ClipQueue _clipQueue;

        public SoundClipManager(SoundEffect[] clips)
        {
            _soundClips = new SoundClips(clips.Length);
            _clipQueue = new ClipQueue();

            foreach(var clip in clips)
                _soundClips.Add(clip);
        }

        public void Queue(int index, float volume = 1.0f)
        {
            _clipQueue.Queue(index, volume);
        }

        internal void PlayClips()
        {
            _clipQueue.PlayClips(_soundClips.Clips);
        }

        internal void Unload()
        {
            foreach (var c in _soundClips.Clips)
                c.Dispose();
        }
    }
}
