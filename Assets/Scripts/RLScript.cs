using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

namespace RL
{
    /// <summary>
    /// Главный скрипт для всеx Rhythm Lines скриптам
    /// </summary>
    public class RLScript : MonoBehaviour
    {
        /// <summary>
        /// Возращает запущенную версию игры
        /// </summary>
        public static string GameVersion { get => Application.version; }
        /// <summary>
        /// Игровой курсор
        /// </summary>
        public static Cursor Cursor;
        public static string m_Username = "Username";
        /// <summary>
        /// Ник игрока
        /// </summary>
        public static string Username { get => m_Username; set => m_Username = value; }


#if !UNITY_EDITOR
        /// <summary>
        /// Папка где находится exe файл с игрой
        /// </summary>
        public static DirectoryInfo GameDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
#else
        /// <summary>
        /// Папка где находится exe файл с игрой
        /// </summary>
        public static DirectoryInfo GameDirectory = new DirectoryInfo(Environment.CurrentDirectory);
#endif
        /// <summary>
        /// Папка где находятся все созданные карты игрока
        /// </summary>
        /// <remarks>По умолчанию - Rhythm Lines/Created maps</remarks>
        public static DirectoryInfo CreatedMapsDirectory = new DirectoryInfo(GameDirectory.FullName + "/Created maps");
        /// <summary>
        /// Папка где находятся все карты 
        /// </summary>
        /// <remarks>По умолчанию - Rhythm Lines/Maps</remarks>
        public static DirectoryInfo MapsDirectory = new DirectoryInfo(GameDirectory.FullName + "/Maps");
        /// <summary>
        /// Папка где находятся все скины 
        /// </summary>
        /// <remarks>По умолчанию - Rhythm Lines/Skins</remarks>
        public static DirectoryInfo SkinsDirectory = new DirectoryInfo(GameDirectory.FullName + "/Skins");


    }
}
