using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SoundEffect
{
	public string Name;
	public AudioClip Clip;
	public float Volume;
	public float MinPitch;
	public float MaxPitch;
}

public class SoundManager : MonoBehaviour {

	public static SoundManager Instance;
    private void Awake()
    {
		Instance = this;
    }

	[SerializeField] SoundEffect[] Clips;
	Dictionary<string, SoundEffect> _seDict;

	[SerializeField] AudioSource MusicSource;
	[SerializeField] float MaxMusicVolume;
	[SerializeField] float MusicOffset;

	List<AudioSource> _usedSources;
	List<AudioSource> _unusedSources;

	// Use this for initialization
	void Start () {

		_seDict = new Dictionary<string, SoundEffect>();
		if (Clips != null)
		{
			foreach (var clip in Clips)
			{
				_seDict.Add(clip.Name, clip);
			}
		}

		_unusedSources = new List<AudioSource>();
		_usedSources = new List<AudioSource>();

		MusicSource.volume = MaxMusicVolume;
		MusicSource.Play();
		MusicSource.time = MusicOffset;
	}
	
	public void SetMusicVolume(float volume)
    {
		MusicSource.volume = volume * MaxMusicVolume;
    }

	public void PlayUISound(string key)
	{
		if (_seDict.ContainsKey(key))
			StartCoroutine(PlaySound(GetAvailableSource(), _seDict[key]));
		else
			Debug.LogError("Could not find sound " + key);
	}

	public void PlayUISoundExtreme(string key, bool upper)
	{
		if (_seDict.ContainsKey(key))
			StartCoroutine(PlaySound(GetAvailableSource(), _seDict[key], upper));
		else
			Debug.LogError("Could not find sound " + key);
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

	IEnumerator PlaySound(AudioSource source, SoundEffect se)
	{
		_usedSources.Add(source);

		source.volume = se.Volume;
		source.clip = se.Clip;
		source.pitch = Random.Range(se.MinPitch, se.MaxPitch);
		source.Play();
		while (source.isPlaying)
		{
			yield return null;
		}
		_usedSources.Remove(source);
		_unusedSources.Add(source);
	}

	IEnumerator PlaySound(AudioSource source, SoundEffect se, bool upper)
	{
		_usedSources.Add(source);

		source.volume = se.Volume;
		source.clip = se.Clip;
		source.pitch = upper ? se.MaxPitch : se.MinPitch;
		source.Play();
		while (source.isPlaying)
		{
			yield return null;
		}
		_usedSources.Remove(source);
		_unusedSources.Add(source);
	}

}
