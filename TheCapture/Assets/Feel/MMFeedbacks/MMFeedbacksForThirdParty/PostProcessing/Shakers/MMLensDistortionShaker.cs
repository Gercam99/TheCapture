﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using MoreMountains.Feedbacks;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// Add this class to a Camera with a lens distortion post processing and it'll be able to "shake" its values by getting events
    /// </summary>
    [AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMLensDistortionShaker")]
    [RequireComponent(typeof(PostProcessVolume))]
    public class MMLensDistortionShaker : MMShaker
    {
        [Header("Intensity")]
        /// whether or not to add to the initial value
        [Tooltip("whether or not to add to the initial value")]
        public bool RelativeIntensity = false;
        /// the curve used to animate the intensity value on
        [Tooltip("the curve used to animate the intensity value on")]
        public AnimationCurve ShakeIntensity = new AnimationCurve(new Keyframe(0, 0),
                                                                    new Keyframe(0.2f, 1),
                                                                    new Keyframe(0.25f, -1),
                                                                    new Keyframe(0.35f, 0.7f),
                                                                    new Keyframe(0.4f, -0.7f),
                                                                    new Keyframe(0.6f, 0.3f),
                                                                    new Keyframe(0.65f, -0.3f),
                                                                    new Keyframe(0.8f, 0.1f),
                                                                    new Keyframe(0.85f, -0.1f),
                                                                    new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")]
        [Range(-100f, 100f)]
        public float RemapIntensityZero = 0f;
        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")]
        [Range(-100f, 100f)]
        public float RemapIntensityOne = 50f;

        protected PostProcessVolume _volume;
        protected LensDistortion _lensDistortion;
        protected float _initialIntensity;
        protected float _originalShakeDuration;
        protected AnimationCurve _originalShakeIntensity;
        protected float _originalRemapIntensityZero;
        protected float _originalRemapIntensityOne;
        protected bool _originalRelativeIntensity;

        /// <summary>
        /// On init we initialize our values
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();
            _volume = this.gameObject.GetComponent<PostProcessVolume>();
            _volume.profile.TryGetSettings(out _lensDistortion);
        }

        /// <summary>
        /// When that shaker gets added, we initialize its shake duration
        /// </summary>
        protected virtual void Reset()
        {
            ShakeDuration = 0.8f;
        }

        /// <summary>
        /// Shakes values over time
        /// </summary>
        protected override void Shake()
        {
            float newValue = ShakeFloat(ShakeIntensity, RemapIntensityZero, RemapIntensityOne, RelativeIntensity, _initialIntensity);
            _lensDistortion.intensity.Override(newValue);
        }

        /// <summary>
        /// Collects initial values on the target
        /// </summary>
        protected override void GrabInitialValues()
        {
            _initialIntensity = _lensDistortion.intensity;
        }

        /// <summary>
        /// When we get the appropriate event, we trigger a shake
        /// </summary>
        /// <param name="intensity"></param>
        /// <param name="duration"></param>
        /// <param name="amplitude"></param>
        /// <param name="relativeIntensity"></param>
        /// <param name="feedbacksIntensity"></param>
        /// <param name="channel"></param>
        public virtual void OnMMLensDistortionShakeEvent(AnimationCurve intensity, float duration, float remapMin, float remapMax, bool relativeIntensity = false,
            float feedbacksIntensity = 1.0f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, 
            bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
        {
            if (!CheckEventAllowed(channel) || (!Interruptible && Shaking))
            {
                return;
            }
            
            if (stop)
            {
                Stop();
                return;
            }
            
            _resetShakerValuesAfterShake = resetShakerValuesAfterShake;
            _resetTargetValuesAfterShake = resetTargetValuesAfterShake;

            if (resetShakerValuesAfterShake)
            {
                _originalShakeDuration = ShakeDuration;
                _originalShakeIntensity = ShakeIntensity;
                _originalRemapIntensityZero = RemapIntensityZero;
                _originalRemapIntensityOne = RemapIntensityOne;
                _originalRelativeIntensity = RelativeIntensity;
            }

            TimescaleMode = timescaleMode;
            ShakeDuration = duration;
            ShakeIntensity = intensity;
            RemapIntensityZero = remapMin * feedbacksIntensity;
            RemapIntensityOne = remapMax * feedbacksIntensity;
            RelativeIntensity = relativeIntensity;

            ForwardDirection = forwardDirection;

            Play();
        }

        /// <summary>
        /// Resets the target's values
        /// </summary>
        protected override void ResetTargetValues()
        {
            base.ResetTargetValues();
            _lensDistortion.intensity.Override(_initialIntensity);
        }

        /// <summary>
        /// Resets the shaker's values
        /// </summary>
        protected override void ResetShakerValues()
        {
            base.ResetShakerValues();
            ShakeDuration = _originalShakeDuration;
            ShakeIntensity = _originalShakeIntensity;
            RemapIntensityZero = _originalRemapIntensityZero;
            RemapIntensityOne = _originalRemapIntensityOne;
            RelativeIntensity = _originalRelativeIntensity;
        }

        /// <summary>
        /// Starts listening for events
        /// </summary>
        public override void StartListening()
        {
            base.StartListening();
            MMLensDistortionShakeEvent.Register(OnMMLensDistortionShakeEvent);
        }

        /// <summary>
        /// Stops listening for events
        /// </summary>
        public override void StopListening()
        {
            base.StopListening();
            MMLensDistortionShakeEvent.Unregister(OnMMLensDistortionShakeEvent);
        }
    }

    /// <summary>
    /// An event used to trigger vignette shakes
    /// </summary>
    public struct MMLensDistortionShakeEvent
    {
        public delegate void Delegate(AnimationCurve intensity, float duration, float remapMin, float remapMax, bool relativeIntensity = false,
            float feedbacksIntensity = 1.0f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, 
            bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false);
        static private event Delegate OnEvent;
        static public void Register(Delegate callback)
        {
            OnEvent += callback;
        }
        static public void Unregister(Delegate callback)
        {
            OnEvent -= callback;
        }
        static public void Trigger(AnimationCurve intensity, float duration, float remapMin, float remapMax, bool relativeIntensity = false,
            float feedbacksIntensity = 1.0f, int channel = 0, bool resetShakerValuesAfterShake = true, bool resetTargetValuesAfterShake = true, 
            bool forwardDirection = true, TimescaleModes timescaleMode = TimescaleModes.Scaled, bool stop = false)
        {
            OnEvent?.Invoke(intensity, duration, remapMin, remapMax, relativeIntensity, feedbacksIntensity, channel, resetShakerValuesAfterShake, resetTargetValuesAfterShake, forwardDirection, timescaleMode, stop);
        }
    }
}
