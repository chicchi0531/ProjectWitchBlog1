using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectWitch.Sys;

namespace ProjectWitch.Menu.System
{
    public class HilightBoxLoadReciever : HilightBoxSaveReciever
    {
        // Update is called once per frame
        void Update()
        {
            if(SelectedIndex == -1)
            {
                mcText.text = "オートセーブファイルをロードします。\nよろしいですか？";
                mcButton.interactable = true;
            }
            else if (SelectedIndex >= 0)
            {
                mcText.text = SelectedIndex.ToString() + "番のファイルをロードします。\nよろしいですか？";
                mcButton.interactable = true;
            }
            else
            {
                mcText.text = "ロードするファイルを選択して下さい。";
                mcButton.interactable = false;
            }

        }

        //セーブを実行
        public void Load()
        {
            if (Container)
            {
                LoadContainer lContainer = (LoadContainer)Container;
                lContainer.Load();
            }
        }
    }
}