using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager Instance;
    private void Awake()
    {
		Instance = this;
    }

    [SerializeField] AudioClip[] Clips;
	Dictionary<string, AudioClip> _clipDict;

	[SerializeField] AudioSource MusicSource;
	[SerializeField] float MaxMusicVolume;

	List<AudioSource> _usedSources;
	List<AudioSource> _unusedSources;

	// Use this for initialization
	void Start () {

		_clipDict = new Dictionary<string, AudioClip>();
		if (Clips != null)
		{
			foreach (var clip in Clips)
			{
				_clipDict.Add(clip.name, clip);
			}
		}

		_unusedSources = new List<AudioSource>();
		_usedSources = new List<AudioSource>();
	}
	
	public void SetMusicVolume(float volume)
    {
		MusicSource.volume = volume * MaxMusicVolume;
    }

	public void PlayUISound(string clip, float volume = 1.0f)
	{
		if (_clipDict.ContainsKey(clip))
			StartCoroutine(PlaySound(GetAvailableSource(), _clipDict[clip]));
		else
			Debug.LogError("Could not find sound " + clip);
	}

	AudioSource GetAvailableSource()
    {
		if(_unusedSources.Count > 0)
        {
			AudioSource source = _unusedSources[0];
			_unusedSources.Remove(source);
			return source;
        }
		return gameObject.AddComponent<AudioSource>();
	}

	IEnumerator PlaySound(AudioSource source, AudioClip clip)
	{
		_usedSources.Add(source);

		source.volume = 1.0f;
		source.clip = clip;
		source.Play();
		while (source.isPlaying)
		{
			yield return null;
		}
		_usedSources.Remove(source);
		_unusedSources.Add(source);
	}

}
