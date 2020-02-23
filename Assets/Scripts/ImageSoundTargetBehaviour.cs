using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EasyAR
{
    public class ImageSoundTargetBehaviour : ImageTargetBehaviour
    {
        public AudioClip loadSound;
        public AudioSource source;
        public float lowPitchRange = .75F;
        public float highPitchRange = 1.5F;
        [Range(0,1)]
        public float volume = 0.8f;
        
        protected override void Awake()
        {
            base.Awake();
            TargetFound += OnTargetFound;
        }
        void OnTargetFound(TargetAbstractBehaviour behaviour)
        {
            source.pitch = Random.Range (lowPitchRange,highPitchRange);
            source.PlayOneShot(loadSound, volume);
        }
    }
}
