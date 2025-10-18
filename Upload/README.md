# 🪄 Wizard Of Silence

> **"Why chant your spells when you can just *click*?"**

**Wizard Of Silence** (class name: `SpellShortcut`) is a mod for **Mage Arena** built with **BepInEx** and **Harmony**.  
It allows you to cast spells and heal yourself directly using your mouse buttons — no menus, no voice commands, no nonsense.

Originally, I wanted it to cast **Magic Missile**, but its logic turned out to be more complex than I expected.  
If someone wants to make a pull request adding that feature, I’d be *immensely grateful*. ❤️  
Also, keep in mind: even though there’s healing, **you can still die** — this mod doesn’t make you immortal.

## ✨ Features

| Action | Button | Effect |
|--------|---------|--------|
| 🔥 Cast Fireball | **Mouse0 (Left Click)** | Launches a powerful fire projectile instantly |
| ❄️ Cast Frostbolt | **Mouse1 (Right Click)** | Shoots a freezing projectile to slow your enemies |
| 🥣 Drink Wooden Soup | **Mouse3 (Side Button)** | Instantly restores **150 HP** — the brave man’s heal |

## ⚙️ How It Works

`SpellShortcut` hooks into Unity’s main update loop (`Update()`) and:

1. Finds the game’s instances of `VoiceControlListener` and `MageBookController`.  
2. Uses reflection to force the correct spellbook page (`ForceFlipServer`).  
3. Invokes the internal casting methods (`CastFireball`, `CastFrostBolt`).  
4. On the side mouse button, accesses the player (`PlayerMovement`) and calls `nonnetworkedheal(150f)`.

This completely removes the dependency on voice commands — perfect for players who prefer **quiet, fast, and click-based magic**.

## 📦 Installation

1. Make sure **BepInEx 5.4+** is installed in your `MageArena.exe` directory.  
2. Copy the compiled `SpellShortcut.dll` file into:
```

MageArena/BepInEx/plugins/

```
3. Launch the game.  
4. Look for the console message:  
```

[Info : BepInEx] SpellShortcut loaded!

````
Then… start clicking.

## 🧙‍♀️ Requirements

- **Game:** Mage Arena  
- **Framework:** [BepInEx 5.x](https://github.com/BepInEx/BepInEx)  
- **Library:** [Harmony 2.x](https://github.com/pardeike/Harmony)  
- **Compatible with:** Windows x64 / Unity 2023+

## 💀 Core Logic Example

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

Three lines. Three powers.
**Simplicity is true magic.**

## 🧪 Development

This mod was built using:

* **C# 11**
* **.NET Framework 4.7.2**
* **Unity API 2023.x**
* **BepInEx Core**
* **HarmonyLib** (for patching)

When compiling, make sure your SDK is set to `x64` and reference the following assemblies:

* `UnityEngine.dll`
* `BepInEx.dll`
* `HarmonyLib.dll`
* Mage Arena’s own assemblies:

  * `Assembly-CSharp.dll`
  * `UnityEngine.CoreModule.dll`

## 🩸 License

This project is released under the **MIT License**.
You’re free to use, modify, and distribute it — just give proper credit to the original author: **harukadev** 🦇

## ☕ Credits

* **Developer:** harukadev
* **Engine:** Unity 2023
* **Framework:** BepInEx + Harmony
* **Inspiration:** “I just wanted to click to cast magic.”

### 💬 Final Note

> *“Fireball on the left, Frostbolt on the right, and a good soup when everything goes wrong.”*
> — *harukadev, 2025*
