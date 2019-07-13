using UniRx;
using UnityEngine;

namespace TurkeySpinner.Turkey
{
    public class AnimatorSpeedChanger : MonoBehaviour
    {
        [SerializeField]
        Rigidbody rb;

        [SerializeField]
        Animator animator;

        [SerializeField]
        float velocitySpeedRate;

        void Start()
        {
            rb.ObserveEveryValueChanged(rb => rb.angularVelocity)
                .Subscribe(v => animator.SetFloat("Speed", v.y * velocitySpeedRate))
                .AddTo(this);
        }
    }
}
