using TaleWorlds.CampaignSystem;

namespace DictatorLaw
{
    public class DictatorLawCampaignBehavior : CampaignBehaviorBase
    {
        private bool _wasActiveForPlayerKingdom;

        public override void RegisterEvents()
        {
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
            CampaignEvents.OnGameLoadFinishedEvent.AddNonSerializedListener(this, OnGameLoadFinished);
            CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, OnNewGameCreated);
        }

        public override void SyncData(IDataStore dataStore)
        {
            if (dataStore.IsSaving)
            {
                Kingdom playerKingdomForSave = Clan.PlayerClan != null ? Clan.PlayerClan.Kingdom : null;
                if (playerKingdomForSave != null)
                {
                    _wasActiveForPlayerKingdom = DictatorLawPolicyHelper.IsDictatorLawActive(playerKingdomForSave);
                }

                DictatorLawSubModule.WriteLog($"SyncData (saving): _wasActiveForPlayerKingdom={_wasActiveForPlayerKingdom}");
            }

            dataStore.SyncData("dictator_law_was_active_for_player_kingdom", ref _wasActiveForPlayerKingdom);
        }

        private void OnNewGameCreated(CampaignGameStarter starter)
        {
            _wasActiveForPlayerKingdom = false;
        }

        private void OnDailyTick()
        {
            Kingdom playerKingdom = Clan.PlayerClan != null ? Clan.PlayerClan.Kingdom : null;
            if (playerKingdom != null)
            {
                _wasActiveForPlayerKingdom = DictatorLawPolicyHelper.IsDictatorLawActive(playerKingdom);
            }
        }

        private void OnGameLoadFinished()
        {
            DictatorLawSubModule.WriteLog($"OnGameLoadFinished: _wasActiveForPlayerKingdom={_wasActiveForPlayerKingdom}");

            if (!_wasActiveForPlayerKingdom)
            {
                DictatorLawSubModule.WriteLog("OnGameLoadFinished: Flag says policy was not active, nothing to restore.");
                return;
            }

            Kingdom playerKingdom = Clan.PlayerClan != null ? Clan.PlayerClan.Kingdom : null;
            if (playerKingdom == null)
            {
                DictatorLawSubModule.WriteLog("OnGameLoadFinished: Player has no kingdom, nothing to restore.");
                return;
            }

            if (DictatorLawPolicyHelper.IsDictatorLawActive(playerKingdom))
            {
                DictatorLawSubModule.WriteLog("OnGameLoadFinished: Policy reference survived the load, nothing to do.");
                return;
            }

            DictatorLawSubModule.WriteLog(
                $"OnGameLoadFinished: Policy reference was lost for kingdom '{playerKingdom.Name}', attempting restore.");

            DictatorLawPolicyHelper.RegisterDictatorLawPolicy();
            PolicyObject policy = DictatorLawPolicyHelper.FindDictatorLawPolicy();

            if (policy == null)
            {
                DictatorLawSubModule.WriteLog("OnGameLoadFinished: Could not find/register Dictator Law policy object, cannot restore.");
                return;
            }

            playerKingdom.AddPolicy(policy);

            bool verifyImmediate = DictatorLawPolicyHelper.IsDictatorLawActive(playerKingdom);
            DictatorLawSubModule.WriteLog(
                $"OnGameLoadFinished: post-AddPolicy verify on THIS playerKingdom instance (hash={playerKingdom.GetHashCode()}, id={playerKingdom.Id}) => IsDictatorLawActive={verifyImmediate}");

            DictatorLawSubModule.WriteLog(
                $"OnGameLoadFinished: Restored lost Dictator Law reference for kingdom '{playerKingdom.Name}' " +
                $"(policy hash={policy.GetHashCode()}, instance={policy.Id}).");
        }
    }
}