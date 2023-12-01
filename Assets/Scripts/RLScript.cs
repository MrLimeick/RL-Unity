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
#if !UNITY_EDITOR
        /// <summary>
        /// Папка где находится exe файл с игрой
        /// </summary>
        public static DirectoryInfo GameDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
#else
        /// <summary>
        /// Папка где находится exe файл с игрой
        /// </summary>
        public static DirectoryInfo GameDirectory = new(Environment.CurrentDirectory);
#endif
        /// <summary>
        /// Папка где находятся все созданные карты игрока
        /// </summary>
        /// <remarks>По умолчанию - Rhythm Lines/Created maps</remarks>
        public static DirectoryInfo CreatedMapsDirectory = new(GameDirectory.FullName + "/Created maps");
        /// <summary>
        /// Папка где находятся все карты 
        /// </summary>
        /// <remarks>По умолчанию - Rhythm Lines/Maps</remarks>
        public static DirectoryInfo MapsDirectory = new(GameDirectory.FullName + "/Maps");
        /// <summary>
        /// Папка где находятся все скины 
        /// </summary>
        /// <remarks>По умолчанию - Rhythm Lines/Skins</remarks>
        public static DirectoryInfo SkinsDirectory = new(GameDirectory.FullName + "/Skins");


    }
}
