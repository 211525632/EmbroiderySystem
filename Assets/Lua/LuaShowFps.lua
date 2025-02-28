_G.LuaUi.LuaShowFps = {}

-- local LuaShowFps = _G.LuaUi.LuaShowFps

-- 继承
LuaUi.LuaShowFps.__index = LuaUi.LuaShowFps

setmetatable(LuaUi.LuaShowFps,{__index = _G.LuaUi.LuaBasicUiAction})

--  变量
local UiText = " "
local m_LastUpdateShowTime = 0
local m_UpdateShowDeltaTime = 0.2
local m_FrameUpdate = 0
local m_FPS = 0


function LuaUi.LuaShowFps:new(o)
    -- 初始化
    _G.LuaUi.LuaBasicUiAction:new(o)
    print("init LuaShowFps!")
    return self
end

function LuaUi.LuaShowFps:OnEnable()
    print("run enable")
    m_LastUpdateShowTime = CS.UnityEngine.Time.realtimeSinceStartup;
    UiText = LuaUi.LuaShowFps.uiCollection:GetComponent("UiCollection"):GetUi("FpsNum")
end

function LuaUi.LuaShowFps:OnUpdate()
    m_FrameUpdate = m_FrameUpdate + 1;
    if CS.UnityEngine.Time.realtimeSinceStartup - m_LastUpdateShowTime >= m_UpdateShowDeltaTime then
        
        -- FPS = 某段时间内的总帧数 / 某段时间
        m_FPS = m_FrameUpdate / (CS.UnityEngine.Time.realtimeSinceStartup - m_LastUpdateShowTime);
        m_FrameUpdate = 0;
        m_LastUpdateShowTime = CS.UnityEngine.Time.realtimeSinceStartup;
        UiText:SetText(1 / CS.UnityEngine.Time.deltaTime)
    end
end

function LuaUi.LuaShowFps:OnDisable()
    
    print("ShowFps::OnDisable")
end

function LuaUi.LuaShowFps:Test()
    print("get func test")
end