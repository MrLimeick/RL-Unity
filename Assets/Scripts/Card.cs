using System;
using System.IO;
using UnityEngine;

namespace RL
{
    public interface ICard
    {
        /// <summary>
        /// Название трека
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Название карточки
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Создатели трека
        /// </summary>
        public string Artits { get; set; }

        /// <summary>
        /// Создатели карты
        /// </summary>
        public string Creators { get; set; }

        /// <summary>
        /// Относительный путь до файл с треком
        /// </summary>
        public string AudioFilePath { get; set; }

        /// <summary>
        /// Относительный путь до файла с заднем фоном
        /// </summary>
        public string BackgroundFilePath { get; set; }
    }

    /// <summary>
    /// Хранилище всей информации об карте
    /// </summary>
    [Serializable] public class Card : ICard
    {
        [field: SerializeField] public string Title { get; set; } = "Untitled";
        [field: SerializeField] public string Name { get; set; } = "Easy";
        [field: SerializeField] public string Artits { get; set; } = "Undefined";
        [field: SerializeField] public string Creators { get; set; } = "Undefined";
        [field: SerializeField] public float BPM { get; set; } = 80;

        public string AudioFilePath { get; set; }
        public string BackgroundFilePath { get; set; }

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
        public FileInfo FileInfo { get; set; }
        /// <summary>
        /// Путь игрока
        /// </summary>
        public Paths.Path PlayerPath { get; set; }

        public override string ToString()
            => $"{Artits} - {Title} ({Name} by {Creators})";
    }
}
