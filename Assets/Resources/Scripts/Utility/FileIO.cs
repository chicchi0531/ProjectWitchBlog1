using System.Linq;
using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ProjectWitch
{

    //セーブを想定したデータ
    public abstract class ISaveableData
    {
        //セーブするためのデータをbyte配列にパックして取得
        public abstract byte[] GetSaveBytes();

        //バイト配列からデータを再現
        //@offset　何バイト目から読み出し始めるか
        //@return　何バイト読み込んだか、引数のoffsetと合計した数値を返す
        public abstract int SetFromBytes(int offset, byte[] data);

    }
    
    //セーブ用のメタデータ
    public class SaveMetaData
    {
        private byte _major = 0;
        private byte _minor = 0;
        public byte Major { get { return _major; } set { _major = value; } }
        public byte Minor { get { return _minor; } set { _minor = value; } }

        public virtual int GetSize()
        {
            var size = 0;
            size += Marshal.SizeOf(Major);
            size += Marshal.SizeOf(Minor);

            return size;
        }

        public virtual byte[] GetSaveBytes()
        {
            return new byte[1];
        }

        public virtual void SetFromBytes(byte[] data)
        {

        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            SaveMetaData v = (SaveMetaData)obj;
            return (Major == v.Major) && (Minor == v.Minor);
        }

        public override string ToString()
        {
            return " Major:" + Major.ToString() +
                   " Minor:" + Minor.ToString();
        }

        public override int GetHashCode()
        {
            return Major ^ Minor;
        }
    }

    static class FileIO
    {
        //任意のオブジェクトをxmlにシリアライズしてファイルに保存
        public static void SaveBinary(string filepath, SaveMetaData meta, ISaveableData data)
        {
            //ディレクトリが存在しない場合はディレクトリを作成する
            var dirPath = Path.GetDirectoryName(filepath);
            if (!Directory.Exists(dirPath))
            {
                try
                {
                    Directory.CreateDirectory(dirPath);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }

            using (FileStream fs = new FileStream(filepath, FileMode.Create))
            {
                try
                {
                    //メタデータの書き込み
                    byte[] buffer = meta.GetSaveBytes();
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Flush();

                    //データの書き込み
                    buffer = data.GetSaveBytes();
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Flush();
                }
                catch (Exception e)
                {
                    Debug.LogError("SaveableDataToBinary is Failed:" +
                        " Message :" + e.Message);
                }
            }
        }

        //任意のオブジェクトをファイルのxmlからデシリアライズする
        public static bool LoadBinary<T1,T2>(string filepath, ref T1 meta, T2 data)
            where T1 : SaveMetaData, new()
            where T2 : ISaveableData 
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                var metaData = new T1();

                //メタデータの読み出し
                byte[] buffer = new byte[metaData.GetSize()];
                fs.Read(buffer, 0, buffer.Length);
                metaData.SetFromBytes(buffer);

                //バージョンチェック
                if (metaData.Major != meta.Major)
                {
                    Debug.LogError("セーブファイルのバージョンが違います");
                    return false;
                }
                meta = metaData;

                //データの読み出し
                buffer = new byte[fs.Length-meta.GetSize()];
                fs.Read(buffer, 0, buffer.Length);
                data.SetFromBytes(0, buffer);

                return true;
            }
        }

        //任意のロードファイルのメタ情報を読み出す
        public static void LoadMetaData(string filepath, SaveMetaData meta)
        {
            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                //メタデータの読み出し
                byte[] buffer = new byte[meta.GetSize()];
                fs.Read(buffer, 0, buffer.Length);
                meta.SetFromBytes(buffer);
            }
        }

        ////ISavableDataを実装するオブジェクトをバイナリで書き出す
        //private static void SaveableDataToBibary<T>(Stream stream, T data)
        //    where T : ISaveableData
        //{
        //    try
        //    {
        //        byte[] buffer = data.GetSaveBytes();
        //        stream.Write(buffer, 0, buffer.Length);
        //        stream.Flush();
        //    }
        //    catch(Exception e)
        //    {
        //        Debug.LogError("SaveableDataToBinary is Failed:" +
        //            " Message :" + e.Message);
        //    }
        //}

        ////ISaveableDataを実装するオブジェクトに、バイナリファイルからデータを読みだす
        //private static void SaveableDataFromBinary<T>(Stream stream, SaveMetaData version, ref T data)
        //    where T : ISaveableData
        //{

        //    byte[] buffer = new byte[stream.Length];
        //    stream.Read(buffer, 0, buffer.Length);
        //    data.SetFromBytes(0, buffer);
        //}

    }
}