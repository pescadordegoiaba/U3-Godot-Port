# Godot Resource Cache

## Por que existe

`GodotSceneBridge.SyncAll()` roda a cada frame. Sem cache, cada sync criaria novos `Godot.Mesh` e `StandardMaterial3D` para os mesmos componentes Unity-like, gerando alocacoes desnecessarias e dificultando diagnostico visual.

## O que e cacheado

`GodotSceneBridge` cacheia recursos por identidade de objeto:

- `UnityEngine.Mesh` para `Godot.Mesh`
- `UnityEngine.Material` para `StandardMaterial3D`
- `UnityEngine.GameObject` para `MeshInstance3D`

O cache de mesh cria `BoxMesh` ou `SphereMesh` uma vez para cada instancia `UnityEngine.Mesh`.

O cache de material cria um `StandardMaterial3D` uma vez para cada instancia `UnityEngine.Material` e atualiza apenas `ResourceName` e `AlbedoColor` quando `name` ou `color` mudam.

## Diagnostico

O bridge expoe contadores simples:

- `MeshResourcesCreated`
- `MaterialResourcesCreated`
- `MeshInstancesCreated`

Eles ajudam a confirmar que recursos nao estao sendo recriados a cada frame.

## Limitacoes

- O cache e por referencia, nao por igualdade estrutural.
- `UnityEngine.Mesh.primitiveKind` e tratado como imutavel.
- Nao ha assets reais, texturas, shaders ou import pipeline.
- Nao ha invalidacao para futuras meshes custom mutaveis.
- Fisica, UI e networking continuam fora desta etapa.

## Como testar manualmente

Abra `godot/` em uma instalacao Godot 4 C#/.NET e rode `res://scenes/Main.tscn`.

Resultado esperado:

- `ParentObject` continua aparecendo como box verde e se movendo;
- `ChildObject` continua aparecendo como sphere azul e acompanhando o pai;
- o host continua sem criar `MeshInstance3D` manualmente.

Tambem e possivel validar a compilacao com:

```bash
dotnet build godot/U3GodotPort.Godot.csproj
```

## Proximo passo

Adicionar testes de integracao Godot quando houver runner Godot .NET disponivel, ou extrair uma camada pura de cache para permitir testes unitarios sem depender do runtime Godot.
