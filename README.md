# 装备虚拟仿真示教系统

基于 Unity 2022.3 LTS（URP）的 Windows 桌面应用，为装卡车与起重起卷机车提供虚拟仿真示教，涵盖结构认知、原理演示、操作训练、考核评估四大教学模块。

## 系统要求

| 项目 | 要求 |
|------|------|
| 操作系统 | Windows 10 x64 及以上 |
| GPU | NVIDIA GTX 1060 或同等（目标 60 FPS） |
| 分辨率 | 1920 × 1080 |
| 运行时 | 无需安装 Unity，直接运行 `.exe` |

---

## 本地开发环境搭建

### 1. 安装 Unity

- 下载 [Unity Hub](https://unity.com/download)
- 安装 **Unity 2022.3 LTS**，勾选以下模块：
  - Windows Build Support (IL2CPP)
  - Visual Studio（或 VS Code）

### 2. 打开项目

在 Unity Hub 中选择 **Open > Add project from disk**，指向本仓库根目录。Unity 会自动安装 `Packages/manifest.json` 中声明的所有包（首次打开约需几分钟）。

> **首次打开后必须提交 ProjectSettings/：**
> ```bash
> git add ProjectSettings/ Packages/packages-lock.json
> git commit -m "chore: add Unity ProjectSettings from Editor"
> ```
> 这是 GitHub Actions 自动构建所必需的。

### 3. 手动放置 SQLite 依赖

sqlite-net-pcl 无法通过 Package Manager 安装，需手动放置：

```
Assets/Plugins/sqlite-net/SQLite.cs      ← 从 NuGet 包 sqlite-net-pcl 提取
Assets/Plugins/x86_64/sqlite3.dll        ← Windows x64 原生库
```

### 4. 创建 Assembly Definition 文件

在 Unity Editor 中（Project 窗口右键 > Create > Assembly Definition）：

| 目录 | 文件名 | 说明 |
|------|--------|------|
| `Assets/Scripts/` | `VirtualSim.asmdef` | 运行时程序集 |
| `Assets/Tests/EditMode/` | `VirtualSim.Tests.asmdef` | 测试程序集，引用 NUnit 和上面的运行时程序集 |

### 5. 运行测试

**Window > General > Test Runner > EditMode > Run All**

### 6. 打包

详见 [`docs/unity-setup/windows-build-setup.md`](docs/unity-setup/windows-build-setup.md)。

---

## 添加训练课程

在 `Assets/StreamingAssets/configs/` 放置 JSON 文件，无需重新编译：

```json
{
  "training_name": "课程名称",
  "equipment_type": "ZhuangKaChe_TypeA",
  "pass_score": 60,
  "time_limit_seconds": 600,
  "steps": [
    {
      "step_id": 1,
      "step_name": "步骤名称",
      "hint_text": "操作提示文字",
      "content_info": "知识点说明",
      "voice_file": "configs/audio/step_01.mp3",
      "target_object": "GameObject名称",
      "trigger_action": "click",
      "trigger_params": {},
      "score_weight": 20,
      "timeout_seconds": 60
    }
  ]
}
```

所有 `score_weight` 之和必须等于 100。将新课程文件名注册到 `Assets/Scripts/UI/MainMenuController.cs` 的 `EquipmentConfigMap` 字典中。

---

## 添加 3D 模型

1. SolidWorks STEP/IGES → PiXYZ Studio / CAD Exchanger 减面 → 导出 FBX
2. 放入 `Assets/Models/<装备类型>/<TypeA|TypeB>/`
3. 在 Unity Editor 中配置 LOD Group 和 Addressable 标记

详见 [`docs/unity-setup/addressable-lod-setup.md`](docs/unity-setup/addressable-lod-setup.md)。

---

## 自动构建 & 发布（GitHub Actions）

推送版本标签即可触发自动构建并发布到 GitHub Releases：

```bash
git tag v1.0.0
git push origin v1.0.0
```

Actions 会自动构建 `VirtualSimSystem-Windows-v1.0.0.zip` 并挂到 Release 页面。

### 首次配置（只需一次）

需要在仓库 **Settings > Secrets and variables > Actions** 中添加以下三个 Secret：

| Secret 名称 | 说明 |
|-------------|------|
| `UNITY_LICENSE` | Unity 许可证文件内容（`.ulf` 文件的完整文本） |
| `UNITY_EMAIL` | Unity ID 邮箱 |
| `UNITY_PASSWORD` | Unity ID 密码 |

**获取 `.ulf` 许可证文件的步骤：**

1. 在本地运行以下命令生成激活申请文件（`.alf`）：

   ```bash
   # 将 YOUR_UNITY_VERSION 替换为实际版本，如 2022.3.62f1
   docker run --rm \
     -e UNITY_VERSION=YOUR_UNITY_VERSION \
     unityci/editor:YOUR_UNITY_VERSION-base-3 \
     unity-editor -batchmode -createManualActivationFile -logfile /dev/stdout
   ```

   或直接在本机 Unity Editor 执行：**Help > Manage License > Activate manually** 下载 `.alf` 文件。

2. 前往 [license.unity3d.com](https://license.unity3d.com/manual) 上传 `.alf`，下载返回的 `.ulf` 文件。

3. 将 `.ulf` 文件的**全部文本内容**粘贴到 `UNITY_LICENSE` Secret 中。

> Unity Personal（免费版）即可。每次许可证到期时重复上述流程更新 Secret。

### 构建说明

- 构建使用 [game-ci/unity-builder](https://game.ci) 在 Ubuntu Docker 容器中执行，已内置 Unity 安装
- `Library/` 目录有缓存，相同代码的二次构建速度显著更快
- 工作流文件：[`.github/workflows/release.yml`](.github/workflows/release.yml)

---

## 文档

| 文档 | 说明 |
|------|------|
| [`docs/superpowers/specs/`](docs/superpowers/specs/) | 系统设计规格文档 |
| [`docs/superpowers/plans/`](docs/superpowers/plans/) | 实施计划（任务分解） |
| [`docs/unity-setup/addressable-lod-setup.md`](docs/unity-setup/addressable-lod-setup.md) | Addressable 资源打包 & LOD 配置 |
| [`docs/unity-setup/windows-build-setup.md`](docs/unity-setup/windows-build-setup.md) | Windows 打包配置指南 |
