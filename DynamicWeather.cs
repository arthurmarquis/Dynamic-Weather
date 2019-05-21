using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class DynamicWeather : MonoBehaviour
{
    private Transform _player;
    private Transform _weather;

    public float _weatherHeight = 10.0f;

    private int _switchWeather;                     //Defines the naming convention of switch range
   
    private ParticleSystem.EmissionModule _sunnyEmission;
    private ParticleSystem.EmissionModule _thunderEmission;
    private ParticleSystem.EmissionModule _mistEmission;
    private ParticleSystem.EmissionModule _overcastEmission;
    private ParticleSystem.EmissionModule _snowEmission;   
    
    public WeatherStates _weatherStates;            //Defines the naming convention of weather states

    //Audio
    public float _audioFadeTimer = 0.25f;
    public AudioClip _sunnyAudio;
    public AudioClip _thunderAudio;
    public AudioClip _mistAudio;
    public AudioClip _overcastAudio;
    public AudioClip _snowAudio;

    //Timers
    public float _switchWeatherTimer = 0f;          //switches the weather when the timer reaches zero
    public float _minTimer = 20f;                   //Defines minimum time that weather will be active
    public float _maxTimer = 60f;                   //Defines maximum time that weather will be active

    //ParticleSystems
    public ParticleSystem _sunnyPS;
    public ParticleSystem _thunderPS;
    public ParticleSystem _mistPS;
    public ParticleSystem _overcastPS;
    public ParticleSystem _snowPS;

    //Lighting
    public float _lightDimTime = 0.1f;
    public float _sunnyIntensity = 0f;
    public float _thunderIntensity = 1f;
    public float _mistIntensity = .5f;
    public float _overcastIntensity = .75f;
    public float _snowIntensity = 0.1f;

    //Fog
    public float _fogChangeSpeed = 0.1f;
    public Color _sunnyFog;
    public Color _thunderFog;
    public Color _mistFog;
    public Color _overcastFog;
    public Color _snowFog;

    public enum WeatherStates
    {
        PickWeather,
        SunnyWeather,
        ThunderWeather,
        MistWeather,
        OvercastWeather,
        SnowWeather
    }


    void Start()
    {
        GameObject _playerGameObject = GameObject.FindGameObjectWithTag("Player");
        _player = _playerGameObject.transform;
        GameObject _weatherGameObject = GameObject.FindGameObjectWithTag("Weather");
        _weather = _weatherGameObject.transform;

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.01f;

        _sunnyEmission = _sunnyPS.emission;
        _thunderEmission = _thunderPS.emission;
        _mistEmission = _mistPS.emission;
        _overcastEmission = _overcastPS.emission;
        _snowEmission = _snowPS.emission; 
        
        _switchWeatherTimer = Random.Range(_minTimer, _maxTimer);
        Debug.Log("Starting");
        StartCoroutine(WeatherFSM());
    }
    void Update()
    {
        Debug.Log("Updating");
        SwitchWeatherTimer();                       //Checks on the timer to see if we need to change weather

        _weather.transform.position = new Vector3(_player.position.x, _player.position.y + _weatherHeight, _player.position.z);
    }
    void SwitchWeatherTimer()
    {
        Debug.Log("SwitchWeatherTimer()");
        _switchWeatherTimer -= Time.deltaTime;
        if (_switchWeatherTimer > 0) return;
        if (_switchWeatherTimer <= 0)
        {
            _weatherStates = DynamicWeather.WeatherStates.PickWeather;
        }
        _switchWeatherTimer = Random.Range(_minTimer, _maxTimer);
    }
    IEnumerator WeatherFSM()                        
    {
        Debug.Log("Enumerating");
        while (true)                                //while the weather state machine is active
        {
            switch (_weatherStates)                 //switch weather states
            {
                case WeatherStates.PickWeather:
                    PickWeather();
                    break;
                case WeatherStates.SunnyWeather:
                    SunnyWeather();
                    break;
                case WeatherStates.ThunderWeather:
                    ThunderWeather();
                    break;
                case WeatherStates.MistWeather:
                    MistWeather();
                    break;
                case WeatherStates.OvercastWeather:
                    OvercastWeather();
                    break;
                case WeatherStates.SnowWeather:
                    SnowWeather();
                    break;
            }
            yield return null;
        }
    }                       
    void PickWeather()
    {
        Debug.Log("Picking Weather");
        
        _sunnyEmission.enabled = false;
        Debug.Log("Sunny PS is " + _sunnyEmission.enabled);
        _thunderEmission.enabled = false;
        Debug.Log("Thunder PS is " + _thunderEmission.enabled);
        _mistEmission.enabled = false;
        Debug.Log("Mist PS is " + _mistEmission.enabled);
        _overcastEmission.enabled = false;
        Debug.Log("Overcast PS is " + _overcastEmission.enabled);
        _snowEmission.enabled = false;
        Debug.Log("Snow PS is " + _snowEmission.enabled);


        /*_sunnyPS.enableEmission = false;
        _thunderPS.enableEmission = false;
        _mistPS.enableEmission = false;
        _overcastPS.enableEmission = false;
        _snowPS.enableEmission = false;*/

        _switchWeather = Random.Range(0, 5);

        switch (_switchWeather)
        {
            case 0:
                _weatherStates = DynamicWeather.WeatherStates.SunnyWeather;
                break;
            case 1:
                _weatherStates = DynamicWeather.WeatherStates.ThunderWeather;
                break;
            case 2:
                _weatherStates = DynamicWeather.WeatherStates.MistWeather;
                break;
            case 3:
                _weatherStates = DynamicWeather.WeatherStates.OvercastWeather;
                break;
            case 4:
                _weatherStates = DynamicWeather.WeatherStates.SnowWeather;
                break;
        }
    }
    void SunnyWeather()
    {
        Debug.Log("It's Sunny");
        _sunnyEmission.enabled = true;
        //_sunnyPS.enableEmission = true;

        if (GetComponent<Light>().intensity > _sunnyIntensity)
            GetComponent<Light>().intensity -= Time.deltaTime * _lightDimTime;
        if (GetComponent<Light>().intensity < _sunnyIntensity)
            GetComponent<Light>().intensity += Time.deltaTime * _lightDimTime;

        if (_sunnyEmission.enabled == true)
           Debug.Log("Starting Butterflies and dandylions");

        if (GetComponent<AudioSource>().volume > 0 && GetComponent<AudioSource>().clip != _sunnyAudio)
            GetComponent<AudioSource>().volume -= Time.deltaTime * _audioFadeTimer;
        

        if (GetComponent<AudioSource>().volume == 0)
        {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().clip = _sunnyAudio;
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().Play();
        }

        if (GetComponent<AudioSource>().volume < 1 && GetComponent<AudioSource>().clip == _sunnyAudio)
            GetComponent<AudioSource>().volume += Time.deltaTime * _audioFadeTimer;

        Color _currentFogColor = RenderSettings.fogColor;
        RenderSettings.fogColor = Color.Lerp(_currentFogColor, _sunnyFog, _fogChangeSpeed * Time.deltaTime);
    }
    void ThunderWeather()
    {
        _thunderEmission.enabled = true;
        //_thunderPS.enableEmission = true;
        Debug.Log("It is Thunder and Lightning");
        if (GetComponent<Light>().intensity > _thunderIntensity)
            GetComponent<Light>().intensity -= Time.deltaTime * _lightDimTime;
        if (GetComponent<Light>().intensity < _thunderIntensity)
            GetComponent<Light>().intensity += Time.deltaTime * _lightDimTime;
        

        if (GetComponent<AudioSource>().volume > 0 && GetComponent<AudioSource>().clip != _thunderAudio)
            GetComponent<AudioSource>().volume -= Time.deltaTime * _audioFadeTimer;

        if (GetComponent<AudioSource>().volume == 0)
        {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().clip = _thunderAudio;
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().Play();
        }

        if (GetComponent<AudioSource>().volume < 1 && GetComponent<AudioSource>().clip == _thunderAudio)
            GetComponent<AudioSource>().volume += Time.deltaTime * _audioFadeTimer;

        Color _currentFogColor = RenderSettings.fogColor;
        RenderSettings.fogColor = Color.Lerp(_currentFogColor, _thunderFog, _fogChangeSpeed * Time.deltaTime);
    }
    void MistWeather()
    {
        _mistEmission.enabled = true;
        //_mistPS.enableEmission = true;
        Debug.Log("It is Misty");
        if (GetComponent<Light>().intensity > _mistIntensity)
            GetComponent<Light>().intensity -= Time.deltaTime * _lightDimTime;
        if (GetComponent<Light>().intensity < _mistIntensity)
            GetComponent<Light>().intensity += Time.deltaTime * _lightDimTime;
        

        if (GetComponent<AudioSource>().volume > 0 && GetComponent<AudioSource>().clip != _mistAudio)
            GetComponent<AudioSource>().volume -= Time.deltaTime * _audioFadeTimer;

        if (GetComponent<AudioSource>().volume == 0)
        {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().clip = _mistAudio;
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().Play();
        }

        if (GetComponent<AudioSource>().volume < 1 && GetComponent<AudioSource>().clip == _mistAudio)
            GetComponent<AudioSource>().volume += Time.deltaTime * _audioFadeTimer;

        Color _currentFogColor = RenderSettings.fogColor;
        RenderSettings.fogColor = Color.Lerp(_currentFogColor, _mistFog, _fogChangeSpeed * Time.deltaTime);
    }
    void OvercastWeather()
    {
        _overcastEmission.enabled = true;
        //_overcastPS.enableEmission = true;
        Debug.Log("It is Overcast");
        if (GetComponent<Light>().intensity > _overcastIntensity)
            GetComponent<Light>().intensity -= Time.deltaTime * _lightDimTime;
        if (GetComponent<Light>().intensity < _overcastIntensity)
            GetComponent<Light>().intensity += Time.deltaTime * _lightDimTime;
        

        if (GetComponent<AudioSource>().volume > 0 && GetComponent<AudioSource>().clip != _overcastAudio)
            GetComponent<AudioSource>().volume -= Time.deltaTime * _audioFadeTimer;

        if (GetComponent<AudioSource>().volume == 0)
        {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().clip = _overcastAudio;
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().Play();
        }

        if (GetComponent<AudioSource>().volume < 1 && GetComponent<AudioSource>().clip == _overcastAudio)
            GetComponent<AudioSource>().volume += Time.deltaTime * _audioFadeTimer;

        Color _currentFogColor = RenderSettings.fogColor;
        RenderSettings.fogColor = Color.Lerp(_currentFogColor, _overcastFog, _fogChangeSpeed * Time.deltaTime);
    }
    void SnowWeather()
    {
        _snowEmission.enabled = true;
        //_snowPS.enableEmission = true;
        Debug.Log("It is Snowing");
        if (GetComponent<Light>().intensity > _snowIntensity)
            GetComponent<Light>().intensity -= Time.deltaTime * _lightDimTime;
        if (GetComponent<Light>().intensity < _snowIntensity)
            GetComponent<Light>().intensity += Time.deltaTime * _lightDimTime;
        

        if (GetComponent<AudioSource>().volume > 0 && GetComponent<AudioSource>().clip != _snowAudio)
            GetComponent<AudioSource>().volume -= Time.deltaTime * _audioFadeTimer;

        if (GetComponent<AudioSource>().volume == 0)
        {
            GetComponent<AudioSource>().Stop();
            GetComponent<AudioSource>().clip = _snowAudio;
            GetComponent<AudioSource>().loop = true;
            GetComponent<AudioSource>().Play();
        }

        if (GetComponent<AudioSource>().volume < 1 && GetComponent<AudioSource>().clip == _snowAudio)
            GetComponent<AudioSource>().volume += Time.deltaTime * _audioFadeTimer;

        Color _currentFogColor = RenderSettings.fogColor;
        RenderSettings.fogColor = Color.Lerp(_currentFogColor, _snowFog, _fogChangeSpeed * Time.deltaTime);
    }
}
