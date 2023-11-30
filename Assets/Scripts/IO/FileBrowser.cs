using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
//using System.Windows.Forms;
using UnityEngine;
using UnityEngine.Networking;

//namespace RL.IO
//{
//    public class FileBrowser
//    {
//        /// <summary>
//        /// Открыть файл
//        /// </summary>
//        /// <returns>Возращает NULL если пользователь не чего не выбрал</returns>
//        public static FileInfo OpenFile(string Title,params ExtensionFilter[] filters)
//        {
//            var fileContent = string.Empty;
//            var filePath = string.Empty;

//            using (OpenFileDialog openFileDialog = new OpenFileDialog())
//            {
//                string Filter = filters.Length > 0 ? filters[0].ToString() : "All Files(*.*)|*.*";
//                for (int i = 1; i < filters.Length; i++)
//                {
//                    Filter += "|" + filters[i].ToString();
//                }
//                openFileDialog.Filter = Filter;
//                openFileDialog.FilterIndex = 1;
//                openFileDialog.RestoreDirectory = true;
//                openFileDialog.Title = Title;

//                if (openFileDialog.ShowDialog() == DialogResult.OK)
//                {
//                    filePath = openFileDialog.FileName;

//                    return new FileInfo(filePath);
//                }
//            }
//            return null;
//        }
//    }
//    public struct ExtensionFilter
//    {
//        public string Name;
//        public string[] Extensions;

//        public ExtensionFilter(string filterName, params string[] filterExtensions)
//        {
//            Name = filterName;
//            Extensions = filterExtensions;
//        }
//        public override string ToString()
//        {
//            string filters = "|*." + Extensions[0];
//            for(int i = 1; i < Extensions.Length; i++)
//            {
//                filters += ";*." + Extensions[i];
//            }
//            return Name + filters;
//        }
//    }

public class RLIO
{
    public static async Task<Sprite> GetSpriteFromPathAsync(string path)
    {
        var bytes = await File.ReadAllBytesAsync(path);
        var texture = new Texture2D(2, 2);
        texture.LoadImage(bytes);
        return Sprite.Create(texture, new(0, 0, texture.width, texture.height), new(.5f, .5f));
    }
    public static async Task<AudioClip> GetAudioClipFromPathAsync(string Path, AudioType type = AudioType.UNKNOWN)
    {
        if (!File.Exists(Path)) throw new Exception("Трека не существует");
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("File://" + new FileInfo(Path).FullName, type))
        {
            var AO = www.SendWebRequest();
            while (!AO.isDone) await Task.Yield();

            if (www.result == UnityWebRequest.Result.Success)
            {
                return await Task.FromResult(DownloadHandlerAudioClip.GetContent(www));
            }
            else
            {
                throw new Exception("Не удалось загрузить трек, ошибка: " + www.error);
            }
        }
    }
}
//}
