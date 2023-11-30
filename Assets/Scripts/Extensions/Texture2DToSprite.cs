using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RL
{
    public static class Texture2DExtension
    {
        public static Sprite ToSprite(this Texture2D texture) => Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new(.5f, .5f));
    }
}
