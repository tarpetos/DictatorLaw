using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;

namespace DictatorLaw
{
    [HarmonyPatch(typeof(Kingdom), "CreateArmy")]
    internal static class KingdomCreateArmyPatch
    {
        private static bool Prefix(Kingdom __instance, Hero armyLeader)
        {
            DictatorLawSubModule.WriteLog($"KingdomCreateArmyPatch.Prefix called for Leader: {armyLeader?.Name}");
            if (armyLeader == null)
            {
                return true;
            }

            Clan armyLeaderClan = armyLeader.Clan;
            bool isActive = DictatorLawPolicyHelper.IsDictatorLawActiveForPlayerKingdom(__instance);
            bool isRuler = DictatorLawPolicyHelper.IsRulerClan(__instance, armyLeaderClan);

            DictatorLawSubModule.WriteLog($"CreateArmy: IsActive={isActive}, IsRuler={isRuler}");

            return !isActive || isRuler;
        }
    }

    [HarmonyPatch]
    internal static class KingdomAddDecisionPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            foreach (MethodInfo method in typeof(Kingdom).GetMethods(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (method.Name == "AddDecision")
                {
                    yield return method;
                }
            }
        }

        private static bool Prefix(Kingdom __instance, KingdomDecision kingdomDecision)
        {
            DictatorLawSubModule.WriteLog($"KingdomAddDecisionPatch.Prefix called");
            if (kingdomDecision != null)
            {
                bool isActive = DictatorLawPolicyHelper.IsDictatorLawActiveForPlayerKingdom(__instance);
                bool isRuler = DictatorLawPolicyHelper.IsRulerClan(__instance, kingdomDecision.ProposerClan);

                DictatorLawSubModule.WriteLog($"AddDecision: DecisionType={kingdomDecision.GetType().Name}, IsActive={isActive}, IsRuler={isRuler}");

                if (isActive && !isRuler)
                {
                    DictatorLawSubModule.WriteLog("AddDecision: Blocked by Dictator Law.");
                    return false;
                }
            }

            return true;
        }
    }

    [HarmonyPatch]
    internal static class KingdomDecisionIsAllowedPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            return HarmonyReflectionHelper.GetAllOverrides(typeof(KingdomDecision), "IsAllowed");
        }

        private static void Postfix(KingdomDecision __instance, ref bool __result)
        {
            if (!__result || __instance == null)
            {
                return;
            }

            Kingdom kingdom = __instance.Kingdom;
            bool isActive = DictatorLawPolicyHelper.IsDictatorLawActiveForPlayerKingdom(kingdom);
            bool isRuler = DictatorLawPolicyHelper.IsRulerClan(kingdom, __instance.ProposerClan);

            if (isActive && !isRuler)
            {
                DictatorLawSubModule.WriteLog($"KingdomDecisionIsAllowedPatch: Blocked allowed state for {__instance.GetType().Name}");
                __result = false;
            }
        }
    }

    [HarmonyPatch]
    internal static class KingdomDecisionInfluenceCostPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            return HarmonyReflectionHelper.GetAllOverrides(typeof(KingdomDecision), "GetProposalInfluenceCost");
        }

        private static void Postfix(KingdomDecision __instance, ref int __result)
        {
            if (__instance == null)
            {
                return;
            }

            KingdomPolicyDecision policyDecision = __instance as KingdomPolicyDecision;
            if (policyDecision != null && DictatorLawPolicyHelper.IsDictatorLawDecision(policyDecision))
            {
                __result = 100;
                DictatorLawSubModule.WriteLog("InfluenceCostPatch: Cost set to 100 for Dictator Law decision.");
                return;
            }

            Kingdom kingdom = __instance.Kingdom;
            bool isActive = DictatorLawPolicyHelper.IsDictatorLawActiveForPlayerKingdom(kingdom);
            bool isRuler = DictatorLawPolicyHelper.IsRulerClan(kingdom, __instance.ProposerClan);

            if (isActive && !isRuler)
            {
                DictatorLawSubModule.WriteLog($"InfluenceCostPatch: Cost set to 9999999 for {__instance.GetType().Name} due to Dictator Law.");
                __result = 9999999;
            }
        }
    }

    [HarmonyPatch]
    internal static class ClanPoliticsModelPatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            Assembly campaignAssembly = typeof(Campaign).Assembly;
            foreach (Type type in campaignAssembly.GetTypes())
            {
                if (type.Name.Contains("ClanPoliticsModel"))
                {
                    MethodInfo method = AccessTools.Method(type, "CalculateInfluenceChange");
                    if (method != null && method.DeclaringType == type && !method.IsAbstract)
                    {
                        yield return method;
                    }
                }
            }
        }

        private static void Postfix(Clan clan, ref TaleWorlds.CampaignSystem.ExplainedNumber __result)
        {
            if (clan == null || clan.Kingdom == null)
            {
                return;
            }

            if (DictatorLawPolicyHelper.IsDictatorLawActive(clan.Kingdom) && DictatorLawPolicyHelper.IsRulerClan(clan.Kingdom, clan))
            {
                int penalty = GetPenaltySafe();

                if (penalty != 0)
                {
                    __result.Add(penalty, new TaleWorlds.Localization.TextObject("{=dictator_law_name}Dictator law"), null);
                }
            }
        }

        private static int GetPenaltySafe()
        {
            try
            {
                return GetMcmPenalty();
            }
            catch
            {
                return -10;
            }
        }

        private static int GetMcmPenalty()
        {
            if (DictatorLawSettings.Instance != null)
            {
                return DictatorLawSettings.Instance.InfluencePenalty;
            }
            return -10;
        }
    }
    internal static class HarmonyReflectionHelper
    {
        internal static IEnumerable<MethodBase> GetAllOverrides(Type baseType, string methodName)
        {
            HashSet<MethodBase> seen = new HashSet<MethodBase>();

            foreach (MethodInfo method in SafeGetMethodsByName(
                baseType, methodName, BindingFlags.Instance | BindingFlags.Public))
            {
                if (seen.Add(method))
                {
                    yield return method;
                }
            }

            Type[] types;
            try
            {
                types = baseType.Assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException exception)
            {
                types = exception.Types;
            }

            foreach (Type type in types)
            {
                if (type == null || type == baseType || !baseType.IsAssignableFrom(type))
                {
                    continue;
                }

                foreach (MethodInfo method in SafeGetMethodsByName(
                    type, methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                {
                    if (seen.Add(method))
                    {
                        yield return method;
                    }
                }
            }
        }

        private static IEnumerable<MethodInfo> SafeGetMethodsByName(Type type, string methodName, BindingFlags flags)
        {
            if (type == null)
            {
                yield break;
            }

            MethodInfo[] methods = null;
            try
            {
                methods = type.GetMethods(flags);
            }
            catch
            {
                methods = null;
            }

            if (methods == null)
            {
                yield break;
            }

            foreach (MethodInfo method in methods)
            {
                if (method.Name == methodName && !method.IsAbstract)
                {
                    yield return method;
                }
            }
        }
    }
}