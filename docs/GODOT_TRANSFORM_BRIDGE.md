# Godot Transform Bridge

## Arquitetura

`U3.GodotBridge` agora contém `GodotSceneBridge`, uma ponte pequena que associa um `UnityEngine.GameObject` fake a um `Godot.Node3D`.

O `UnityEngine.Shim` continua sem dependência de Godot. Ele apenas mantém `GameObject`, `Transform` e `RuntimeLoop`. A conversão para `Node3D` fica restrita ao bridge.

`GodotHost` cria um `GameObject` chamado `MovingCube`, adiciona um `MonoBehaviour` fake que atualiza `transform.position` e cria um `Node3D` com um `MeshInstance3D` usando `BoxMesh`. Depois de cada `RuntimeLoop.Tick(delta)`, o host chama `GodotSceneBridge.SyncAll()`.

## Sincronização atual

`GodotSceneBridge.SyncAll()` aplica:

- `Transform.position` para `Node3D.Position`
- `Transform.localScale` para `Node3D.Scale`
- `Transform.localRotation` para `Node3D.Quaternion`
- `GameObject.activeInHierarchy` para `Node3D.Visible`

O mapeamento de coordenadas e quaternion é direto nesta primeira fatia.

## Limitações

- Não há sincronização automática de hierarquia entre `Transform.parent` e árvore Godot.
- Não há criação automática de mesh, material, asset ou renderer a partir do U3-SDK.
- Não há física, UI ou assets.
- O cálculo world/local do shim ainda é simplificado, então `position` e `localPosition` se comportam da mesma forma.
- O mapeamento de eixos Unity/Godot ainda não aplica conversões de convenção.

## Como testar manualmente

Abra a pasta `godot/` em uma instalação Godot 4 C#/.NET e rode a cena `res://scenes/Main.tscn`.

Resultado esperado:

- logs aparecem no Output dock via `GodotUnityLogger`;
- um cubo chamado `MovingCube` aparece na cena;
- o cubo se move horizontalmente conforme `RuntimeLoop.Tick(delta)` chama o `MonoBehaviour` fake.

Também é possível validar a compilação com:

```bash
dotnet build godot/U3GodotPort.Godot.csproj
```

## Próximos passos

Adicionar sincronização de hierarquia `Transform.parent -> Node3D` e um componente fake de renderer mínimo para que objetos visuais possam ser descritos pelo lado Unity-like sem montar mesh manualmente no `GodotHost`.
