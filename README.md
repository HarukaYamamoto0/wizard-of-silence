# wizard-of-silence

# 🪄 SpellShortcut — Mouse-Casting Mod for Mage Arena

> **"Why chant your spells when you can just *click*?"**

SpellShortcut é um mod para **Mage Arena**, desenvolvido com **BepInEx** e **Harmony**, que permite lançar feitiços e se curar diretamente pelos botões do mouse — sem menus, sem voz, sem enrolação.

## ✨ Funcionalidades

| Ação | Botão | Efeito |
|------|--------|---------|
| 🔥 Lançar Fireball | **Mouse0 (Esquerdo)** | Invoca um poderoso projétil de fogo diretamente |
| ❄️ Lançar Frostbolt | **Mouse1 (Direito)** | Atira uma rajada de gelo para congelar seus inimigos |
| 🥣 Beber Sopa de Madeira | **Mouse3 (Botão lateral)** | Restaura instantaneamente **150 HP** — a cura dos bravos |

## ⚙️ Como funciona

SpellShortcut intercepta o loop de entrada da Unity (`Update()`) e:
1. Detecta as instâncias de `VoiceControlListener` e `MageBookController`;
2. Usa reflexão para forçar a página correta do grimório (`ForceFlipServer`);
3. Invoca diretamente os métodos internos de cast (`CastFireball`, `CastFrostBolt`);
4. No botão lateral, acessa o jogador (`PlayerMovement`) e chama `nonnetworkedheal(150f)`.

Isso elimina completamente a dependência de comandos de voz — ideal para quem prefere gameplay ágil e silencioso.

## 📦 Instalação

1. Certifique-se de ter o **BepInEx 5.4+** instalado em `MageArena.exe`.
2. Copie o arquivo `SpellShortcut.dll` para a pasta:
```

MageArena/BepInEx/plugins/

````
3. Inicie o jogo normalmente.
4. Veja a mensagem `SpellShortcut loaded!` no console do BepInEx — e comece a clicar.

## 🧙‍♀️ Requisitos

- **Jogo:** Mage Arena  
- **Framework:** [BepInEx 5.x](https://github.com/BepInEx/BepInEx)  
- **Biblioteca:** [Harmony 2.x](https://github.com/pardeike/Harmony)  
- **Compatibilidade:** Windows x64 / Unity 2023+  

## 💀 Código-fonte

```csharp
if (Input.GetKeyDown(KeyCode.Mouse0))
{
 ForceSelectSpell(mageBook, 1);
 TryCast(_voiceListener.CastFireball, "fireball");
}
else if (Input.GetKeyDown(KeyCode.Mouse1))
{
 ForceSelectSpell(mageBook, 2);
 TryCast(_voiceListener.CastFrostBolt, "frostbolt");
}
else if (Input.GetKeyDown(KeyCode.Mouse3))
{
 var player = FindFirstObjectByType<PlayerMovement>();
 player.nonnetworkedheal(150f);
}
````

Três linhas, três poderes.
Simplicidade é magia pura.

## 🧪 Desenvolvimento

Este mod foi construído usando:

* **C# 11**
* **.NET Framework 4.7.2**
* **Unity API 2023.x**
* **BepInEx Core**
* **HarmonyLib** (para patching)

Compile com o SDK configurado para `x64` e referencie:

* `UnityEngine.dll`
* `BepInEx.dll`
* `HarmonyLib.dll`
* As DLLs de Mage Arena (`Assembly-CSharp.dll`, `UnityEngine.CoreModule.dll`)

## 🩸 Licença

Este projeto é disponibilizado sob a **MIT License**.
Use, modifique e distribua livremente — apenas lembre de dar crédito ao autor original: **harukadev** 🦇

## ☕ Créditos

* **Desenvolvimento:** harukadev
* **Motor:** Unity 2023
* **Framework:** BepInEx + Harmony
* **Inspiração:** “Eu só queria clicar pra soltar magia.”

### 💬 Nota final

> *“Fireball com o esquerdo, Frostbolt com o direito, e uma boa sopa quando tudo dá errado.”*
> — *harukadev, 2025*
