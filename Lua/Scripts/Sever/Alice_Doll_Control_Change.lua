---
--- Generated by EmmyLua(https://github.com/EmmyLua)
--- Created by Hakurei Reimu.
--- DateTime: 8/10/2024 下午6:06
---

local function CharacterToClient(character)

    if not SERVER then return nil end

    for key,client in pairs(Client.ClientList) do
        if client.Character == character then
            return client
        end
    end

    return nil
end

Hook.Add("Touhou_Alice_Magic_Book.OnUse", "Touhou.Alice_Doll_Control_Change", function(effect, deltaTime, item, targets, worldPosition, client)

    if targets[1] == nil then return end

    print(targets[1].Name .. " Used the Touhou_Alice_Magic_Book!")

    if client == nil then
        print("Error: client is nil.")
    end

    local character = targets[1]
    local client = CharacterToClient(targets[1])

    Timer.Wait(function()
        client.SetClientCharacter(character)
    end,60000)
--[[    Timer.Wait(function()
        client.SetClientCharacter(character)
    end,60000)]]
end)


--[[Hook.Add("Alice.Doll.Controller", "Player.Control.Change", function (client, deltaTime, character, targets)
    if targets[1] == nil then return end

    -- 获取上一次控制的角色
    local lastControlledCharacter = CharacterInfo("LastControlled")

    if CLIENT then
        if lastControlledCharacter then
            Timer.Wait(function()
                -- 将控制权转移到上一个控制的角色
                Character.Controlled = character
            end, 10)
        end
    else
        if lastControlledCharacter then
            Timer.Wait(function()
                -- 在服务器端设置客户端控制的角色为上一个控制的角色
                client.SetClientCharacter(character)
            end, 10)
        end
    end

    return true
end)]]


local function ClientFromName(name)

    if not SERVER then return nil end

    for key,client in pairs(Client.ClientList) do
        if client.Name == name then
            return client
        end
    end

    return nil
end

--[[
TLE.ItemMethods = {} -- with the identifier as the key

TLE.ItemMethods.Touhou_Elixir_Of_Penglai = function(targetCharacter)

        if SERVER then
            local Deadclient = CharacterToClient(targetCharacter)
            local client = ClientFromName(Deadclient)
            if client ~= nil then
                client.SetClientCharacter(targetCharacter)
            end
        end
end]]
