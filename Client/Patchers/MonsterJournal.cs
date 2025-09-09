using Archipelago.MonsterSanctuary.Client.AP;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Archipelago.MonsterSanctuary.Client
{
    public partial class Patcher
    {
        #region Patches
        private static bool _inMonsterJournal = false;

        [HarmonyPatch(typeof(MonsterJournal), "OpenPage")]
        private class MonsterJournal_OpenPage
        {
            private static void Prefix()
            {
                _inMonsterJournal = true;
            }

            private static void Postfix()
            {
                _inMonsterJournal = false;
            }
        }

        [HarmonyPatch(typeof(MonsterJournal), "OnMenuItemHovered")]
        private class MonsterJournal_OnMenuItemHovered
        {
            private static void Prefix()
            {
                _inMonsterJournal = true;
            }

            private static void Postfix()
            {
                _inMonsterJournal = false;
            }
        }

        [HarmonyPatch(typeof(MonsterJournal), "ProcessLoreText")]
        private class MonsterJournal_ProcessLoreText
        {
            private static bool Prefix(MonsterJournal __instance, Monster monster, ref MonsterLore lore, ref List<string> ___TrimmedLore)
            {
                ___TrimmedLore.Clear();
                string text =
                    GameDefines.FormatTextAsInfo("Location:") + GameDefines.GetSpaceChar() + Monsters.GetMonsterJournalLocationText(monster)
                    + "\n\n"
                    + GameDefines.FormatTextAsInfo(Utils.LOCA("Bio:")) + GameDefines.GetSpaceChar() + Utils.LocalizeString(lore.Bio, monster.Name + "_Bio")
                    + "\n\n"
                    + GameDefines.FormatTextAsInfo(Utils.LOCA("History:")) + GameDefines.GetSpaceChar() + Utils.LocalizeString(lore.History, monster.Name + "_Lore");

                var traverse = Traverse.Create(__instance);
                if (OptionsManager.Instance.IsAsianLanguage())
                    traverse.Method("ProcessAsianText", text).GetValue();
                else
                    traverse.Method("ProcessTextSegment", text, 350).GetValue();

                __instance.Pager.SetActive(___TrimmedLore.Count > 1);

                return false;
            }
        }

        [HarmonyPatch(typeof(ProgressManager), "HasMonterEntry")]
        private class ProgressManager_HasMonterEntry
        {
            private static void Postfix(ref bool __result)
            {
                if (!ApState.IsConnected)
                    return;

                if (_inMonsterJournal)
                    __result = true;
            }
        }
        #endregion
    }
}