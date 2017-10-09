using UnityEngine;
using System.Collections;

namespace ProjectWitch.Menu
{
    public class ArmyMenu : BaseMenu
    {
        //ユニットリストへの参照
        [SerializeField]
        private UnitList mUnitList = null;
        public UnitList UnitList { get { return mUnitList; }  private set { } }

        //ステータスウィンドウへの参照
        [SerializeField]
        private StatusWindow mStatusWindow = null;

        protected override IEnumerator _Close()
        {
            //ステータスウィンドウを閉じる
            mStatusWindow.UnitID = -1;
            mStatusWindow.Init();
            
            yield return StartCoroutine(base._Close());

        }
    }
}