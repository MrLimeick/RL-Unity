using System.Collections;
using UnityEngine;
using RL.Paths;
using System.Threading.Tasks;
using System.Threading;

namespace RL.Game
{
    /// <summary>
    /// Скрипт игрока
    /// </summary>
    public class Player : MonoBehaviour
    {
        /// <summary>
        /// Экземпляр игрока на сцене
        /// </summary>
        public static Player Instance { get; private set; }
        public void Awake()
        {
            if (Instance != null) Destroy(Instance.gameObject);
            Instance = this;
        }
        /// <summary>
        /// Двигать игрока по пути
        /// </summary>
        /// <param name="path">Путь по которому будет двигаться игрок</param>
        /// <param name="ct">Токен для остановки</param>
        /// <returns></returns>
        public async Task Move(Path path,CancellationToken ct)
        {
            if (path.Count < 2) return; // Проверка на наличее более двух точек в пути
            while (!ct.IsCancellationRequested)
            {
                float OldTime = Time.time;

                while (Time.time - OldTime < path[^1].B.time)
                {
                    await Task.Yield();
                    transform.position = path.GetPosition(Time.time - OldTime);
                }
                transform.position = path.GetPosition(Time.time - OldTime);
                return;
            }
        }
    }
}