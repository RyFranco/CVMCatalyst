using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get; private set;}

    private float fadeTime = 3f;

    private AudioSource sourceA;
    private AudioSource sourceB;
    private AudioSource activeSource;

    public AudioClip[] tracks;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        sourceA = gameObject.AddComponent<AudioSource>();
        sourceB = gameObject.AddComponent<AudioSource>();

        sourceA.loop = true;
        sourceB.loop = true;

        activeSource = sourceA;
        activeSource.clip = tracks[0];
        activeSource.volume = 0.5f;
        activeSource.Play();
        

    }

    public void Play(int i)
    {
        activeSource.clip = tracks[i];
        activeSource.volume = 0.5f;
        activeSource.Play();
        
    }

    public void PlayWithFade(int i)
    {
        AudioSource inactive = activeSource == sourceA ? sourceB : sourceA;

        inactive.clip = tracks[i ];
        inactive.volume = 0f;
        inactive.Play();

        StopAllCoroutines();
        StartCoroutine(Crossfade(activeSource, inactive));

        activeSource = inactive;
    }

    private IEnumerator Crossfade(AudioSource from, AudioSource to)
    {
        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeTime;
            from.volume = Mathf.Lerp(0.5f, 0f, t);
            to.volume = Mathf.Lerp(0f, 0.5f, t);
            yield return null;
        }

        from.Stop();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I)) PlayWithFade (1);
    }
}
