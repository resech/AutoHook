using System;
using System.Numerics;
using AutoHook.Configurations;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Logging;
using ImGuiNET;

namespace AutoHook.Ui
{
    abstract class TabConfig : IDisposable
    {
        public abstract string TabName { get; }

        public abstract bool Enabled { get; }

        public string StrHookWeak => "Hook Weak (!)";
        public string StrHookStrong => "Hook Strong (!!)";
        public string StrHookLegendary => "Hook Legendary (!!!)";

        public abstract void Draw();

        public abstract void DrawHeader();

        public virtual void Dispose() { }

        public void DrawDeleteBaitButton(HookSettings cfg)
        {

            ImGui.PushFont(UiBuilder.IconFont);
            if (ImGui.Button($"{FontAwesomeIcon.Trash.ToIconChar()}", new Vector2(ImGui.GetFrameHeight(), 0)) && ImGui.GetIO().KeyShift)
            {
                Service.Configuration.CustomBaitMooch.RemoveAll(x => x.BaitName == cfg.BaitName);
                Service.Configuration.Save();
            }
            ImGui.PopFont();

            if (ImGui.IsItemHovered())
                ImGui.SetTooltip("Hold SHIFT to delete.");
        }

        public void DrawHookCheckboxes(HookSettings cfg)
        {
            ImGui.Checkbox(StrHookWeak, ref cfg.HookWeak);
            ImGui.Checkbox(StrHookStrong, ref cfg.HookStrong);
            ImGui.Checkbox(StrHookLegendary, ref cfg.HookLendary);
        }

        public void DrawInputTextName(HookSettings cfg)
        {
            string matchText = new string(cfg.BaitName);
            ImGui.SetNextItemWidth(-200 * ImGuiHelpers.GlobalScale);
            if (ImGui.InputText("Mooch/Bait Name", ref matchText, 64, ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.EnterReturnsTrue))
            {
                if (cfg.BaitName != matchText && Service.Configuration.CustomBaitMooch.Contains(new HookSettings(matchText)))
                    cfg.BaitName = "Bait already exists";
                else
                    cfg.BaitName = matchText;

                Service.Configuration.Save();
            };
        }

        public void DrawInputDoubleMaxTime(HookSettings cfg)
        {
            ImGui.SetNextItemWidth(100 * ImGuiHelpers.GlobalScale);
            if (ImGui.InputDouble("Max. Wait", ref cfg.MaxTimeDelay, .1, 1, "%.1f%"))
            {
                switch (cfg.MaxTimeDelay)
                {
                    case 0.1:
                        cfg.MaxTimeDelay = 2;
                        break;
                    case <= 0:
                    case <= 1.9: //This makes the option turn off if delay is 2 seconds when clicking the minus.
                        cfg.MaxTimeDelay = 0;
                        break;
                    case > 99:
                        cfg.MaxTimeDelay = 99;
                        break;
                }
            }
            ImGui.SameLine();
            ImGuiComponents.HelpMarker("Hook will be used after the defined amount of time has passed\nMin. time: 2s (because of animation lock)\n\nSet Zero (0) to disable, and dont make this lower than the Min. Wait");
        }

        public void DrawInputDoubleMinTime(HookSettings cfg)
        {
            ImGui.SetNextItemWidth(100 * ImGuiHelpers.GlobalScale);
            if (ImGui.InputDouble("Min. Wait", ref cfg.MinTimeDelay, .1, 1, "%.1f%"))
            {

            }
            ImGui.SameLine();
            ImGuiComponents.HelpMarker("Hook will NOT be used until the minimum time has passed.\n\nEx: If you set the number as 14 and something bites after 8 seconds, the fish will not to be hooked\n\nSet Zero (0) to disable");
        }


        public void DrawEnabledButtonCustomBait(HookSettings cfg)
        {
            ImGui.Checkbox("Enabled ->", ref cfg.Enabled);
            ImGuiComponents.HelpMarker("Important!!!\n\nIf disabled, the fish will NOT be hooked.\nTo use the default behavior (General Tab), please delete this configuration.");
        }

        public void DrawCheckBoxDoubleTripleHook(HookSettings cfg)
        {
            if (ImGui.TreeNode("Double/Triple Hook"))
            {
                
                if (ImGui.Checkbox("Use Double Hook (If gp > 400)", ref cfg.UseDoubleHook))
                {
                    if (cfg.UseDoubleHook) cfg.UseTripleHook = false;
                    Service.Configuration.Save();
                }
                if (ImGui.Checkbox("Use Triple Hook (If gp > 700)", ref cfg.UseTripleHook))
                {
                    if (cfg.UseTripleHook) cfg.UseDoubleHook = false;
                    Service.Configuration.Save();
                }

                if (cfg.UseTripleHook || cfg.UseDoubleHook) {
                    ImGui.Indent();
                    
    
                    ImGui.Checkbox("Also use when Patience is active (not recommended)", ref cfg.UseTripleDoubleHookPacience);
                    ImGuiComponents.HelpMarker("Important!!!\n\nIf disabled, Precision/Powerful hook will be used instead when Patience is up.");
                    ImGui.Unindent();
                }
                
                ImGui.TreePop();
            }
            
        }

    }
}