# Addressable 资源打包 & LOD 配置指南

> 在 Unity Editor 中手动完成以下步骤（Task 18）

## 前置条件

- Unity 2022.3 LTS 已安装 Addressables 包（`com.unity.addressables`）
- 所有车辆 FBX 模型已放入 `Assets/Models/` 对应目录

## 1. 标记 Prefab 为 Addressable

1. 在 Project 窗口选中 `Assets/Models/ZhuangKaChe/TypeA/` 下的车辆 Prefab
2. Inspector 中勾选 **Addressable** 复选框
3. 在弹出的 Group 分配对话框中选择/创建 Group 名：`ZhuangKaChe-TypeA`
4. 对以下目录重复操作，使用对应 Group 名：
   - `Assets/Models/ZhuangKaChe/TypeB/` → Group: `ZhuangKaChe-TypeB`
   - `Assets/Models/QiJuanChe/TypeA/`  → Group: `QiJuanChe-TypeA`
   - `Assets/Models/QiJuanChe/TypeB/`  → Group: `QiJuanChe-TypeB`
   - `Assets/Models/LiftObjects/`      → Group: `LiftObjects`

## 2. 配置 LOD Group

对每个车辆 Prefab 根 GameObject：

1. 在 Inspector 点击 **Add Component** → 搜索 `LOD Group`
2. 配置三级 LOD：

| LOD 级别 | 屏幕占比阈值 | 目标面数 | 说明 |
|----------|------------|---------|------|
| LOD 0    | 100% ~ 30% | ≤ 100,000 面 | 近景高精度 |
| LOD 1    | 30% ~ 10%  | ≤ 30,000 面  | 中景简化 |
| LOD 2    | 10% ~ 2%   | ≤ 10,000 面  | 远景极简 |
| Culled   | < 2%       | —           | 完全剔除 |

3. 将对应精度的 Renderer 组件拖入每个 LOD 插槽
4. 关键部件（操作热点）保持独立 Prefab，LOD 0 面数 ≤ 10,000

## 3. 验证 Addressable 加载

在 StructureDisplay 场景添加临时测试脚本验证加载：

```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableTest : MonoBehaviour
{
    private IEnumerator Start()
    {
        var op = Addressables.LoadAssetAsync<GameObject>("ZhuangKaChe-TypeA");
        yield return op;
        if (op.Status == AsyncOperationStatus.Succeeded)
        {
            Instantiate(op.Result);
            Debug.Log("[Test] Addressable load OK");
        }
        else
            Debug.LogError("[Test] Addressable load FAILED: " + op.OperationException);
    }
}
```

确认 Console 无报错后删除此测试脚本。

## 4. 构建 Addressable Bundle

**Window > Asset Management > Addressables > Groups** → 点击 **Build > New Build > Default Build Script**

构建完成后 `Assets/StreamingAssets/aa/` 目录会生成 bundle 文件。
