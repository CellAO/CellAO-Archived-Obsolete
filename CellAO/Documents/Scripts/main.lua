print("Main.lua script initialized!")
dofile("onuse_scripts.lua")

function LolFunc(client)
	client:Print("Lollar!")
end

function OnPlayerConnect(client)
	client:Print("Welcome, " .. client.Character.Name .. ". Date is " .. os.date())
end

function OnPlayerCommand(client, cmd, arg1)
	if (cmd == "lol") then
		LolFunc(client)
		return true
	elseif (cmd == "coords") then
		local x = client.Character.coordX
		local y = client.Character.coordY
		local z = client.Character.coordZ
		client:Print("Your coordinates: " .. x .. ", " .. y .. ", " .. z)
		return true
	elseif (cmd == "time") then
		client:Print("Current time: " .. os.date())
		return true
	elseif (cmd == "mobtest") then
		SpawnMob(client, 1)
	end
	
	return false
end
