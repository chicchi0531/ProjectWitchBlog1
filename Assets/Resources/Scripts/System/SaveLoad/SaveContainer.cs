using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Sys
{
    public class SaveContainer : SaveLoadContainerBase
    {
        //セーブを実行
        public void Save()
        {
            var game = Game.GetInstance();
            game.GameData.Save(mFileIndex);
            game.SystemData.Save();

            Reset();
        }
    }
}