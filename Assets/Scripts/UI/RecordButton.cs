using UniRx;
using UnityEngine;
using UnityEngine.Apple.ReplayKit;
using UnityEngine.UI;

namespace TurkeySpinner.UI
{
    public class RecordButton : MonoBehaviour
    {
        [SerializeField]
        Toggle toggle;

        void Start()
        {
            toggle.OnValueChangedAsObservable()
                .Skip(1)
                .Subscribe(Changed)
                .AddTo(this);

            this.ObserveEveryValueChanged(_ => ReplayKit.isRecording)
                .Skip(1)
                .Subscribe(isRecording => toggle.isOn = isRecording)
                .AddTo(this);
        }

        void OnEnable()
        {
            gameObject.SetActive(ReplayKit.APIAvailable);
        }

        static void Changed(bool isOn)
        {
            if (isOn)
            {
                ReplayKit.StartRecording(true);
            }
            else
            {
                ReplayKit.StopRecording();
                ReplayKit.Preview();
            }
        }
    }
}
