using System.Collections.Generic;
using UnityEngine;
using System;

namespace dingus.Behaviors
{
    internal class DingusInjury : MonoBehaviour
    {
        private const float VELOCITY_FOR_IMPACT = 10f;
        private const float IMPACT_VOLUME = 1f;
        private List<AudioClip> audioClips = new List<AudioClip>();
        private AudioSource audioSource;
        static System.Random rnd = new System.Random();

        void Awake()
        {
            audioClips.Add(Plugin.bundle.LoadAsset<AudioClip>("body_medium_impact_hard2"));
            audioClips.Add(Plugin.bundle.LoadAsset<AudioClip>("body_medium_impact_hard3"));
            audioClips.Add(Plugin.bundle.LoadAsset<AudioClip>("body_medium_impact_hard5"));
            audioClips.Add(Plugin.bundle.LoadAsset<AudioClip>("body_medium_impact_hard6"));
            audioSource = gameObject.GetComponent<AudioSource>();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.relativeVelocity.magnitude > VELOCITY_FOR_IMPACT)
            {
                int r = rnd.Next(audioClips.Count);
                audioSource.PlayOneShot(audioClips[r], IMPACT_VOLUME);
            }
        }
    }
}
