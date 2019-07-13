using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace TurkeySpinner.Turkey
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField]
        Rigidbody rb;

        [SerializeField]
        float initialTorque;

        void Start()
        {
            this.FixedUpdateAsObservable()
                .First()
                .Subscribe(_ => AddTorque(initialTorque))
                .AddTo(this);
        }

        void AddTorque(float y)
        {
            rb.angularVelocity = Vector3.zero;
            rb.AddTorque(0, y, 0, ForceMode.Impulse);
        }
    }
}
