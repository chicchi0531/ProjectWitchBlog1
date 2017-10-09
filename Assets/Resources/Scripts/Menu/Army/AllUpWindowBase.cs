using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProjectWitch.Menu
{
    public class AllUpWindowBase : MonoBehaviour
    {
        [SerializeField]
        private ArmyMenu mMenu = null;

        [SerializeField]
        private GameObject mPanel = null;

        public virtual void Reset()
        {
            
        }

        public void Open()
        {
            mPanel.SetActive(true);
            mMenu.Closable = false;
            Reset();
        }

        public void Close()
        {
            mPanel.SetActive(false);
            mMenu.Closable = true;
        }

        public virtual void Yes()
        {
            Close();
        }

        public virtual void No()
        {
            Close();
        }
    }
}