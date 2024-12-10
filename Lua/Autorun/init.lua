TLE = {}
TLE.Name="Touhou_Lua_Expansion"
TLE.Version = "1.0"
TLE.VersionNum = 01000000
TLE.Path = table.pack(...)[1]

dofile(TLE.Path.."/Lua/Scripts/Sever/Touhou_Hisoutensoku_Amor.lua")

if CLIENT then
	Timer.Wait(function()
		local runstring = "\n/// Touhou Lua Expansion"..TLE.Version.." ///\n"
		print(runstring)  
	end,1)
	dofile(TLE.Path.."/Lua/Scripts/Client/Touhou_Cam_Offset.lua")
end

if CLIENT then
	dofile(TLE.Path.."/Lua/Scripts/Client/Alice_Doll_Control_Change_Client.lua")
end