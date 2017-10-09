using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Menu.Item
{
    public class ItemMenu : BaseMenu
    {
        [SerializeField]
        private List mList = null;

        [SerializeField]
        private EquipmentInfo mEquipmentInfo = null;
        [SerializeField]
        private CardInfo mCardInfo = null;

        protected override IEnumerator _Close()
        {
            //リストを非表示
            mList.gameObject.SetActive(false);
            HideInfo();

            yield return StartCoroutine(base._Close());

        }

        private void HideInfo()
        {
            mEquipmentInfo.ID = -1;
            mEquipmentInfo.Init();
            mCardInfo.ID = -1;
            mCardInfo.Init();
        }

        public void ShowEquipment()
        {
            mList.gameObject.SetActive(true);
            HideInfo();
            mList.IsEquipment = true;
            mList.Init();
        }

        public void ShowCard()
        {
            mList.gameObject.SetActive(true);
            HideInfo();
            mList.IsEquipment = false;
            mList.Init();
        }
    }
}