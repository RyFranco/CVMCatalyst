using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {get; private set;}

    public int SongPlaying;

    private float fadeTime = 3f;

    private AudioSource sourceA;
    private AudioSource sourceB;
    private AudioSource activeSource;

    public AudioClip[] tracks;

    public float PlayerSelectedVolume;

    public bool InCombat;

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
        activeSource.volume = PlayerSelectedVolume;
        activeSource.Play();
        

    }

    void CombatSongCheck()
    {
        List<GameObject> AllUnitsList = GameObject.FindGameObjectsWithTag("Unit").ToList();
        foreach( GameObject Unit in AllUnitsList)
        {
            if(Unit.GetComponent<Unit>().currentState == ActionState.Attacking)
            {
                InCombat = true;
                if(SongPlaying != 1) PlayWithFade(1);
                return;
            }
        }
        if(SongPlaying != 0) PlayWithFade(0);
        InCombat = false; 
    }



    public void Play(int i)
    {
        SongPlaying = i;
        activeSource.clip = tracks[i];
        activeSource.volume = PlayerSelectedVolume;
        activeSource.Play();
        
    }

    public void PlayWithFade(int i)
    {
        SongPlaying = i;
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
            from.volume = Mathf.Lerp(PlayerSelectedVolume, 0f, t);
            to.volume = Mathf.Lerp(0f, PlayerSelectedVolume, t);
            yield return null;
        }

        from.Stop();
    }

    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.I)) PlayWithFade (1);
        CombatSongCheck();
    }
}
