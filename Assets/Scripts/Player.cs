using System;
using System.Collections;
using RL.Paths;
using UnityEngine;

namespace RL.Game
{
    /// <summary>
    /// Скрипт игрока
    /// </summary>
    public class Player : MonoBehaviour
    {
        private Coroutine MoveHandle;

        public bool Moved { get; protected set; } = false;

        public SpriteRenderer Sprite;

        public void Hit()
        {
            Sprite.color = Color.yellow;
        }

        private void Update()
        {
            Sprite.color = Color.Lerp(Sprite.color, Color.white, Time.deltaTime * 6f);
        }

        public void Move(IReadOnlyPath path, Func<float> getTime)
        {
            if(path == null)
            {
                Debug.LogError("Player path is null!");
                return;
            }

            transform.position = Vector2.one;
            transform.localScale = Vector2.one;

            MoveHandle = StartCoroutine(MoveCoroutine(path, getTime));
        }

        public void Move(IReadOnlyPath path)
        {
            float startTime = Time.time;
            float getTime() => Time.time - startTime;

            Move(path, getTime);
        }

        public void Stop()
        {
            StopCoroutine(MoveHandle);
            Moved = false;

            transform.position = Vector2.zero;
            transform.localScale = Vector2.zero;
        }

        IEnumerator MoveCoroutine(IReadOnlyPath path, Func<float> getTime)
        {
            if (path.Duration <= 0)
            {
                Debug.LogError("Длинна пути 0 секунд, движение невозможно.");
                yield break; // Проверка на наличее более двух точек в пути
            }

            Debug.Log("Начато движение по пути");
            Moved = true;

            foreach (Vector2 pos in path.GetPositions(getTime))
            {
                //if (localTime < 1) transform.localScale = new(localTime, localTime);
                //else if (path.Duration - localTime < 1)
                //{
                //    float s = path.Duration - localTime;
                //    transform.localScale = new(s, s);
                //}
                //else transform.localScale = Vector3.one;

                transform.position = pos;

                yield return new WaitForEndOfFrame();
            }

            transform.localScale = transform.position = Vector3.zero;

            Debug.Log("Движение по пути законченно");
            Moved = false;
        }
    }
}