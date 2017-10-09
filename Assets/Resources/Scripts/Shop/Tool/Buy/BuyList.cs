using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch.Shop.Tool
{
    public class BuyList : BaseList
    {

        public override void Reset()
        {
            base.Reset();

            var game = Game.GetInstance();
            
            //子供を追加
            for(int i=0; i< game.GameData.Equipment.Count; i++)
            {
                var inst = Instantiate(mListContent);
                var cp = inst.GetComponent<BuyItem>();
                cp.ItemID = i;
                cp.InfoWindow = mInfoWindow;
                cp.MesBox = mMessageBox;
                cp.Reset();

                inst.transform.SetParent(mListContentParent.transform,false);
            }
        }
    }
}