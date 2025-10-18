using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace SpellShortcut
{
    [BepInPlugin("com.harukadev.spellshortcut", "SpellShortcut", "1.1.0")]
    [BepInProcess("MageArena.exe")]
    public class SpellShortcutMod : BaseUnityPlugin
    {
        private static ManualLogSource? LoggerInstance;
        private readonly Harmony harmony = new("com.harukadev.spellshortcut");

        private VoiceControlListener? _voiceListener;

        /// <summary>
        /// Initializes the plugin by setting up the logger instance, applying Harmony patches,
        /// and logging a confirmation message indicating successful loading of the plugin.
        /// </summary>
        private void Awake()
        {
            LoggerInstance = Logger;
            harmony.PatchAll();
            Logger.LogInfo("SpellShortcut loaded!");
        }

        /// <summary>
        /// Processes game loop logic to handle user input events for selecting spells,
        /// casting specific spells, or performing a healing action for the player.
        /// </summary>
        private void Update()
        {
            _voiceListener ??= FindFirstObjectByType<VoiceControlListener>();
            var mageBook = FindFirstObjectByType<MageBookController>();

            if (_voiceListener is null || mageBook is null) return;

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
        }

        /// <summary>
        /// Forces the selection of a specific spell page in the MageBookController
        /// and triggers a server-side flipping logic.
        /// </summary>
        /// <param name="mageBook">The MageBookController instance used to manage spell pages.</param>
        /// <param name="page">The number of the page to be forcefully selected.</param>
        private static void ForceSelectSpell(MageBookController mageBook, int page)
        {
            try
            {
                var last = mageBook.LastPressedPage;
                mageBook.LastPressedPage = page;
                var method = mageBook.GetType().GetMethod("ForceFlipServer",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                method?.Invoke(mageBook, [page, last]);
            }
            catch (Exception ex)
            {
                LoggerInstance?.LogError($"[ForceSelectSpell] Error forcing spell page: {ex}");
            }
        }

        /// <summary>
        /// Attempts to invoke a specific spell-casting method and handles any exceptions
        /// that occur during the execution, logging appropriate error messages.
        /// </summary>
        /// <param name="castMethod">The delegate method responsible for executing the spell cast logic. Can be null.</param>
        /// <param name="spellName">The identifier for the spell being cast, used for logging purposes.</param>
        private static void TryCast(Action? castMethod, string spellName)
        {
            try
            {
                castMethod?.Invoke();
            }
            catch (Exception e)
            {
                LoggerInstance?.LogError($"[TryCast] Error casting {spellName}: {e}");
            }
        }
    }
}