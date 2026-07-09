# Godot Mesh Renderer Bridge

## Arquitetura

`UnityEngine.Shim` agora tem stubs mínimos de render:

- `Mesh`
- `MeshFilter`
- `Renderer`
- `MeshRenderer`
- `Material`

`GodotSceneBridge` procura `MeshFilter` e `MeshRenderer` em cada `GameObject` registrado. Quando ambos existem e `MeshFilter.sharedMesh` está definido, a ponte cria automaticamente um `Godot.MeshInstance3D` como filho do `Node3D` do objeto.

O `UnityEngine.Shim` continua sem dependência de Godot. A conversão para `MeshInstance3D`, `BoxMesh`, `SphereMesh` e `StandardMaterial3D` fica restrita ao `U3.GodotBridge`.

## Shapes Suportadas

`Mesh` suporta apenas primitives simples nesta etapa:

- `Mesh.CreateBox()` -> `Godot.BoxMesh`
- `Mesh.CreateSphere()` -> `Godot.SphereMesh`

`MeshFilter.mesh` e `MeshFilter.sharedMesh` são aliases simples para o mesmo valor.

## Material

`Material` é um stub com `name` herdado de `Object` e `Color color`.

O bridge cria um `StandardMaterial3D` simples e aplica `Material.color` em `AlbedoColor`. `Renderer.material` e `Renderer.sharedMaterial` são aliases simples nesta etapa.

## Demo Atual

`GodotHost` cria `ParentObject` e `ChildObject` usando componentes Unity-like:

- `AddComponent<MeshFilter>()`
- `AddComponent<MeshRenderer>()`
- `meshFilter.sharedMesh = Mesh.CreateBox()` ou `Mesh.CreateSphere()`
- `meshRenderer.material.color = ...`

O bridge cria os `MeshInstance3D` automaticamente durante `SyncAll()`.

## Limitações

- Não há assets reais, mesh customizada, textura, shader ou import pipeline.
- Não há `MeshFilter`/`MeshRenderer` compatível com toda a API Unity.
- O bridge recria recursos Godot simples conforme a sincronização, suficiente para smoke test.
- Não há física, UI ou networking.
- Não há conversão de convenções de eixo Unity/Godot.

## Como testar manualmente

Abra `godot/` em uma instalação Godot 4 C#/.NET e rode `res://scenes/Main.tscn`.

Resultado esperado:

- `ParentObject` aparece com mesh box verde e se move horizontalmente;
- `ChildObject` aparece com mesh sphere azul e acompanha o pai pela hierarquia;
- não há criação manual de `MeshInstance3D` no `GodotHost`.

Também é possível validar a compilação com:

```bash
dotnet build godot/U3GodotPort.Godot.csproj
```

## Próximo Passo

Adicionar cache de recursos Godot para evitar recriar mesh/material em syncs repetidos e começar a mapear stubs adicionais usados por renderização do U3-SDK sem puxar assets reais ainda.
