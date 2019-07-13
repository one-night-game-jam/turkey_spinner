using UniRx;
using UnityEngine;

namespace TurkeySpinner.Turkey
{
    public class PitchChanger : MonoBehaviour
    {
        [SerializeField]
        AudioSource audioSource;

        [SerializeField]
        Rigidbody rb;

        [SerializeField]
        float velocityPitchRate;

        void Start()
        {
            rb.ObserveEveryValueChanged(rb => rb.angularVelocity)
                .Subscribe(v => audioSource.pitch = v.y * velocityPitchRate)
                .AddTo(this);
        }
    }
}
