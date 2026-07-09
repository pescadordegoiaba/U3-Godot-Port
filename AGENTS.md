# Projeto: U3-SDK para Godot C#

## Objetivo

Portar gradualmente o U3-SDK para rodar sem Unity, usando Godot 4 C# como backend.

## Estratégia obrigatória

Não reescrever o U3-SDK inteiro.
Não converter para GDScript.
Não modificar arquivos dentro de external/U3-SDK, exceto se explicitamente pedido.
Criar uma camada de compatibilidade UnityEngine.Shim.
Fazer o código compilar por etapas.
Preferir stubs seguros antes de implementação real.

## Estrutura

- src/UnityEngine.Shim: classes fake compatíveis com UnityEngine.
- src/UnityEngine.UI.Shim: classes fake para UnityEngine.UI.
- src/TMPro.Shim: classes fake para TextMeshPro.
- src/U3.Runtime: projeto que referencia arquivos C# do U3-SDK.
- src/U3.GodotBridge: adaptação entre UnityEngine.Shim e Godot.
- godot: projeto Godot 4 C#.

## Regra principal

Toda tarefa precisa terminar com:
1. build passando ou relatório exato dos erros restantes;
2. lista dos arquivos alterados;
3. próximo passo recomendado.

## Ordem de implementação

1. Solução .NET.
2. UnityEngine.Shim básico.
3. GameObject, Component, MonoBehaviour, Transform.
4. Vector2, Vector3, Quaternion, Color, Mathf, Time.
5. Coroutine fake.
6. U3.Runtime compilando parcialmente.
7. Godot host chamando loop fake.
8. Física básica.
9. Assets.
10. UI.
11. Networking.
