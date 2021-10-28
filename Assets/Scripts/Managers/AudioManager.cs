using Shared;
using UnityEngine;

namespace Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private AudioClip ballCrashSound;
        [SerializeField] private AudioClip ballBounceSound;
        [SerializeField] private AudioClip powerUpSound;

        private AudioSource _audioSource;

        private void Awake()
        {
            Helpers.AssertIsNotNullOrQuit(ballCrashSound, "AudioManager.ballCrashSound was not assigned");
            Helpers.AssertIsNotNullOrQuit(ballBounceSound, "AudioManager.ballBounceSound was not assigned");
            Helpers.AssertIsNotNullOrQuit(powerUpSound, "AudioManager.powerUpSound was not assigned");

            _audioSource = GetComponent<AudioSource>();

            ballCrashSound.LoadAudioData();
            ballBounceSound.LoadAudioData();
            powerUpSound.LoadAudioData();
        }

        public void PlayBallBounce()
        {
            _audioSource.PlayOneShot(ballBounceSound);
        }

        public void PlayBallCrash()
        {
            _audioSource.PlayOneShot(ballCrashSound);
        }

        public void PlayPowerUp()
        {
            _audioSource.PlayOneShot(powerUpSound);
        }
    }
}