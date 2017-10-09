using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch.Menu.Tips
{
    public class TipsMenu : BaseMenu
    {
        [SerializeField]
        private List mList = null;

        public override void Open()
        {
            //リストを初期化
            mList.Init();

            base.Open();
        }
    }

}