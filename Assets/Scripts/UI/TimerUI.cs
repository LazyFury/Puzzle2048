using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Puzzle2048
{
    public class TimerUI : MonoBehaviour
    {
        public   UnityEvent<string> onTimerUpdate;
        [SerializeField] float time;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            time = Time.time;
            var m = Mathf.FloorToInt(time / 60);
            var s = Mathf.FloorToInt(time % 60);
            onTimerUpdate?.Invoke(string.Format("Time: {0:00}:{1:00}", m, s));
        }

        public void ClearTimer()
        {
            onTimerUpdate?.Invoke("Time: 00:00");
            time = 0;
        }
    }
}
