using UnityEngine;
using UnityEngine.UI;

using System.Collections;

namespace ProjectWitch.Menu
{
    public class UnemployWindow : MonoBehaviour
    {

        [SerializeField]
        private GameObject mPanel = null;
        [SerializeField]
        private ArmyMenu mArmyMenu = null;
        [SerializeField]
        public StatusWindow mStatusWindow = null;
        [SerializeField]
        public UnitList mList = null;

        //ID
        public int UnitID { get; set; }

        // Use this for initialization
        void Start()
        {
            UnitID = -1;
        }

        public void Show(int unitID)
        {
            UnitID = unitID;
            mArmyMenu.Closable = false;
            mPanel.SetActive(true);
        }

        public void OnClickOK()
        {
            var game = Game.GetInstance();
            game.GameData.Territory[0].RemoveUnit(UnitID);

            //ステータスウィンドウをリセット
            mStatusWindow.UnitID = -1;
            mStatusWindow.Init();

            //リストをリセット
            mList.Init();

            Close();
        }

        public void OnClickCancel()
        {
            Close();
        }

        private void Close()
        {
            mArmyMenu.Closable = true;
            mPanel.SetActive(false);
        }
    }
}