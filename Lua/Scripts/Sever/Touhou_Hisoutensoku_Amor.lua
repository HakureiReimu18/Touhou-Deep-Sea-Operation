
TLE.Preapply = {}
TLE.Postapply = {}

local function pre_apply(characterHealth, attackResult,hitLimb)
    for func in TLE.Preapply do
        if func(characterHealth, attackResult,hitLimb) == true then
            return true
        end
    end
    return false
end

local function post_apply(characterHealth, attackResult,hitLimb)
    for func in TLE.Postapply do
        func(characterHealth, attackResult,hitLimb)
    end
end

Hook.Add("character.applyDamage", "Touhou_Hisoutensoku_Amor.OnDamage", function (characterHealth, attackResult, hitLimb)

    --precheck
    if pre_apply(characterHealth, attackResult,hitLimb) then
        return true
    end

    local character = characterHealth.Character
    if character.Inventory == nil or characterHealth == nil or characterHealth.Character.IsDead or hitLimb == nil or  attackResult == nil or attackResult.Afflictions == nil then
        post_apply(characterHealth, attackResult, hitLimb)
        return
    end

    local armor = character.Inventory.GetItemInLimbSlot(InvSlotType.OuterClothes)



    if armor == nil or armor.Prefab.Identifier ~= "Touhou_Hisoutensoku" then return end

    local damage = 0

    for affliction in attackResult.Afflictions do
        damage = damage + affliction.Strength
    end
    armor.Condition = armor.Condition - damage * 0.1

    if armor.Condition > 0 then
        return true
    else
        Entity.Spawner.AddEntityToRemoveQueue(armor)
    end
    if armor.Condition <= 0 then
        armor.Condition = 0 -- 确保 Condition 不为负值
        Entity.Spawner.AddEntityToRemoveQueue(armor)
    end
end)