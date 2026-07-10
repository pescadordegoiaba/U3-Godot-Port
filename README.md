# U3-Godot-Port

Port gradual do U3-SDK para rodar sem Unity, usando .NET 8 e Godot 4 C# como backend.

Este repositório não copia nem modifica o SDK original. O código upstream fica em `external/U3-SDK` como submodule apontando para `SmartlyDressedGames/U3-SDK`, e o port consome arquivos do SDK por links no projeto `src/U3.Runtime`.

## Estado Atual

O projeto já tem uma base compilável com:

- solução .NET `U3GodotPort.sln`;
- shim `UnityEngine.Shim` com `GameObject`, `Component`, `MonoBehaviour`, `Transform`, `RuntimeLoop`, `Debug`, `Time`, math básico, câmera, luz e render básico;
- `UnityEngine.UI.Shim` e `TMPro.Shim` como projetos reservados para etapas futuras;
- `U3.Runtime` compilando uma fatia real do U3-SDK;
- `U3.GodotBridge` com logger, loop runtime, sincronização básica `GameObject -> Node3D`, hierarquia, mesh, material, câmera e luz;
- projeto Godot C# mínimo em `godot/`;
- smoke tests cobrindo shim, NetPak, UnityNetPak e UnityEx math helpers.

Validação atual:

```bash
dotnet build U3GodotPort.sln
dotnet test U3GodotPort.sln
dotnet build godot/U3GodotPort.Godot.csproj
```

Último estado verificado: 70 testes passando.

## Fatias do U3-SDK Já Linkadas

`U3.Runtime` já compila, por links para `external/U3-SDK`, estas áreas pequenas e de baixo risco:

- `SystemEx`;
- core de `UnturnedDat`;
- core de `SDG.NetPak`;
- `SystemNetPakReaderEx` / `SystemNetPakWriterEx`;
- `UnityNetPakReaderEx` / `UnityNetPakWriterEx`;
- `UnityEx/QuaternionEx.cs`;
- `UnityEx/Vector3Ex.cs`;
- `UnityEx/MathfEx.cs`.

`MathfEx` agora usa o arquivo real do SDK. O shim adiciona apenas os tipos Unity mínimos exigidos por ele, como `Random.insideUnitCircle`.

## Estratégia

1. Manter `external/U3-SDK` intocado.
2. Expandir `UnityEngine.Shim` só quando uma fatia real do SDK exigir.
3. Linkar pequenos grupos de arquivos do U3-SDK em `U3.Runtime`.
4. Criar smoke tests para cada fatia linkada.
5. Usar `U3.GodotBridge` para mapear o runtime fake para Godot C#.
6. Só depois avançar para física, assets, UI, cenas e networking.

## Integração Godot Atual

O projeto Godot em `godot/` tem uma cena principal e um `GodotHost.cs` que:

- inicializa o runtime fake;
- instala logger Godot para `UnityEngine.Debug`;
- cria `GameObject`s fake;
- sincroniza `Transform` para `Node3D`;
- cria visual automático para `MeshFilter` + `MeshRenderer`;
- sincroniza câmera e luz simples.

Isso é um smoke test visual, não gameplay real.

## O Que Ainda Falta

Principais blocos pendentes:

- completar mais APIs matemáticas e helpers UnityEx;
- implementar ou simular `Random`, `Bounds`, `Matrix4x4`, `Ray`, `LayerMask` e tipos próximos;
- física: `Rigidbody`, `Collider`, `Physics`, raycasts e queries;
- assets: `Resources`, `AssetBundle`, referências de conteúdo e carregamento real;
- scenes: `SceneManager` e fluxo de mundo;
- animação: `Animator` e estados básicos;
- UI: `UnityEngine.UI`, TMPro e Glazier;
- networking pesado, Steamworks e NetTransport;
- gameplay systems do U3/Unturned ainda não foram compilados.

## Quão Perto Está da Gameplay?

Ainda está longe de gameplay jogável.

Estimativa técnica atual:

- infraestrutura de port: baixa a média, já iniciada;
- runtime fake Unity básico: aproximadamente 20-30%;
- bridge visual Godot mínimo: aproximadamente 15-25%;
- fatia real do U3-SDK compilada: menos de 5%;
- gameplay real: 0%.

O que já existe prova que a abordagem funciona: partes reais do SDK compilam, testes rodam, e objetos fake aparecem no Godot. Mas gameplay exige compilar e adaptar grandes áreas que ainda não foram tocadas: assets, física, entidades, mundo, itens, players, interação, UI e rede.

## Próximos Passos Recomendados

1. Avaliar `UnityEx/ColorEx.cs` ou `UnityEx/Vector2Ex.cs` como próxima fatia pequena.
2. Expandir `UnturnedDat` com helpers UnityDat de baixo risco.
3. Mapear próxima fatia de data classes sem `MonoBehaviour`, física, UI ou assets pesados.
4. Só depois iniciar stubs de física e assets.

## Regras de Contribuição

- Não modificar `external/U3-SDK` diretamente.
- Não converter para GDScript.
- Não tentar portar o SDK inteiro de uma vez.
- Toda fatia nova deve compilar, ter testes ou documentação dos erros, e manter o escopo pequeno.
