using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Election;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace DictatorLaw
{
    internal static class DictatorLawPolicyHelper
    {
        internal const string PolicyId = "dictator_law";

        internal static void RegisterDictatorLawPolicy()
        {
            Campaign campaign = Campaign.Current;
            if (campaign == null || campaign.CampaignObjectManager == null)
            {
                DictatorLawSubModule.WriteLog("RegisterDictatorLawPolicy: Campaign or CampaignObjectManager is null.");
                return;
            }

            PolicyObject policy = campaign.CampaignObjectManager.Find<PolicyObject>(PolicyId);
            bool isNewPolicy = policy == null;

            DictatorLawSubModule.WriteLog($"RegisterDictatorLawPolicy: isNewPolicy={isNewPolicy}");

            if (isNewPolicy)
            {
                policy = new PolicyObject(PolicyId);
            }

            policy.Initialize(
                new TextObject("{=dictator_law_name}Dictator Law"),
                new TextObject("{=dictator_law_description}The ruler holds sole authority over the kingdom's decisions"),
                new TextObject("{=dictator_law_secondary_effects}centralized rule"),
                new TextObject("{=dictator_law_details}Vassals cannot propose kingdom decisions.{newline}Vassals cannot form armies."),
                1f,
                0f,
                0f);

            if (isNewPolicy)
            {
                MBObjectManager.Instance.RegisterObject(policy);
                DictatorLawSubModule.WriteLog("RegisterDictatorLawPolicy: Policy registered in MBObjectManager.");
            }
        }

        internal static PolicyObject FindDictatorLawPolicy()
        {
            Campaign campaign = Campaign.Current;
            if (campaign == null || campaign.CampaignObjectManager == null)
            {
                return null;
            }

            return campaign.CampaignObjectManager.Find<PolicyObject>(PolicyId);
        }

        internal static bool IsDictatorLawActive(Kingdom kingdom)
        {
            if (kingdom == null || kingdom.ActivePolicies == null)
            {
                return false;
            }

            foreach (PolicyObject activePolicy in kingdom.ActivePolicies)
            {
                if (activePolicy != null && activePolicy.StringId == PolicyId)
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
            if (playerKingdom == null || kingdom == null || playerKingdom != kingdom)
            {
                return false;
            }

            return IsDictatorLawActive(kingdom);
        }

        internal static bool IsDictatorLawDecision(KingdomPolicyDecision decision)
        {
            return decision != null
                && decision.Policy != null
                && decision.Policy.StringId == PolicyId;
        }

        internal static bool IsRulerClan(Kingdom kingdom, Clan clan)
        {
            return kingdom != null && clan != null && kingdom.RulingClan == clan;
        }
    }
}