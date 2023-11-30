using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System.IO;

namespace RL
{
    public class MusicManager
    {
        public static MusicManager Current;

        /// <summary>
        /// Получить трек по пути к фалу
        /// </summary>
        /// <param name="Path">Путь к файлу</param>
        /// <param name="Clip">Клип полученный из файла</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public async static Task LoadAudioCourutine(string Path, UnityAction<AudioClip> Clip)
        {
            if (File.Exists(Path)) throw new Exception("Файла, который должен был является музыкой, не существует!");
            using (UnityWebRequest UWR = UnityWebRequestMultimedia.GetAudioClip(Path, AudioType.UNKNOWN))
            {
                var a = UWR.SendWebRequest();
                while (!a.isDone)
                    await Task.Yield();

                if (UWR.result == UnityWebRequest.Result.Success)
                {
                    AudioClip AC = DownloadHandlerAudioClip.GetContent(UWR);
                    Debug.Log("Музыка была успешно загружена!");
                    Clip.Invoke(AC);
                }
                else
                {
                    throw new Exception("Музыка не-была загружена из-за ошибки: " + UWR.error);
                }
            }
        }
    }
}
