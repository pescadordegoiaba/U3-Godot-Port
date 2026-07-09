# U3-Godot-Port

Este projeto porta gradualmente o U3-SDK para rodar sem Unity, usando Godot 4 C# como backend futuro.

A estratégia inicial é criar uma camada `UnityEngine.Shim` com APIs compatíveis o suficiente para permitir que partes do U3-SDK compilem por etapas. Os shims começam como stubs seguros e ganham comportamento real conforme a necessidade do port.

Arquivos em `external/U3-SDK` não devem ser modificados diretamente. O SDK original deve ser consumido por projetos de adaptação em `src`.

Ordem inicial de implementação:

1. Solução .NET base.
2. `UnityEngine.Shim` básico.
3. `GameObject`, `Component`, `MonoBehaviour` e `Transform`.
4. Tipos matemáticos e utilitários: `Vector2`, `Vector3`, `Quaternion`, `Color`, `Mathf` e `Time`.
5. Coroutine fake.
6. `U3.Runtime` compilando parcialmente.
7. Bridge Godot futura.
