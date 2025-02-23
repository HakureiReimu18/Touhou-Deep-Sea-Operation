using System.Collections.Immutable;
using Barotrauma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using TouhouGUI;

namespace TouhouGUI
{
    public static class TouhouGUIExtensions
    {
        public const int DonutSegments = 30;

        public static readonly VertexPositionColorTexture[] donutVerts = new VertexPositionColorTexture[DonutSegments * 4];

        public static readonly ImmutableArray<Vector2> canonicalCircle = Enumerable.Range(0, DonutSegments)
                                                                                   .Select(i => i * (2.0f * MathF.PI / DonutSegments))
                                                                                   .Select(angle => new Vector2(MathF.Cos(angle), MathF.Sin(angle)))
                                                                                   .ToImmutableArray();

        private static Vector2 GetDirection(int index, float sectionProportion, int maxDirectionIndex)
        {
            int directionIndex = GetDirectionIndex(index);
            Vector2 direction = canonicalCircle[directionIndex % DonutSegments];
            if (maxDirectionIndex > 0 && directionIndex >= maxDirectionIndex)
            {
                float maxSectionProportion = (float)maxDirectionIndex / DonutSegments;
                direction = Vector2.Lerp(
                    canonicalCircle[maxDirectionIndex - 1],
                    canonicalCircle[maxDirectionIndex % DonutSegments],
                    1.0f - (maxSectionProportion - sectionProportion) * DonutSegments);
            }

            return new Vector2(direction.Y, -direction.X);
        }

        private static int GetDirectionIndex(int index)
        {
            return (index % 4) switch
            {
                0 => (index / 4) + 0,
                1 => (index / 4) + 1,
                2 => (index / 4) + 0,
                3 => (index / 4) + 1,
                _ => throw new InvalidOperationException()
            };
        }

        private static float GetRadius(int index, ref Range<float> radii)
        {
            return (index % 4) switch
            {
                0 => radii.End,
                1 => radii.End,
                2 => radii.Start,
                3 => radii.Start,
                _ => throw new InvalidOperationException()
            };
        }

        private static Vector2 RotatePoint(Vector2 center, Vector2 vertexPosition, float rotationAngle)
        {
            float cosTheta = MathF.Cos(rotationAngle);
            float sinTheta = MathF.Sin(rotationAngle);

            float offsetX = vertexPosition.X - center.X;
            float offsetY = vertexPosition.Y - center.Y;

            float rotatedX = cosTheta * offsetX - sinTheta * offsetY;
            float rotatedY = sinTheta * offsetX + cosTheta * offsetY;

            return new Vector2(rotatedX + center.X, rotatedY + center.Y);
        }

        // ============ 修改部分开始 ============
        public static void DrawDonutSection(SpriteBatch sb, Vector2 center, Range<float> radii, float sectionRad, Color clr, float depth = 0.0f, float rotationAngle = 0.0f, bool clockwise = true)
        {
            float sectionProportion = sectionRad / (MathF.PI * 2.0f);
            int maxDirectionIndex = Math.Min(DonutSegments, (int)MathF.Ceiling(sectionProportion * DonutSegments));

            // 五色渐变定义
            Color[] gradientColors = new Color[] {
                GUIStyle.ColorInventoryEmpty,
                Color.Orange,
                GUIStyle.ColorInventoryHalf,
                Color.LimeGreen,
                GUIStyle.ColorInventoryFull
            };

            for (int vertexIndex = 0; vertexIndex < maxDirectionIndex * 4; vertexIndex++)
            {
                Vector2 direction = GetDirection(vertexIndex, sectionProportion, maxDirectionIndex);
                Vector2 vertexPosition = new(
                    center.X + (direction.X * GetRadius(vertexIndex, ref radii)),
                    center.Y + (direction.Y * GetRadius(vertexIndex, ref radii)));

                Vector2 rotatedPosition = RotatePoint(center, vertexPosition, rotationAngle);

                // 修改顶点颜色计算部分
                float angle = rotationAngle + (vertexIndex / (float)(maxDirectionIndex * 4)) * sectionRad;
                float angleProgress = angle / (MathHelper.TwoPi);
                angleProgress = (angleProgress % 1f + 1f) % 1f; // 确保在0-1范围内
                Color vertexColor = TouhouGUI.MultiColorGradientLerp(angleProgress, gradientColors);

                donutVerts[vertexIndex].Color = vertexColor;
                donutVerts[vertexIndex].Position = new Vector3(rotatedPosition, depth);
            }

            sb.Draw(GUI.WhiteTexture, donutVerts, depth, count: maxDirectionIndex);
        }
        // ============ 修改部分结束 ============

        public static void DrawDonutSectionOutLine(SpriteBatch sb, Vector2 center, Range<float> radii, float sectionRad, Color outlineclr, float depth = 0.0f, float rotationAngle = 0.0f)
        {
            float outlineWidth = 1.0f;
            //draw curved line
            DrawDonutSection(sb, center, new Range<float>(radii.Start - outlineWidth, radii.Start), sectionRad, outlineclr, depth, rotationAngle);
            DrawDonutSection(sb, center, new Range<float>(radii.End, radii.End + outlineWidth), sectionRad, outlineclr, depth, rotationAngle);
            //draw straight line
            Vector2 start1 = new(center.X + (radii.Start - outlineWidth) * MathF.Sin(rotationAngle), center.Y + (radii.Start - outlineWidth) * MathF.Cos(rotationAngle));
            Vector2 end1 = new(center.X + (radii.End + outlineWidth) * MathF.Sin(rotationAngle), center.Y + (radii.End + outlineWidth) * MathF.Cos(rotationAngle));
            GUI.DrawLine(sb, start1, end1, outlineclr, depth);
            Vector2 start2 = new(center.X + (radii.Start - outlineWidth) * MathF.Sin(rotationAngle + sectionRad), center.Y + (radii.Start - outlineWidth) * MathF.Cos(rotationAngle + sectionRad));
            Vector2 end2 = new(center.X + (radii.End + outlineWidth) * MathF.Sin(rotationAngle + sectionRad), center.Y + (radii.End + outlineWidth) * MathF.Cos(rotationAngle + sectionRad));
            GUI.DrawLine(sb, start2, end2, outlineclr, depth);
        }
    }
}