using System.Linq;

namespace ProjectWitch.Shop.Magic
{
    public class SellList : BaseList
    {
        public override void Reset()
        {
            base.Reset();

            var game = Game.GetInstance();

            //子供を追加
            for (int i = 0; i < game.GameData.Card.Count; i++)
            {
                var item = game.GameData.Card[i];
                var itemList = game.GameData.Group[game.GameData.Territory[0].GroupList[0]].CardList;
                
                //個数を取得
                var num = (itemList.Where(x => x == i).ToList()).Count;

                //所持していない場合は表示しない
                if (num == 0) continue;

                //リストのアイテムを生成
                var inst = Instantiate(mListContent);
                var cp = inst.GetComponent<SellItem>();
                cp.ItemID = i;
                cp.InfoWindow = mInfoWindow;
                cp.MesBox = mMessageBox;
                cp.Reset();

                inst.transform.SetParent(mListContentParent.transform, false);
            }
        }
    }
}