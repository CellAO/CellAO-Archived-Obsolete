--[[
Copyright (c) 2005-2008, CellAO Team

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
]]

--This is for Suiv, who wanted the hardcoded stuff ripped out and wanted to see it in a script, so here; LUA IN ACTION :D

Types =
{
	Soldier = 3222340084,
	Martial_Artist = 3222405620,
	Engineer = 3222471156,
	Fixer = 3222536692,
	Agent = 3222602228,
	Adventurer = 3222667764,
	Trader = 3222733300,
	Bureaucrat = 3222798836,
	Enforcer = 3222864372,
	Doctor = 3222929908,
	Nano_Technician = 3222995444,
	Meta_Physicist = 3223060980,
	Shade = 3223323124,
	Keeper = 3223388660,
	Neutral = 3223126516,
	Clan = 3223192052,
	Omni_Tek = 3223257588,
	Nano_Male = 3221553652,
	Nano_Female = 3221619188,
	Atrox = 3221684724,
	Opifex_Female = 3221488116,
	Opifex_Male = 3221422580,
	Solitus_Female = 3221357044,
	Solitus_Male = 3221291508,
	Ectomorph = 3222077940,
	Mesomorph = 3221946868,
	Endomorph = 3222012404,
	Tall = 3222209012,
	Normalize = 3222143476,
	Short = 3222274548,
}

local OnUseMap =
{
	--insert more here plx
	500 = OnUse_ARKISLAND,
}

local function SetStat(p, idx, val)
	p.Character.stats.SetStat(idx, val)
end

function OnUse(pf, sender, target, instance)
	return OnUseMap[pf](sender, target, instance)
end

function OnUse_ARKISLAND(sender, target, instance)
	if (target.Type ~= 51005) then
		return false
	end
	
	--GM CHECK
	if (sender.Character.stats.GetStat("GmLevel") <= 0) then
		return false
	end
	
	if (instance == Types.Soldier) then
		p:SetStat(60, 1)
		p:SetStat(368, 1)
	elseif (instance == Types.Martial_Artist) then
		p:SetStat(60, 2)
		p:SetStat(368, 2)
	elseif (instance == Types.Engineer) then
		p:SetStat(60, 3)
		p:SetStat(368, 3)
	elseif (instance == Types.Fixer) then
		p:SetStat(60, 4)
		p:SetStat(368, 4)
	elseif (instance == Types.Agent) then
		p:SetStat(60, 5)
		p:SetStat(368, 5)
	elseif (instance == Types.Adventurer) then
		p:SetStat(60, 6)
		p:SetStat(368, 6)
	elseif (instance == Types.Trader) then
		p:SetStat(60, 7)
		p:SetStat(368, 7)
	elseif (instance == Types.Bureaucrat) then
		p:SetStat(60, 8)
		p:SetStat(368, 8)
	elseif (instance == Types.Enforcer) then
		p:SetStat(60, 9)
		p:SetStat(368, 9)
	elseif (instance == Types.Doctor) then
		p:SetStat(60, 10)
		p:SetStat(368, 10)
	elseif (instance == Types.Nano_Technician) then
		p:SetStat(60, 11)
		p:SetStat(368, 11)
	elseif (instance == Types.Meta_Physicist) then
		p:SetStat(60, 12)
		p:SetStat(368, 12)
	elseif (instance == Types.Shade) then
		p:SetStat(60, 15)
		p:SetStat(368, 15)
	elseif (instance == Types.Keeper) then
		p:SetStat(60, 14)
		p:SetStat(368, 14)
	elseif (instance == Types.Neutral) then
		p:SetStat(33, 0)
	elseif (instance == Types.Clan) then
		p:SetStat(33, 1)
	elseif (instance == Types.Omni_Tek) then
		p:SetStat(33, 2)
	-- ///////////////////////////////////////////
	elseif (instance == Types.Nano_Male) then
		p:SetStat(367, 3)
		p:SetStat(369, 2)
		p:SetStat(59, 2)
		p:SetStat(4, 3)
	elseif (instance == Types.Nano_Female) then
		p:SetStat(367, 3)
		p:SetStat(369, 3)
		p:SetStat(59, 3)
		p:SetStat(4, 3)
	elseif (instance == Types.Atrox) then
		p:SetStat(367, 4)
		p:SetStat(369, 1)
		p:SetStat(59, 1)
		p:SetStat(4, 4)
	elseif (instance == Types.Opifex_Female) then
		p:SetStat(367, 2)
		p:SetStat(369, 3)
		p:SetStat(59, 3)
		p:SetStat(4, 2)
	elseif (instance == Types.Opifex_Male) then
		p:SetStat(367, 2)
		p:SetStat(369, 2)
		p:SetStat(59, 2)
		p:SetStat(4, 2)
	elseif (instance == Types.Solitus_Female) then
		p:SetStat(367, 1)
		p:SetStat(369, 3)
		p:SetStat(59, 3)
		p:SetStat(4, 1)
	elseif (instance == Types.Solitus_Male) then
		p:SetStat(367, 1)
		p:SetStat(369, 2)
		p:SetStat(59, 2)
		p:SetStat(4, 1)
		p:SetStat(58, 99)
	elseif (instance == Types.Ectomorph) then
		p:SetStat(47, 0)
	elseif (instance == Types.Mesomorph) then
		p:SetStat(47, 1)
	elseif (instance == Types.Endomorph) then
		p:SetStat(47, 2)
	elseif (instance == Types.Tall) then
		p:SetStat(360, 110)
	elseif (instance == Types.Short) then
		p:SetStat(360, 90)
	elseif (instance == Types.Normalize) then
		p:SetStat(360, 100)
	else
		--unknown
		return false
	end
	
	--send reply
	return true
end
