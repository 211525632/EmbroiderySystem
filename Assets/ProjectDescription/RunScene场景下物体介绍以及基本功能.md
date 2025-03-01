# RunScene场景下物体介绍以及基本功能

## RunScene介绍：
- 简介：目标是在此场景下游玩刺绣游戏
- 场景位置： Assets/Scenes/RunScene

## 非play状态下的物品与功能介绍：

- MainCamera：主相机，不过多介绍
---
- DirectionalLight：平行光，不过多介绍
---
- GlobalVolume：后处理相关，不过多介绍
---
- EventSystem：Ui交互的事件系统，没有此系统不能进行Ui交互
---
- Canvas：画布，属于Ui模块
- ---
- Manager：控制器（十分重要），以下为介绍
  - `RopeManager`组件，功能如下：
    - **单例模式**组件（可以直接在所有C#脚本中通过类名.Instance.属性/方法使用），**但是要注意：**需要手动将其挂载在场景某些物体下
    - 1. 管理所有rope的在场景中的生成     **其中有控制绳子与布料生成距离的函数**
    - 2. 通过RopePool来重复使用rope实例 （`CeateRope()`）
    - 3. 通过Rope来生成RopeMesh模型 (`RopeChangeToModel()`)
    - 4. 隐藏绳子（仅仅是隐藏ropePool中复用的绳子，不能隐藏已经模型化的Rope）
  - `PinManager`组件：**单例组件**,功能如下：
    - 初始化pin、pinOperator实例
    - 获取当前操控的针的`pinHelper`
    - 获取当前针的**刺绣状态**
    - 获取当前针与布料的相对位置
    - **设置pin的位置与旋转**（但是建议使用pinOperator来控制针，此方法仅作为初始化使用）
  - `PinPointVisualization`组件，**单例组件**，功能如下：
    - 设置穿刺点的可视化模型（*绿十字*）
    - 设置可视化模型的位置
    - 设置可视化模型的隐藏与显示
    -  单位**坐标吸附**功能的实现
    -  通过射线检测和坐标吸附来决定的可视化模型位置
    -  设置布料位置
   -  `GameManager`组件，`monoBehaviour`组件,功能如下
       -  初始化第一个针、线、布料
       -  实际上根据其作用，其应该被命名为**GameLoop**,而非GameManager。但是由于GameLoop还没有想好，就暂时这样了。
       -  整个***刺绣流程的主函数***
       -  PS：在这个组件中大量使用到**事件中心**。
          -  如果发现如下代码：`EventCenter<Action>.Instance.AddEvent(EventConst.OnRopeShrink, OnRopeShrink);`想知道其作用。请按照如下方式
          -  1. 查找OnRopeShrink这个变量的位置
          -  2. 检测这个变量的所有引用
          -  3. 转到各个引用地点来检查被注册的事件是什么
       -  其他不多赘述，请查看代码
   -  `EmbroideryOpMethedManager`:**单例组件**,功能如下
      -  ***设置不同的操作方式（如VR，键鼠）（十分重要）***
      -  设置了在键鼠操作方式下的针移动方式（前面一坨，无需关心）
      -  在"添加与设置"分区中，添加实现了接口`IEmbroideryOp`的VR操作方式是你们的工作
      -  当然，不要忘记修改最上方的`EEmbroideryOperation _operationState`枚举变量。
---
-  Helper：帮助实现一些效果的物体
   -  `MonoHelper`组件，继承了`MonoBehavior`的单例类。
   -  目的是通过此单例来为非`MonoBehavior`子类的普通类中使用携程，而非线程。
---
-  InputManager：
   -  `PlayerInput`组件,本来是用于收集玩家基本操作的，但是在本项目中使用的是newInputSys的其他用法。由此这个组件与物体***暂时无用***
---
-  CamerManager:
   -  `EmbroideryCameraCtl`：**相机操作**组件，由于不清楚VR设备对相机的操作行为，**没有预留VR**的镜头设置。
---
- Environment：作为环境的物体
  - 其下有BuLiaoGroup物体。
    - 是由两个相反的BuLiao组成。如果是单面可能无法使用物理检测
  - **SP特别注意：** 对于布料来讲，由于使用的是射线检测，所以需要设置他们的Layer为**布料**
  - 其他物体无需在意
---
- SaveOpManager：是实现撤销与恢复操作的物体
  - `EmbroideryOpSaverCtl`，**单例组件**，完成了基于键鼠的刺绣撤销操作与回忆操作。
  - 值得注意的是，依次撤销与回忆将会**连续操作两次**。此设置时为了放置撤销操作时，撤销之后**针与布料相对位置错误**的问题。
  - **PS：**键鼠操作下 ctrl + 箭头上下键即可使用撤销与回忆操作

---

### 特别提醒

> ***`PinPointVisualization`组件：设置布料位置***（十分重要）。
> 
> 在之前写开启布料检测的范围的时候，
> 
> 为了简写，就通过直接检测布料与针在y轴上的位置差距来实现。
> 
> 因为是纯粹的y值比较，所以没有考虑到布料旋转的问题。
>
> 如果旋转布料，可能会出现难以预料的问题。
    

> ***`PinPointVisualization`组件：再注意***:此组件中的可视化模型是**针头指向吸附**的重要标志
  
> `EmbroideryOpMethedManager`**VR操作实现**是通过完成实现了接口`IEmbroideryOp`的组件或者类来实现的（最好写在Asset/Scripts/Embroixxxx/Control目录下）
>
> ps：如果需要在普通类中使用***携程***，请使用`MonoHelp`单例类。