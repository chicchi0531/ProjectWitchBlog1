using UnityEngine;
using System.Collections;

namespace ProjectWitch.Menu
{
    public class BaseMenu:MonoBehaviour
    {
        //コントローラへの参照
        [SerializeField]
        protected MenuController mController = null;

        //トップメニューへの参照
        [SerializeField]
        protected TopMenu mTopMenu = null;

        //component 
        protected Animator mcAnim = null;

        //閉じれるかどうか
        public bool Closable { get; set; }

        // Use this for initialization
        protected virtual void Awake()
        {
            mcAnim = GetComponent<Animator>();
            Closable = true;
        }

        protected virtual void Start()
        {
            
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (mcAnim.GetBool("IsShow") && Closable && mController.InputEnable)
            {
                if (Input.GetButtonDown("Cancel"))
                {
                    Close();
                }
            }
        }

        public virtual void Open()
        {
            mcAnim.SetBool("IsShow", true);
        }

        public virtual void Close()
        {
            StartCoroutine(_Close());
        }

        protected virtual IEnumerator _Close()
        {
            mcAnim.SetBool("IsShow", false);
            yield return new WaitForSeconds(0.2f);
            mTopMenu.Open();
            yield return new WaitForSeconds(0.2f);

        }
    }
}
