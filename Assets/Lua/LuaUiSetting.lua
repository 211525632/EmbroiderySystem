-- 命名空间
LuaUi = {}

-- 导入所有的require
function LuaUi:SetAllRequire()
    print("load all require")

    require("LuaBasicUiAction")
    require("LuaShowFps")

end