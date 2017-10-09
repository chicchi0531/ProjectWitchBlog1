using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch.Menu
{
    public class AllUpLevel : AllUpWindowBase
    {
        public override void Yes()
        {
            var game = Game.GetInstance();

            //領地内全ユニットの取得
            var units = game.GameData.Group[game.GameData.Territory[0].GroupList[0]].UnitList;

            //ユニットリストにある全ユニットをレベルアップ
            foreach(var unit in units)
            {
                var data = game.GameData.Unit[unit];
                var upLevel = (int)(data.Experience / UnitDataFormat.REQUIPRED_EXPERIENCE_TO_LVUP);
                if (data.MaxLevel != -1) upLevel = Mathf.Min(data.MaxLevel - data.Level, upLevel);

                //レベルアップ
                data.Level += upLevel;

                //経験値リセット
                data.Experience -= upLevel * UnitDataFormat.REQUIPRED_EXPERIENCE_TO_LVUP;

                //HP回復
                data.HP = data.MaxHP;
                
            }

            Close();
        }
    }
}