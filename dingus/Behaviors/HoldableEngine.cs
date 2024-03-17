/*
MIT License

Copyright (c) 2023 dev9998

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using UnityEngine;
using UnityEngine.XR;
using GorillaLocomotion;
using CSCore;
using dingus;
using Photon.Voice;

namespace DevHoldableEngine
{
    public class DevHoldable : HoldableObject
    {
        public bool InHand;
        public bool InLeftHand;
        public bool PickUp;
        public Rigidbody Rigidbody;
        public AudioClip clip;
        public AudioSource audioSource;
        public Collider boxColl;

        public float Distance = 0.2f;
        public float ThrowForce = 1.75f;

        public virtual void OnGrab(bool isLeft)
        {
            if (Rigidbody != null)
            {
                Rigidbody.isKinematic = true;
                Rigidbody.useGravity = false;
            }
            if (clip == null || audioSource == null || boxColl == null)
            {
                audioSource = gameObject.GetComponent<AudioSource>();
                boxColl = gameObject.GetComponent<BoxCollider>();
                clip = Plugin.bundle.LoadAsset<AudioClip>("wpn_select");
            }

            audioSource.PlayOneShot(clip, 1f);
        }

        public virtual void OnDrop(bool isLeft)
        {
            if (Rigidbody != null)
            {
                GorillaVelocityEstimator gorillaVelocityEstimator = (isLeft ? Player.Instance.leftControllerTransform.GetComponentInChildren<GorillaVelocityEstimator>() : Player.Instance.rightControllerTransform.GetComponentInChildren<GorillaVelocityEstimator>()) ?? null;

                if (gorillaVelocityEstimator != null)
                {
                    Rigidbody.isKinematic = false;
                    Rigidbody.useGravity = true;
                    Rigidbody.velocity = (gorillaVelocityEstimator.linearVelocity * ThrowForce) + Player.Instance.GetComponent<Rigidbody>().velocity;
                    Rigidbody.angularVelocity = gorillaVelocityEstimator.angularVelocity;
                }
            }
        }

        public void Update()
        {
            bool leftGrip = ControllerInputPoller.instance.leftGrab;
            bool rightGrip = ControllerInputPoller.instance.rightGrab;

            if (PickUp && leftGrip && Vector3.Distance(Player.Instance.leftControllerTransform.position, transform.position) < Distance && !InHand && EquipmentInteractor.instance.leftHandHeldEquipment == null)
            {
                // Hold logic
                InLeftHand = true;
                InHand = true;
                transform.SetParent(GorillaTagger.Instance.offlineVRRig.leftHandTransform.parent);

                // Other logic
                GorillaTagger.Instance.StartVibration(true, 0.1f, 0.05f);
                EquipmentInteractor.instance.leftHandHeldEquipment = this;

                // Callback
                OnGrab(true);
            }
            else if (!leftGrip && InHand && InLeftHand)
            {
                // Drop logic
                InLeftHand = true;
                InHand = false;
                transform.SetParent(null);

                // Other logic
                GorillaTagger.Instance.StartVibration(true, 0.1f, 0.05f);
                EquipmentInteractor.instance.leftHandHeldEquipment = null;

                // Callback
                OnDrop(true);
            }

            if (PickUp && rightGrip && Vector3.Distance(Player.Instance.rightControllerTransform.position, transform.position) < Distance && !InHand && EquipmentInteractor.instance.rightHandHeldEquipment == null)
            {
                // Hold logic
                InLeftHand = false;
                InHand = true;
                transform.SetParent(GorillaTagger.Instance.offlineVRRig.rightHandTransform.parent);

                // Other logic
                GorillaTagger.Instance.StartVibration(false, 0.1f, 0.05f);
                EquipmentInteractor.instance.rightHandHeldEquipment = this;

                // Callback
                OnGrab(false);
            }
            else if (!rightGrip && InHand && !InLeftHand)
            {
                // Drop logic
                InLeftHand = false;
                InHand = false;
                transform.SetParent(null);

                // Other logic
                GorillaTagger.Instance.StartVibration(false, 0.1f, 0.05f);
                EquipmentInteractor.instance.rightHandHeldEquipment = null;

                // Callback
                OnDrop(false);
            }
        }
    }
}