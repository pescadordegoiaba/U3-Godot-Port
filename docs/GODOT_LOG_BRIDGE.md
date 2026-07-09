# Godot Log Bridge

## Arquitetura

`UnityEngine.Shim` define a interface `IUnityLogger` e mantém `ConsoleUnityLogger` como logger padrão. `UnityEngine.Debug.Log`, `LogWarning` e `LogError` sempre delegam para o logger atual.

O logger pode ser trocado por `Debug.SetLogger(...)` e restaurado por `Debug.ResetLogger()`.

`U3.GodotBridge` implementa `GodotUnityLogger`, que mapeia:

- `Debug.Log` para `Godot.GD.Print`
- `Debug.LogWarning` para `Godot.GD.PushWarning`
- `Debug.LogError` para `Godot.GD.PushError`

`GodotRuntimeBootstrap.Initialize()` instala o logger Godot antes de criar o `GameObject` fake e rodar o lifecycle. `Shutdown()` reseta o runtime e restaura o logger padrão.

## Por que o Shim não depende de Godot

`UnityEngine.Shim` precisa continuar testável e utilizável fora do Godot. Por isso ele conhece apenas a abstração `IUnityLogger` e o fallback de console. A dependência em `GodotSharp` fica restrita ao `U3.GodotBridge`, que é a camada correta para adaptar o runtime fake ao host Godot.

## Como testar no Godot

Abra a pasta `godot/` em uma instalação Godot 4 C#/.NET e rode a cena principal `res://scenes/Main.tscn`.

Os logs esperados no Output dock incluem:

- `GodotRuntimeBootstrap initialized`
- `GodotHost ready`
- `BridgeTestBehaviour.Awake`
- `BridgeTestBehaviour.Start`
- `BridgeTestBehaviour.Update ...`
- `BridgeTestBehaviour.FixedUpdate ...`

Também é possível validar a compilação com:

```bash
dotnet build godot/U3GodotPort.Godot.csproj
```

## Limitações

- O bridge ainda só redireciona logs; não integra console estruturado, categorias ou filtros.
- O lifecycle continua fake e manual, dirigido por `_Process` e `_PhysicsProcess`.
- O binário Godot instalado localmente pode não ser a variante C#/.NET; nesse caso o build `dotnet` passa, mas o editor/headless não carrega scripts `.cs`.
