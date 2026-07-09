# Unity Rotation Shim

## APIs Implementadas

`UnityEngine.Shim` agora tem uma camada minima de rotacao:

- `Quaternion.Euler(float x, float y, float z)`
- `Quaternion.Euler(Vector3 euler)`
- `Quaternion.eulerAngles`
- `Quaternion.LookRotation(Vector3 forward)`
- `Quaternion.LookRotation(Vector3 forward, Vector3 upwards)`
- `Quaternion.normalized`
- `Quaternion.Normalize(...)`
- `Quaternion * Quaternion`
- `Quaternion * Vector3`
- `Vector3.sqrMagnitude`
- `Vector3.Normalize()`
- `Vector3.Angle(...)`
- `Vector3.ProjectOnPlane(...)`
- `Transform.eulerAngles`
- `Transform.localEulerAngles`
- `Transform.LookAt(...)`
- `Transform.forward`
- `Transform.up`
- `Transform.right`

## Limites Matematicos

A implementacao e intencionalmente pequena. Ela cobre casos basicos de camera, luz e objetos visuais, mas ainda nao pretende ser 100% equivalente a Unity.

Limitacoes conhecidas:

- conversao `Quaternion.eulerAngles` e aproximada;
- casos degenerados em `LookRotation` usam fallbacks simples;
- `Transform.rotation` e `localRotation` ainda espelham o mesmo valor;
- nao ha composicao real de rotacao com pais;
- nao ha conversao avancada entre convencoes Unity e Godot.

## Impacto no Godot Bridge

`GodotSceneBridge` sincroniza `Transform.localRotation.normalized` para `Node3D.Quaternion`. Como `Camera3D`, `Light3D` e `MeshInstance3D` sao filhos do `Node3D` do `GameObject`, eles herdam a rotacao desse node.

`GodotHost` usa:

- `Transform.LookAt(...)` para apontar `MainCamera`;
- `Quaternion.Euler(...)` para orientar `SunLight`.

## Como Testar Manualmente

Abra `godot/` em uma instalacao Godot 4 C#/.NET e rode `res://scenes/Main.tscn`.

Resultado esperado:

- a camera gerada aponta para o demo;
- a luz direcional usa a rotacao definida pelo shim;
- `ParentObject` e `ChildObject` continuam visiveis.

Tambem e possivel validar com:

```bash
dotnet build U3GodotPort.sln
dotnet test U3GodotPort.sln
dotnet build godot/U3GodotPort.Godot.csproj
```
