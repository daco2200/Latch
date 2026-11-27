using UnityEngine;

[System.Serializable]
public class Sfx
{
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0f, 0.3f)] public float pitchVariation = 0.08f;
    [Range(0f, 0.1f)] public float maxDelay = 0f;    // random start offset so simultaneous copies don't merge
    [Min(1)] public int voices = 4;                  // simultaneous copies of THIS sound

    private AudioSource[] sources;
    private int next;

    public void Init(GameObject host)
    {
        sources = new AudioSource[voices];
        for (int i = 0; i < voices; i++)
        {
            sources[i] = host.AddComponent<AudioSource>();
            sources[i].playOnAwake = false;
        }
    }

    public void Play(float master)
    {
        if (clip == null || sources == null) return;

        AudioSource s = sources[next];
        next = (next + 1) % sources.Length;

        s.clip = clip;
        s.volume = volume * master;
        s.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        if (maxDelay > 0f) s.PlayDelayed(Random.Range(0f, maxDelay));
        else s.Play();
    }
}