
--[[你说你不想在这里，我也不想在这里]]

local function Lerp(DefaultScreen, Num , OffsetNum)  --线性插值函数(改了个符号方便认)
return DefaultScreen * (1+ OffsetNum) + Num * OffsetNum 
end

local function GetSkillLevel(character,skilltype)  --获取角色技能水平
        return character.GetSkillLevel(Identifier(skilltype))
end



Hook.Patch("Barotrauma.Character", "ControlLocalPlayer", function(instance)
    local character = instance

    if not character then return end
    if Game.GameSession.IsTabMenuOpen then return end
    if GUI.GUI.PauseMenuOpen then return end
    if GUI.KeyboardDispatcher.Subscriber then return end
    --[[slottype中2代表右手，4代表左手]]
    if PlayerInput.SecondaryMouseButtonHeld()
            and (character.HasEquippedItem("Touhou_Cam_Offset_Low",true,2) or character.HasEquippedItem("Touhou_Cam_Offset_Low",true,4))
            and not character.SelectedItem
            and not (character.HasEquippedItem("mountableweapon",true,2) or character.HasEquippedItem("mountableweapon",true,4))
            and not (character.HasEquippedItem("tsm_farsight",true,4) or character.HasEquippedItem("tsm_farsight",true,2)) then
        Screen.Selected.Cam.OffsetAmount = math.min(Lerp(Screen.Selected.Cam.OffsetAmount, 0, 0.24)
                * (1 + math.min( GetSkillLevel(character,"Touhou_Magic") * 0.001, 0.15)), 356.5)
    end
    if PlayerInput.SecondaryMouseButtonHeld()
            and (character.HasEquippedItem("Touhou_Cam_Offset_Normal",true,2) or character.HasEquippedItem("Touhou_Cam_Offset_Normal",true,4))
            and not character.SelectedItem
            and not (character.HasEquippedItem("mountableweapon",true,2) or character.HasEquippedItem("mountableweapon",true,4))
            and not (character.HasEquippedItem("tsm_farsight",true,4) or character.HasEquippedItem("tsm_farsight",true,2)) then
        Screen.Selected.Cam.OffsetAmount = math.min(Lerp(Screen.Selected.Cam.OffsetAmount, 0, 0.42)
                * (1 + math.min( GetSkillLevel(character,"Touhou_Magic") * 0.001, 0.15)), 408.25)
    end
    if PlayerInput.SecondaryMouseButtonHeld()
            and (character.HasEquippedItem("Touhou_Cam_Offset_High",true,2) or character.HasEquippedItem("Touhou_Cam_Offset_High",true,4))
            and not character.SelectedItem
            and not (character.HasEquippedItem("mountableweapon",true,2) or character.HasEquippedItem("mountableweapon",true,4))
            and not (character.HasEquippedItem("tsm_farsight",true,4) or character.HasEquippedItem("tsm_farsight",true,2)) then
        Screen.Selected.Cam.OffsetAmount = math.min(Lerp(Screen.Selected.Cam.OffsetAmount, 0, 0.64)
                * (1 + math.min( GetSkillLevel(character,"Touhou_Magic") * 0.001, 0.15)), 471.5)
        end
end, Hook.HookMethodType.After)
