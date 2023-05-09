using UnityEngine;

namespace Scripts.Extensions
{
    public static class SpriteRendererExtension
    {
        public static void ChangeAlpha(this SpriteRenderer spriteRenderer, float value)
        {
            Color color = spriteRenderer.color;
            color.a = value;
            spriteRenderer.color = color;
        }
    }
}