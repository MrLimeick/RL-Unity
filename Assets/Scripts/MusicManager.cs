﻿using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace RL
{
    public class MusicManager
    {
        /// <summary>
        /// Получить трек по пути к фалу
        /// </summary>
        /// <param name="Path">Путь к файлу</param>
        /// <param name="Clip">Клип полученный из файла</param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public async static Task<AudioClip> LoadAudio(string path)
        {
            if (!File.Exists(path)) throw new Exception("Файла, который должен был является музыкой, не существует!");
            using UnityWebRequest UWR = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.UNKNOWN);

            var a = UWR.SendWebRequest();
            while (!a.isDone) await Task.Yield();

            if (UWR.result == UnityWebRequest.Result.Success)
            {
                AudioClip AC = DownloadHandlerAudioClip.GetContent(UWR);
                Debug.Log("Музыка была успешно загружена!");
                return AC;
            }
            else
                throw new Exception("Музыка не-была загружена из-за ошибки: " + UWR.error);
        }
    }
}
