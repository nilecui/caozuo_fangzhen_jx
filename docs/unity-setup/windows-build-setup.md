# Windows 打包配置指南

> 在 Unity Editor 中手动完成以下步骤（Task 19）

## 1. Build Settings

**File > Build Settings**

- Platform: 选择 **Windows, Mac, Linux**，点击 **Switch Platform**（如未切换）
- Architecture: **x86_64**
- 添加以下场景（拖入 Scenes In Build，按序排列）：

| 序号 | 场景路径 |
|------|---------|
| 0 | Assets/Scenes/MainMenu.unity |
| 1 | Assets/Scenes/StructureDisplay.unity |
| 2 | Assets/Scenes/PrincipleDemo.unity |
| 3 | Assets/Scenes/OperationTraining.unity |
| 4 | Assets/Scenes/ScoreManagement.unity |

## 2. Player Settings

**Edit > Project Settings > Player**

| 设置项 | 值 |
|--------|---|
| Product Name | 装备虚拟仿真示教系统 |
| Company Name | （填写甲方单位名称） |
| Default Screen Width | 1920 |
| Default Screen Height | 1080 |
| Fullscreen Mode | Windowed |
| Run in Background | ✓ 勾选 |
| Scripting Backend | IL2CPP |
| Api Compatibility Level | .NET Standard 2.1 |

## 3. 执行打包

1. **File > Build Settings > Build**
2. 选择输出目录 `Builds/Windows/`
3. 等待构建完成（约 5~15 分钟，取决于资源量）

## 4. 验证

双击 `Builds/Windows/VirtualSimSystem.exe`，检查：

- [ ] 分辨率 1920×1080 正常启动
- [ ] 主菜单界面显示（蓝色标题、输入框、模块按钮）
- [ ] 输入学员信息后可进入 StructureDisplay 场景
- [ ] 3D 模型加载无报错（Console 无红色错误）
- [ ] FPS ≥ 60（GTX 1060 或更高 GPU）
- [ ] 启动时间 < 10 秒（SSD 环境）
- [ ] 完成考核流程后成绩 CSV 可正常导出

## 5. 依赖清单（打包前确认）

- [ ] sqlite-net-pcl DLL 已放入 `Assets/Plugins/sqlite-net/SQLite.cs`
- [ ] sqlite3.dll 已放入 `Assets/Plugins/x86_64/sqlite3.dll`
- [ ] Newtonsoft.Json 包已通过 Package Manager 安装
- [ ] Addressables 包已安装并完成 Bundle 构建
- [ ] 所有语音 .mp3 文件已放入 `Assets/StreamingAssets/configs/audio/`
- [ ] 所有 Assembly Definition (.asmdef) 已在 Unity Editor 中创建并配置引用
