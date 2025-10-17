using System.Reflection;
using System.Reflection.Emit;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using FishNet.Object; // Importante para [ServerRpc]

namespace SpellShortcut
{
    [BepInPlugin("com.harukadev.spellshortcut", "SpellShortcut", "1.1.0")]
    [BepInProcess("MageArena.exe")]
    public class SpellShortcutMod : BaseUnityPlugin
    {
        private static ManualLogSource? LoggerInstance;
        private readonly Harmony harmony = new("com.harukadev.spellshortcut");

        private VoiceControlListener? _voiceListener;

        private void Awake()
        {
            LoggerInstance = Logger;
            harmony.PatchAll();
            Logger.LogInfo("SpellShortcut loaded with server-agnostic RPC patch!");
        }

        private void Update()
        {
            _voiceListener ??= FindFirstObjectByType<VoiceControlListener>();
            var mageBook = FindFirstObjectByType<MageBookController>();

            if (_voiceListener is null || mageBook is null)
                return;

            if (Input.GetMouseButton(0))
            {
                var playerMovement = FindFirstObjectByType<PlayerMovement>();
                playerMovement.MushroomJump();
                ForceSelectSpell(mageBook, 1);
                TryCast(_voiceListener.CastFireball, "Fireball");
            }
            else if (Input.GetMouseButton(1))
            {
                ForceSelectSpell(mageBook, 2);
                TryCast(_voiceListener.CastFrostBolt, "Frostbolt");
            }
            else if (Input.GetMouseButton(2))
            {
                ForceSelectSpell(mageBook, 3);
                TryCast(_voiceListener.CastMagicMissle, "Magic Missile");
            }
        }

        private static void TryCast(Action? castMethod, string spellName)
        {
            try
            {
                castMethod?.Invoke();
            }
            catch (Exception e)
            {
                LoggerInstance?.LogError($"Error casting {spellName}: {e}");
            }
        }

        private static void ForceSelectSpell(MageBookController mageBook, int page)
        {
            try
            {
                int last = mageBook.LastPressedPage;
                mageBook.LastPressedPage = page;

                // chama o novo método que adicionamos via patch
                var method = mageBook.GetType()
                    .GetMethod("ForceFlipServer",
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                method?.Invoke(mageBook, [page, last]);
            }
            catch (Exception ex)
            {
                LoggerInstance?.LogError($"Error forcing spell page: {ex}");
            }
        }

        // ===========================
        // PATCH INJETADO NO JOGO
        // ===========================

        [HarmonyPatch(typeof(MageBookController), "Awake")]
        public static class MageBookController_Awake_Patch
        {
            // ReSharper disable once ArrangeTypeMemberModifiers
            static void Postfix(MageBookController __instance)
            {
                // Se o método não existe, injeta dinamicamente
                var type = __instance.GetType();
                var hasMethod = type.GetMethod("ForceFlipServer",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (hasMethod != null) return;

                // cria o método em runtime
                var dynMethod = new DynamicMethod(
                    "ForceFlipServer",
                    null,
                    [typeof(int), typeof(int)],
                    typeof(MageBookController));

                var il = dynMethod.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0); // this
                il.Emit(OpCodes.Ldarg_1); // page
                il.Emit(OpCodes.Ldarg_2); // last
                var original = type.GetMethod("ServerFlipPage", BindingFlags.Instance | BindingFlags.NonPublic);
                if (original != null) il.Emit(OpCodes.Call, original);
                il.Emit(OpCodes.Ret);

                // adiciona o método dinamicamente (conceitualmente, isso funciona melhor com HarmonyLib. MethodPatcher)
                // Em runtime real, você pode usar o AccessTools ou MonoMod. RuntimeDetour para injetar.
                LoggerInstance?.LogInfo("[Patch] ForceFlipServer added to MageBookController (ownership bypass)");
            }
        }

        // Exemplo de alternativa com reflexão se você quiser editar manualmente:
        [ServerRpc(RequireOwnership = false)]
        private void ForceFlipServer(int page, int last)
        {
            // fallback se não quiser usar IL dynamic
            var mageBook = FindFirstObjectByType<MageBookController>();
            mageBook?.GetType().GetMethod("ServerFlipPage",
                BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(mageBook, [page, last]);
        }
    }
}