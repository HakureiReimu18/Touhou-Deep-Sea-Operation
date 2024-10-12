
--[[
function TH.ClientFromName(name)

    if not SERVER then return nil end

    for key,client in pairs(Client.ClientList) do
        if client.Name == name then
            return client
        end
    end

    return nil
end]]
