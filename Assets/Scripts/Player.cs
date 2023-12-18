using System.Collections;
using UnityEngine;
using RL.Paths;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace RL.Game
{
    /// <summary>
    /// Скрипт игрока
    /// </summary>
    public class Player : MonoBehaviour
    {
        public IReadOnlyPath Path;

        private Coroutine MoveHandle;

        public bool Moved { get; protected set; } = false;

        public void Move()
        {
            if(Path == null)
            {
                Debug.LogError("Player path is null!");
                return;
            }

            MoveHandle = StartCoroutine(MoveCoroutine());
        }

        public void Stop()
        {
            StopCoroutine(MoveHandle);
            Moved = false;

            transform.position = Vector2.zero;
            transform.localScale = Vector2.zero;
        }

        IEnumerator MoveCoroutine()
        {
            IReadOnlyPath path = Path;

            if (path.Duration <= 0)
            {
                Debug.LogError("Длинна пути 0 секунд, движение невозможно.");
                yield break; // Проверка на наличее более двух точек в пути
            }

            Debug.Log("Начато движение по пути");
            Moved = true;
            float StartTime = Time.time;

            float localTime = 0;
            float getTime()
            {
                localTime = Time.time - StartTime;
                return localTime;
            }

            foreach(var pos in path.GetPositions(getTime))
            {
                if (localTime < 1) transform.localScale = new(localTime, localTime);
                else if (path.Duration - localTime < 1)
                {
                    float s = path.Duration - localTime;
                    transform.localScale = new(s, s);
                }
                else transform.localScale = Vector3.one;

                transform.position = pos;
                yield return new WaitForEndOfFrame();
            }

            transform.localScale = transform.position = Vector3.zero;

            Debug.Log("Движение по пути законченно");
            Moved = false;
        }
    }
}