using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RL.Paths;
using RL.Math;

namespace RL.Game
{
    internal class PathBuilder : MonoBehaviour
    {
        [SerializeField] private GameObject Line,Circle;
        public void Load(Path Path)
        {
            var lines = Path.GetLinesArray();

            foreach(var line in lines)
            {
                var Parts = line.GetPartsOfLine();

                foreach(var part in Parts)
                {
                    GameObject LineGameObject = Instantiate(Line);

                    Maths.GetLineTransform(part.A.position, part.B.position, out Vector3 Position, out float Lenght, out float Angle);
                    LineGameObject.transform.SetPosRotScale(Position, Quaternion.Euler(0, 0, Angle), new Vector2(0.25f, Lenght));

                    Instantiate(Circle).transform.position = part.A.position;
                }
            }
            Instantiate(Circle).transform.position = lines[^1].GetPartsOfLine()[^1].B.position;
        }
    }
}
