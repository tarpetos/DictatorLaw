using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace DictatorLaw
{
    internal static class DictatorLawPolicyHelper
    {
        internal const string PolicyId = "dictator_law";
        private static PolicyObject _cachedPolicy;

        internal static void ResetSessionCache()
        {
            DictatorLawSubModule.WriteLog(
                $"ResetSessionCache: clearing cached policy (was {(_cachedPolicy == null ? "null" : "non-null")}).");
            _cachedPolicy = null;
        }

        internal static void RegisterDictatorLawPolicy()
        {
            Campaign campaign = Campaign.Current;
            if (campaign == null || campaign.CampaignObjectManager == null)
            {
                DictatorLawSubModule.WriteLog("RegisterDictatorLawPolicy: Campaign or CampaignObjectManager is null.");
                return;
            }

            if (_cachedPolicy == null)
            {
                _cachedPolicy = campaign.CampaignObjectManager.Find<PolicyObject>(PolicyId);
                DictatorLawSubModule.WriteLog(
                    $"RegisterDictatorLawPolicy: cache was empty, CampaignObjectManager.Find returned {(_cachedPolicy == null ? "null" : "an object")}.");
            }
            else
            {
                DictatorLawSubModule.WriteLog("RegisterDictatorLawPolicy: using already-cached policy object for this session.");
            }

            bool isNewPolicy = _cachedPolicy == null;

            DictatorLawSubModule.WriteLog($"RegisterDictatorLawPolicy: isNewPolicy={isNewPolicy}");

            if (isNewPolicy)
            {
                _cachedPolicy = new PolicyObject(PolicyId);
            }

            _cachedPolicy.Initialize(
                new TextObject("{=dictator_law_name}Dictator Law"),
                new TextObject("{=dictator_law_description}The ruler holds sole authority over the kingdom's decisions"),
                new TextObject("{=dictator_law_secondary_effects}centralized rule"),
                new TextObject("{=dictator_law_details}Vassals cannot propose kingdom decisions.{newline}Vassals cannot form armies.{newline}Takes influence from the ruler."),
                1f,
                0f,
                0f);

            if (isNewPolicy)
            {
                MBObjectManager.Instance.RegisterObject(_cachedPolicy);
                DictatorLawSubModule.WriteLog("RegisterDictatorLawPolicy: Policy registered in MBObjectManager.");
            }
        }

        internal static PolicyObject FindDictatorLawPolicy()
        {
            if (_cachedPolicy != null)
            {
                return _cachedPolicy;
            }

            Campaign campaign = Campaign.Current;
            if (campaign == null || campaign.CampaignObjectManager == null)
            {
                return null;
            }

            PolicyObject found = campaign.CampaignObjectManager.Find<PolicyObject>(PolicyId);
            DictatorLawSubModule.WriteLog(
                $"FindDictatorLawPolicy: cache was empty, CampaignObjectManager.Find returned {(found == null ? "null" : "an object")}.");
            return found;
        }

        internal static bool IsDictatorLawActive(Kingdom kingdom)
        {
            if (kingdom == null || kingdom.ActivePolicies == null)
            {
                DictatorLawSubModule.WriteLog("IsDictatorLawActive: kingdom or ActivePolicies null.");
                return false;
            }

            string dump = string.Join(", ", System.Linq.Enumerable.Select(kingdom.ActivePolicies,
                p => p == null ? "<null>" : $"{p.StringId}(id={p.Id})"));
            DictatorLawSubModule.WriteLog($"IsDictatorLawActive: kingdom={kingdom.StringId}, ActivePolicies=[{dump}]");

            foreach (PolicyObject activePolicy in kingdom.ActivePolicies)
            {
                if (activePolicy != null && (activePolicy == _cachedPolicy || activePolicy.StringId == PolicyId || activePolicy.StringId.StartsWith(PolicyId)))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsDictatorLawActiveForPlayerKingdom(Kingdom kingdom)
        {
            Clan playerClan = Clan.PlayerClan;
            Kingdom playerKingdom = playerClan != null ? playerClan.Kingdom : null;

            if (playerKingdom == null)
            {
                DictatorLawSubModule.WriteLog("IsDictatorLawActiveForPlayerKingdom: playerKingdom is null.");
                return false;
            }

            if (kingdom == null)
            {
                DictatorLawSubModule.WriteLog("IsDictatorLawActiveForPlayerKingdom: kingdom arg is null.");
                return false;
            }

            if (playerKingdom != kingdom)
            {
                DictatorLawSubModule.WriteLog(
                    $"IsDictatorLawActiveForPlayerKingdom: KINGDOM MISMATCH. " +
                    $"playerKingdom.StringId={playerKingdom.StringId}, hash={playerKingdom.GetHashCode()}, id={playerKingdom.Id} | " +
                    $"argKingdom.StringId={kingdom.StringId}, hash={kingdom.GetHashCode()}, id={kingdom.Id}");
                return false;
            }

            bool result = IsDictatorLawActive(kingdom);
            DictatorLawSubModule.WriteLog($"IsDictatorLawActiveForPlayerKingdom: kingdoms match, IsDictatorLawActive={result}");
            return result;
        }

        internal static bool IsDictatorLawDecision(KingdomPolicyDecision decision)
        {
            return decision != null
                && decision.Policy != null
                && decision.Policy.StringId != null
                && decision.Policy.StringId.StartsWith(PolicyId);
        }

        internal static bool IsRulerClan(Kingdom kingdom, Clan clan)
        {
            return kingdom != null && clan != null && kingdom.RulingClan == clan;
        }
    }
}