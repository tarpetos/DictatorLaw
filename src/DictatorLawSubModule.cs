using System;
using System.IO;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ObjectSystem;

namespace DictatorLaw
{
    public class DictatorLawSubModule : MBSubModuleBase
    {
        private Harmony _harmony;

        private static readonly string LogPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "..", "LocalLow", "DictatorLawDebug.log");

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

            try
            {
                // Harmony.DEBUG = true;

                WriteLog("=== DictatorLaw: OnSubModuleLoad start ===");

                _harmony = new Harmony("dictatorlaw.bannerlord.v147.stable");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());

                LogPatchedMethods();
            }
            catch (Exception ex)
            {
                WriteLog("EXCEPTION during PatchAll: " + ex);
            }
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);

            CampaignGameStarter campaignGameStarter = gameStarterObject as CampaignGameStarter;
            if (campaignGameStarter != null)
            {
                WriteLog("DictatorLaw: OnGameStart - Resetting per-session policy cache.");
                DictatorLawPolicyHelper.ResetSessionCache();

                WriteLog("DictatorLaw: OnGameStart - Registering policy.");
                DictatorLawPolicyHelper.RegisterDictatorLawPolicy();

                WriteLog("DictatorLaw: OnGameStart - Registering DictatorLawCampaignBehavior.");
                campaignGameStarter.AddBehavior(new DictatorLawCampaignBehavior());

                DumpAllDictatorLawPolicyObjects("OnGameStart (after registration)");
            }
        }

        private void LogPatchedMethods()
        {
            try
            {
                var patchedMethods = _harmony.GetPatchedMethods();
                bool foundAddDecision = false;
                bool foundCreateArmy = false;
                bool foundIsAllowed = false;
                bool foundInfluenceCost = false;

                foreach (MethodBase method in patchedMethods)
                {
                    string line = "Patched: " + method.DeclaringType + "." + method.Name +
                                  "(" + string.Join(", ", Array.ConvertAll(method.GetParameters(), p => p.ParameterType.Name)) + ")";
                    WriteLog(line);

                    if (method.Name == "AddDecision") foundAddDecision = true;
                    if (method.Name == "CreateArmy") foundCreateArmy = true;
                    if (method.Name == "IsAllowed") foundIsAllowed = true;
                    if (method.Name == "GetProposalInfluenceCost") foundInfluenceCost = true;
                }

                WriteLog("--- RESULT ---");
                WriteLog("AddDecision patched: " + foundAddDecision);
                WriteLog("CreateArmy patched: " + foundCreateArmy);
                WriteLog("IsAllowed patched (any override): " + foundIsAllowed);
                WriteLog("GetProposalInfluenceCost patched (any override): " + foundInfluenceCost);
                WriteLog("=== DictatorLaw: OnSubModuleLoad end ===");
            }
            catch (Exception ex)
            {
                WriteLog("EXCEPTION during LogPatchedMethods: " + ex);
            }
        }

        private static void DumpAllDictatorLawPolicyObjects(string context)
        {
            try
            {
                if (MBObjectManager.Instance == null)
                {
                    WriteLog($"DumpAllDictatorLawPolicyObjects [{context}]: MBObjectManager.Instance not ready.");
                    return;
                }

                var allPolicies = MBObjectManager.Instance.GetObjectTypeList<PolicyObject>();
                int matchCount = 0;

                if (allPolicies != null)
                {
                    foreach (PolicyObject policy in allPolicies)
                    {
                        if (policy != null && policy.StringId != null && policy.StringId.StartsWith(DictatorLawPolicyHelper.PolicyId))
                        {
                            matchCount++;
                            WriteLog(
                                $"DumpAllDictatorLawPolicyObjects [{context}]: match #{matchCount}, " +
                                $"hash={policy.GetHashCode()}, instance={policy.Id}");
                        }
                    }
                }

                WriteLog($"DumpAllDictatorLawPolicyObjects [{context}]: total matches found = {matchCount}");
            }
            catch (Exception ex)
            {
                WriteLog($"DumpAllDictatorLawPolicyObjects [{context}]: EXCEPTION: " + ex);
            }
        }

        public static void WriteLog(string message)
        {
            try
            {
//                File.AppendAllText(LogPath, DateTime.Now.ToString("HH:mm:ss.fff") + " | " + message + Environment.NewLine);
            }
            catch
            {
            }
        }
    }
}