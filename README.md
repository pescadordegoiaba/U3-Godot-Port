# U3-Godot-Port

Port gradual do U3-SDK para rodar sem Unity, usando .NET 8 e Godot 4 C# como backend.

O SDK original fica em `external/U3-SDK` e não deve ser modificado. O projeto consome arquivos reais do U3-SDK por links no `src/U3.Runtime`, enquanto implementa uma camada fake/testável de compatibilidade Unity em `UnityEngine.Shim`.

## Estado Atual

O projeto já tem:

- solução .NET `U3GodotPort.sln`;
- `UnityEngine.Shim` com core Unity-like, math/foundation, lifecycle fake, debug logger, resources/player prefs fake, scene stubs, UI/TMPro stubs, render básico, câmera/luz, physics passivo e input fake;
- `U3.Runtime` compilando fatias reais do U3-SDK sem copiar arquivos;
- `U3.GodotBridge` sincronizando `GameObject -> Node3D`, hierarquia, mesh/material, câmera, luz, colliders e raycast via Godot Physics;
- projeto Godot 4 C# em `godot/`;
- demo mínima jogável com `PlayerControllerDemo`, mouse-look, jump, interação e spawns por dados;
- testes cobrindo shim, NetPak, UnityNetPak, UnityEx helpers, physics/input backend, player demo e data-driven spawns.

Validação atual:

```bash
dotnet build U3GodotPort.sln
dotnet test U3GodotPort.sln
dotnet build godot/U3GodotPort.Godot.csproj
```

Último estado verificado: `155/155` testes passando e builds .NET/Godot passando.

## Fatias Reais do U3-SDK Linkadas

`U3.Runtime` já compila, via links para `external/U3-SDK`, estas fatias:

- `SystemEx`;
- core de `UnturnedDat`;
- core de `SDG.NetPak`;
- `SystemNetPakReaderEx` / `SystemNetPakWriterEx`;
- `UnityNetPakReaderEx` / `UnityNetPakWriterEx`;
- `UnityEx/QuaternionEx.cs`;
- `UnityEx/Vector3Ex.cs`;
- `UnityEx/MathfEx.cs`;
- `UnityEx/Vector2Ex.cs`;
- `UnityEx/ColorEx.cs`;
- `UnityEx/BoundsEx.cs`;
- `UnityEx/Matrix4x4Ex.cs`;
- `UnityEx/RandomEx.cs`;
- `UnityEx/GameObjectEx.cs`;
- `UnityEx/TransformEx.cs`;
- `UnityEx/ComponentEx.cs`;
- `UnityEx/RaycastHitEx.cs`;
- `UnityEx/BoxColliderEx.cs`;
- `UnityEx/SphereColliderEx.cs`;
- `UnityEx/CapsuleColliderEx.cs`.

## Demo Godot

O `GodotHost` cria uma cena simples com chão, alvos, player, câmera e luz. O player usa APIs fake Unity:

- `W/A/S/D`: mover;
- `LeftShift`: sprint;
- mouse: olhar;
- `Space`: pular quando grounded;
- `E` ou mouse esquerdo: raycast/interação;
- `Escape`: liberar mouse.

Para testar manualmente:

1. abra `godot/project.godot` no Godot 4 .NET/C#;
2. rode `scenes/Main.tscn`;
3. veja logs pelo `UnityEngine.Debug` redirecionado para Godot.

Isso é uma demo técnica, não gameplay real do Unturned.

## Estratégia

1. Manter `external/U3-SDK` intocado.
2. Expandir `UnityEngine.Shim` conforme fatias reais exigirem.
3. Linkar arquivos pequenos do U3-SDK em `U3.Runtime`, nunca copiar.
4. Criar smoke tests para cada fatia.
5. Usar `U3.GodotBridge` para conectar o runtime fake ao Godot.
6. Só avançar para assets, UI, cenas, rede e gameplay quando a base estiver estável.

## O Que Ainda Falta

Principais blocos pendentes:

- física real de `Rigidbody`, colisões e triggers;
- `CharacterController` mais próximo de Unity;
- input configurável;
- assets reais: `Resources`, `AssetBundle`, referências e carregamento de conteúdo;
- `SceneManager` real e fluxo de mundo;
- `Animator` funcional;
- UI real, TMPro e Glazier;
- Steamworks, NetTransport e networking pesado;
- gameplay systems do U3/Unturned: mundo, entidades, itens, players, interação e regras.

## Quão Perto Está da Gameplay?

Ainda não está perto de gameplay real, mas já passou da fase puramente estrutural.

Estimativa técnica atual:

- infraestrutura de port: média, com solução, testes, docs e fatias reais funcionando;
- runtime fake Unity: aproximadamente 35-45%;
- bridge Godot visual/física básica: aproximadamente 25-35%;
- fatia real do U3-SDK compilada: ainda menos de 10%;
- gameplay real: 0-5%, apenas uma demo técnica própria.

O avanço importante é que agora existe um loop mínimo jogável: input fake, mouse-look, movimento cinemático, jump simples, colliders, overlap/raycast Godot, interação e objetos visuais criados por dados `.dat` fake. Ainda falta praticamente todo o código de gameplay real do U3/Unturned.

## Próximos Passos Recomendados

1. Melhorar orientação de cápsula nas queries Godot.
2. Adicionar visualização/debug de collision shapes.
3. Adicionar input bindings configuráveis.
4. Avaliar a próxima fatia UnityEx pequena dependente de física passiva.
5. Mapear data classes do U3-SDK que possam compilar sem assets, UI, networking ou gameplay pesado.

## Regras de Contribuição

- Não modificar `external/U3-SDK` diretamente.
- Não copiar arquivos do U3-SDK; usar links no `.csproj`.
- Não converter para GDScript.
- Não portar o SDK inteiro de uma vez.
- Não implementar gameplay real sem fatia planejada.
- Toda fatia nova deve compilar, ter testes ou documentação dos erros, e manter escopo pequeno.
