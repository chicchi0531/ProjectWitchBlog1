using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Menu
{
    public class BaseInfo : MonoBehaviour
    {
        [SerializeField]
        private GameObject mPanel = null;

        public int ID { get; set; }

        // Use this for initialization
        void Start()
        {
            mPanel.SetActive(false);
        }

        // Update is called once per frame
        public virtual void Init()
        {
            if (ID == -1) mPanel.SetActive(false);
            else mPanel.SetActive(true);
        }
    }
}