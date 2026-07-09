# Godot Hierarchy Bridge

## Arquitetura

`GodotSceneBridge` mantém o mapeamento entre `UnityEngine.GameObject` fake e `Godot.Node3D`. Cada node criado também guarda o root Godot informado em `CreateNode(gameObject, parent)`.

Durante `SyncAll()`, a ponte sincroniza primeiro a hierarquia:

- se `Transform.parent == null`, o `Node3D` fica sob o root informado em `CreateNode`;
- se `Transform.parent != null`, o `Node3D` é reparentado para o `Node3D` do `GameObject` pai;
- reparent só acontece quando o parent Godot atual é diferente do parent desejado.

Depois da hierarquia, `SyncAll()` aplica posição, escala, rotação e visibilidade.

## Demo Atual

`GodotHost` cria dois objetos fake:

- `ParentObject`, com um `MonoBehaviour` que move seu `Transform.position`;
- `ChildObject`, com `Transform.parent` apontando para `ParentObject.transform`.

Ambos recebem um `Node3D` com `MeshInstance3D` e `BoxMesh` manual. O filho acompanha o pai porque seu `Node3D` é reparentado para o node do pai.

## Limitações

- Parents sem node Godot criado não geram nodes automaticamente; o filho permanece no root e um warning é emitido.
- O reparent não preserva transform global, seguindo o modelo simplificado atual do shim.
- `Transform.position` e `localPosition` ainda são equivalentes no shim.
- Não há renderer fake, material, física, UI ou assets.
- Não há conversão de convenções de eixos Unity/Godot.

## Como testar manualmente

Abra `godot/` em uma instalação Godot 4 C#/.NET e rode `res://scenes/Main.tscn`.

Resultado esperado:

- logs aparecem no Output dock;
- `ParentObject` se move horizontalmente;
- `ChildObject` permanece visualmente ligado ao pai e acompanha o movimento.

Também é possível validar a compilação com:

```bash
dotnet build godot/U3GodotPort.Godot.csproj
```

## Próximo Passo

Criar um renderer fake mínimo no lado `UnityEngine.Shim`, por exemplo `MeshRenderer`/`MeshFilter` stubs simples, e adaptar o bridge para criar o visual a partir desses componentes em vez de montar `MeshInstance3D` manualmente no `GodotHost`.
