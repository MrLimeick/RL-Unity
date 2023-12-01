using System.IO;
using UnityEngine;

namespace RL
{
    /// <summary>
    /// Хранилище всей информации об карте
    /// </summary>
    [System.Serializable] public class Card
    {
        /// <summary>
        /// Название трека
        /// </summary>
        [SerializeField] public string Name;
        /// <summary>
        /// Название сложности
        /// </summary>
        [SerializeField] public string Difficulty;
        /// <summary>
        /// Создатели трека
        /// </summary>
        [SerializeField] public string Artits; 
        /// <summary>
        /// Создатели карты
        /// </summary>
        [SerializeField] public string Creators;
        /// <summary>
        /// Относительный путь до файл с треком
        /// </summary>
        [SerializeField] public string AudioFilePath;
        /// <summary>
        /// Относительный путь до файла с заднем фоном
        /// </summary>
        [SerializeField] public string BackgroundFilePath;
        /// <summary>
        /// Файл с изображением заднего фона
        /// </summary>
        public FileInfo BackgroundFile 
        {
            get 
            {
                if (FileInfo != null && FileInfo.Exists)
                    return new FileInfo(FileInfo.Directory.FullName + "/" + BackgroundFilePath);
                else if (File.Exists(BackgroundFilePath))
                    return new FileInfo(BackgroundFilePath);
                else return null;
            }
            set => BackgroundFilePath = value.FullName; 
        }
        /// <summary>
        /// Файл с треком уровня
        /// </summary>
        public FileInfo AudioFile 
        { 
            get
            {
                if (FileInfo != null && FileInfo.Exists)
                    return new FileInfo(FileInfo.Directory.FullName + "/" + AudioFilePath);
                else if (File.Exists(AudioFilePath))
                return new FileInfo(AudioFilePath); 
                else return null;
            }
            set => AudioFilePath = value.FullName;
        }
        /// <summary>
        /// FileInfo данного RLM файла
        /// </summary>
        public FileInfo FileInfo;
        /// <summary>
        /// Путь игрока
        /// </summary>
        public Paths.Path PlayerPath; 
    }
}
