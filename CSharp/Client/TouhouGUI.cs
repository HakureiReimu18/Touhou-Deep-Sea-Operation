using Barotrauma;
using Barotrauma.Items.Components;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace TouhouGUI
{
    public class TouhouGUI : IAssemblyPlugin
    {
        public readonly string Name = "TouhouGUI";

        // 删除以下HitHint相关属性
        // public static float HitHintTimer { get; set; }
        // public static int HitHintSize { get; set; } = 10;
        // public static Color HitHintColor { get; set; }
        // public static int CrosshairDistance { get; set; } = 10;

        public static float IndicatorRadiusStart { get; set; } = 40.0f;
        public static float IndicatorThickness { get; set; } = 2.0f;
        public static float IndicatorSectionRad { get; set; } = 2.0f * MathF.PI;
        public static float IndicatorRotationAngle { get; set; } = 2.0f * MathF.PI;

        public Harmony? harmonyInstance;

        public void Initialize()
        {
            LuaCsLogger.Log($"TouhouGUI loading...");
            harmonyInstance = new Harmony("Touhou.GUI.ThanksEsirprus");
        }

        public void Dispose()
        {
            harmonyInstance?.UnpatchAll();
            LuaCsLogger.Log("TouhouGUI disposed!");
        }

        public void OnLoadCompleted()
        {
            if (true)
            {
                harmonyInstance.PatchAll();
                LuaCsLogger.Log($"TouhouGUI loaded!");
            }
            else
            {
                harmonyInstance = null;
                LuaCsLogger.Log($"TouhouGUI has been disabled because other mod contains its function.");
            }
        }

        public void PreInitPatching() { }

        // 删除以下Harmony补丁类：
        // [HarmonyPatch(typeof(Character), nameof(Character.ApplyAttack))]
        // public static class ApplyAttackPatch { ... }

        // [HarmonyPatch(typeof(RangedWeapon), nameof(RangedWeapon.UpdateHUDComponentSpecific))]
        // public static class UpdateHUDComponentSpecificPatch { ... }

        // 修改后的DrawHUD补丁（移除DrawHint调用）
        [HarmonyPatch(typeof(RangedWeapon), nameof(RangedWeapon.DrawHUD))]
        static class RangedWeapon_DrawHUD_Patch
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                bool foundInsertionPoint = false;

                for (int i = 0; i < codes.Count; i++)
                {
                    // 查找插入点：当代码为 `this.crossHairPosDirtyTimer` 时
                    if (codes[i].opcode == OpCodes.Ldarg_0 &&
                        codes[i + 1].opcode == OpCodes.Ldfld &&
                        codes[i + 1].operand.ToString().Contains("crossHairPosDirtyTimer"))
                    {
                        foundInsertionPoint = true;
                        // 插入调用 DrawIndicator 的代码
                        var injectCode = new List<CodeInstruction>
                        {
                            new CodeInstruction(OpCodes.Ldarg_0),       // 加载 RangedWeapon 实例
                            new CodeInstruction(OpCodes.Ldarg_1),       // 加载 SpriteBatch 参数
                            new CodeInstruction(OpCodes.Call, typeof(TouhouGUI).GetMethod("DrawIndicator")) // 调用方法
                        };
                        codes.InsertRange(i + 1, injectCode);
                        break;
                    }
                }
                if (!foundInsertionPoint)
                    throw new Exception("Failed to find insertion point in IL code!");
                return codes;
            }
        }

        // ============ 修改后的DrawHint方法（移除HitHint逻辑） ============
        public static void DrawIndicator(RangedWeapon rangedWeapon, SpriteBatch spriteBatch)
        {

            if (rangedWeapon?.Item == null) return;
            ItemContainer container = rangedWeapon.Item.GetComponent<ItemContainer>();
            if (container == null) return;
            float containedIndicatorState = rangedWeapon.Item.GetComponent<ItemContainer>().GetContainedIndicatorState();

            Color[] gradientColors = new Color[] {
                GUIStyle.ColorInventoryEmpty,
                Color.Orange,
                GUIStyle.ColorInventoryHalf,
                Color.LimeGreen,
                GUIStyle.ColorInventoryFull
            };
            Color indicatorColor = MultiColorGradientLerp(containedIndicatorState, gradientColors);

/*            // 绘制环形轮廓
            TouhouGUIExtensions.DrawDonutSectionOutLine(
                spriteBatch,
                PlayerInput.MousePosition,
                new Range<float>(IndicatorRadiusStart - 1.0f, IndicatorRadiusStart + IndicatorThickness + 1.0f),
                IndicatorSectionRad,
                Color.Gray,
                0,
                IndicatorRotationAngle);

            // 绘制环形填充
            TouhouGUIExtensions.DrawDonutSection(
                spriteBatch,
                PlayerInput.MousePosition,
                new Range<float>(IndicatorRadiusStart, IndicatorRadiusStart + IndicatorThickness),
                containedIndicatorState * IndicatorSectionRad,
                indicatorColor,
                0,
                IndicatorRotationAngle + IndicatorSectionRad,
                false);*/

            // 显示耐久数值
            float durabilityValue = containedIndicatorState * 100f;
            Vector2 textPos = new Vector2(
                PlayerInput.MousePosition.X,
                PlayerInput.MousePosition.Y + IndicatorRadiusStart + IndicatorThickness + 15f);

            Vector2 textSize = GUIStyle.SmallFont.MeasureString($"{durabilityValue:0.0}%");
            Vector2 centeredPos = textPos - textSize / 2;

            GUI.DrawString(
                spriteBatch,
                centeredPos,
                $"{durabilityValue:0.0}%",
                indicatorColor,
                font: GUIStyle.SubHeadingFont);
        }

        // 保留多色渐变方法
        public static Color MultiColorGradientLerp(float t, params Color[] colors)
        {
            if (colors.Length == 0) return Color.White;
            t = MathHelper.Clamp(t, 0f, 1f);

            float segment = 1f / (colors.Length - 1);
            int index = (int)(t / segment);

            if (index >= colors.Length - 1)
            {
                return colors[^1];
            }

            float localT = (t - index * segment) / segment;
            return Color.Lerp(colors[index], colors[index + 1], localT);
        }

    }
}