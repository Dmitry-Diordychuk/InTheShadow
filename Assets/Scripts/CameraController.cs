using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InTheShadow
{
    public class CameraController : MonoBehaviour
    {
        // Movement speed in units per second.
        private float _lerpSpeed = 1.0F;

        // Time when the movement started.
        private float _lerpStartTime;

        // Total distance between the markers.
        private float _lerpLength;
        
        public void InitLerp(Vector3 startPosition, Vector3 endPosition)
        {
            _lerpStartTime = Time.time;
            _lerpLength = Vector3.Distance(startPosition, endPosition);
        }

        public float FocusOnEndPosition(Vector3 startPosition, Vector3 endPosition, Quaternion startRotation, Quaternion endRotation)
        {
            // Distance moved equals elapsed time times speed..
            float distCovered = (Time.time - _lerpStartTime) * _lerpSpeed;

            // Fraction of journey completed equals current distance divided by total distance.
            float fractionOfJourney = distCovered / _lerpLength;

            // Set our position as a fraction of the distance between the markers.
            transform.position = Vector3.Lerp(startPosition, endPosition, fractionOfJourney);
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, fractionOfJourney);

            return fractionOfJourney;
        }
    }
}
