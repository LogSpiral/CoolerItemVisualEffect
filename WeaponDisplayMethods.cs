using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolerItemVisualEffect
{
    internal static class WeaponDisplayMethods
    {
        //public static Vector2 Projectile(this Vector3 vec, float height)
        //{
        //    return height / (height - vec.Z) * new Vector2(vec.X, vec.Y);
        //}
        //public static void DrawLine(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float width = 2)
        //{
        //    spriteBatch.Draw(TextureAssets.MagicPixel.Value, (start + end) / 2f - Main.screenPosition, new Rectangle(0, 0, 1, 1), color, (end - start).ToRotation(), new Vector2(0.5f, 0.5f), new Vector2((end - start).Length(), width), 0, 0);
        //}
        public static float Lerp(this float t, float from, float to, bool clamp = false)
        {
            if (clamp) t = MathHelper.Clamp(t, 0, 1);
            return (1 - t) * from + t * to;
        }
        //public static T Lerp<T>(this float t, T from, T to,bool clamp = false) 
        //{
        //    if (clamp) t = MathHelper.Clamp(t, 0, 1);
        //    return (1 - t) * from + t * to;
        //}

        //public static void DrawPath(this SpriteBatch spriteBatch, Func<float, Vector2> vectorFunc, Func<float, Color> colorFunc, Effect effect, Texture2D baseTex, Texture2D aniTex, Vector2 offest = default, int counts = 25, float min = 0, float max = 1, float width = 16, float kOfX = 1, bool looped = false, Func<float, float> factorFunc = null, Func<float, float> widthFunc = null, Func<float, float> lightFunc = null, Func<float> timeFunc = null, string pass = default, Action<Vector2> doSth = null, bool alwaysDoSth = false, bool beginImmediately = false)
        //{
        //    if (vectorFunc == null || colorFunc == null || effect == null || counts < 3)
        //    {
        //        return;
        //    }
        //    var positions = new Vector2[counts];
        //    var bars = new CustomVertexInfo[counts * 2];
        //    for (int n = 0; n < counts; n++)
        //    {
        //        var factor = (float)n / (counts - 1);
        //        if (factorFunc != null)
        //        {
        //            factor = factorFunc.Invoke(factor);
        //        }
        //        var position = vectorFunc.Invoke(factor.Lerp(min, max)) + offest;
        //        positions[n] = position;
        //        if ((!Main.gamePaused || alwaysDoSth) && doSth != null)
        //        {
        //            doSth.Invoke(position);
        //        }
        //    }
        //    for (int i = 0; i < counts; ++i)
        //    {
        //        var normalDir = i == 0 ? (looped ? positions[0] - positions[counts - 1] : positions[1] - positions[0]) : positions[i] - positions[i - 1];
        //        normalDir = Vector2.Normalize(new Vector2(-normalDir.Y, normalDir.X));
        //        var factor = i / (counts - 1f);
        //        if (factorFunc != null)
        //        {
        //            factor = factorFunc.Invoke(factor);
        //        }
        //        var color = colorFunc.Invoke(factor);
        //        var w = widthFunc == null ? width : widthFunc.Invoke(factor);
        //        var l = lightFunc == null ? factor : lightFunc.Invoke(factor);
        //        bars[2 * i] = (new CustomVertexInfo(positions[i] + normalDir * w, color, new Vector3(factor * kOfX, 1, l)));
        //        bars[2 * i + 1] = (new CustomVertexInfo(positions[i] + normalDir * -w, color, new Vector3(factor * kOfX, 0, l)));
        //    }
        //    List<CustomVertexInfo> triangleList = new List<CustomVertexInfo>();
        //    for (int i = 0; i < bars.Length - 2; i += 2)
        //    {
        //        triangleList.Add(bars[i]);
        //        triangleList.Add(bars[i + 2]);
        //        triangleList.Add(bars[i + 1]);
        //        triangleList.Add(bars[i + 1]);
        //        triangleList.Add(bars[i + 2]);
        //        triangleList.Add(bars[i + 3]);
        //    }
        //    spriteBatch.End();
        //    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        //    RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
        //    //RasterizerState rasterizerState = new RasterizerState();
        //    //rasterizerState.CullMode = CullMode.None;
        //    //rasterizerState.FillMode = FillMode.WireFrame;
        //    //Main.graphics.GraphicsDevice.RasterizerState = rasterizerState;
        //    Main.graphics.GraphicsDevice.RasterizerState.CullMode = CullMode.None;
        //    var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
        //    var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0));
        //    effect.Parameters["uTransform"].SetValue(model * projection);
        //    effect.Parameters["uTime"].SetValue(timeFunc == null ? -(float)Main.time * 0.03f : timeFunc.Invoke());
        //    Main.graphics.GraphicsDevice.Textures[0] = baseTex;
        //    Main.graphics.GraphicsDevice.Textures[1] = aniTex;
        //    Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
        //    Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
        //    if (pass != null) { effect.CurrentTechnique.Passes[pass].Apply(); } else { effect.CurrentTechnique.Passes[0].Apply(); }
        //    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, triangleList.ToArray(), 0, triangleList.Count / 3);
        //    Main.graphics.GraphicsDevice.RasterizerState = originalState;
        //    spriteBatch.End();
        //    spriteBatch.Begin(beginImmediately ? SpriteSortMode.Immediate : SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        //}
    }
}
