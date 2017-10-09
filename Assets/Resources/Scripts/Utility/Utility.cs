using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectWitch.Extention
{
    //乱数関係の拡張
    public static class RandomEx
    {
        static Random _Rand = new Random();

        //リストからランダムに要素を取得
        public static T Random<T>(this List<T> list)
        {
            return list.ElementAt(_Rand.Next(list.Count()));
        }

        //あるリストからn個分ランダムに取り出す
        //リストの要素数<nの場合はリストの要素個分ランダムに取り出す
        public static List<T> RandomN<T>(this List<T> list, int n)
        {
            //ランダム配列を作成
            var dummy = Enumerable.Range(0,list.Count);
            dummy = dummy.OrderBy(i => Guid.NewGuid());
            var indexList = dummy.ToList();

            //リスト要素数 < nの場合、出力のリストサイズを縮小
            int outListSize = Math.Min(list.Count, n);

            //ランダム配列の要素をインデックスとして、n個分取り出す
            var outList = new List<T>();
            for (int i=0; i<outListSize; i++)
            {
                outList.Add(list[indexList[i]]);
            }
            return outList;
        }

        //リストをシャッフルする
        public static List<T> Shuffle<T>(this List<T> list)
        {
            return list.OrderBy(i => Guid.NewGuid()).ToList();
        }

        //順番にN個の要素を抜き出す
        //リストの要素数<nとなるときは、リストの要素数に合わせる
        public static List<T> GetOrderN<T>(this List<T> list, int n)
        {
            var outListSize = Math.Min(list.Count, n);
            return list.GetRange(0, outListSize);
        }

        //インデックスリストを引数に渡し、そのインデックスに対応する要素のリストを作成する
        public static List<T> GetFromIndex<T>(this List<T> list, List<int> indexList)
        {
            var outList = new List<T>();
            foreach(var index in indexList)
            {
                outList.Add(list[index]);
            }

            return outList;
        }
    }

    //Dictionary<T>の拡張
    public static class DictionaryEX
    {
        public static byte[] GetBytes(this Dictionary<int,int> dict)
        {
            List<byte> outData = new List<byte>();

            outData.AddRange(BitConverter.GetBytes(dict.Count));

            foreach(var value in dict)
            {
                outData.AddRange(BitConverter.GetBytes(value.Key));
                outData.AddRange(BitConverter.GetBytes(value.Value));
            }

            return outData.ToArray();
        }
    }

    //List<T>の拡張
    public static class ListEX
    {
        public static bool Contains(this List<int> list, List<int> compareList)
        {
            foreach(var obj in compareList)
            {
                if (list.Contains(obj))
                    return true;
            }
            return false;
        }

        public static byte[] GetBytes(this List<int> list)
        {
            List<byte> outdata = new List<byte>();

            foreach (var item in list)
            {
                outdata.AddRange(BitConverter.GetBytes(item));
            }

            return outdata.ToArray();
        }
        public static byte[] GetBytes(this List<float> list)
        {
            List<byte> outdata = new List<byte>();

            foreach (var item in list)
            {
                outdata.AddRange(BitConverter.GetBytes(item));
            }

            return outdata.ToArray();
        }

        public static byte[] GetBytes(this List<List<int>> list)
        {
            List<byte> outData = new List<byte>();

            foreach(var item in list)
            {
                outData.AddRange(BitConverter.GetBytes(item.Count));
                outData.AddRange(item.GetBytes());
            }

            return outData.ToArray();
        }

        public static byte[] GetBytes<T>(this List<T> list)
            where T : ISaveableData
        {
            var outdata = new List<byte>();

            foreach (var item in list)
            {
                outdata.AddRange(item.GetSaveBytes());
            }

            return outdata.ToArray();
        }
    }

    public static class EnumConverter
    {
        public static T ToEnum<T>(int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }
    }

    public static class MathEx
    {
        //ある値を、min以上max以下の値に丸めるメソッド
        public  static float Saturate(float source, float min, float max)
        {
            return (source < min) ? min : (source > max ? max : source);
        }
        public static int Saturate(int source, int min, int max)
        {
            return (source < min) ? min : (source > max ? max : source);
        }
    }
}