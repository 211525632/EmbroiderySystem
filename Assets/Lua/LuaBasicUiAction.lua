-- 设置命名空间
_G.LuaUi.LuaBasicUiAction = {}

-- 简化书写
-- local LuaBasicUiAction = _G.LuaUi.LuaBasicUiAction
-- 面向对象
_G.LuaUi.LuaBasicUiAction.__index = _G.LuaUi.LuaBasicUiAction

_G.LuaUi.LuaBasicUiAction.uiCollection = "userdata"

-- 继承C# MonoBehaviour
setmetatable(_G.LuaUi.LuaBasicUiAction,{__index = CS.UnityEngine.MonoBehaviour})

function _G.LuaUi.LuaBasicUiAction:new(UiCollection)
    if type(UiCollection)~="userdata" and UiCollection==nil then
        print("init fail:".."the UiCollection Name is not a userdata!".."it's '"..type(UiCollectionName))
        return nil
    end

    

    return self
end

-- 之后将其映射成Unity.Action
function _G.LuaUi.LuaBasicUiAction:OnEnable()
    print("LuaBasicUiAction::OnEnable")
end

function _G.LuaUi.LuaBasicUiAction:OnDisable()
    print("LuaBasicUiAction::OnDisable")
end

function _G.LuaUi.LuaBasicUiAction:OnDestroy()
    print("LuaBasicUiAction::OnDestroy")
end

function _G.LuaUi.LuaBasicUiAction:OnUpdate()
    print("LuaBasicUiAction::OnUpdate")
end
