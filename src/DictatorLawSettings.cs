using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v2;
using MCM.Abstractions.Base.Global;

namespace DictatorLaw
{
    public class DictatorLawSettings : AttributeGlobalSettings<DictatorLawSettings>
    {
        public override string Id => "DictatorLawSettings";
        public override string DisplayName => new TaleWorlds.Localization.TextObject("{=dictator_law_settings_name}Dictator law").ToString();
        public override string FolderName => "DictatorLaw";
        public override string FormatType => "json2";

        [SettingPropertyInteger("{=dictator_law_settings_penalty_name}Influence penalty", -100, 0, "0", Order = 1, RequireRestart = false, HintText = "{=dictator_law_settings_penalty_hint}Influence penalty for the ruler when dictator law is active.")]
        [SettingPropertyGroup("{=dictator_law_settings_group}General")]
        public int InfluencePenalty { get; set; } = -10;

        [SettingPropertyBool("{=dictator_law_settings_block_armies}Block Vassal Armies", Order = 2, RequireRestart = false, HintText = "{=dictator_law_settings_block_armies_hint}If enabled, vassals cannot create their own armies while the law is active.")]
        [SettingPropertyGroup("{=dictator_law_settings_group_restrictions}Restrictions")]
        public bool BlockVassalArmies { get; set; } = true;

        [SettingPropertyBool("{=dictator_law_settings_block_war}Block War Declarations", Order = 3, RequireRestart = false, HintText = "{=dictator_law_settings_block_war_hint}If enabled, vassals cannot propose declaring war.")]
        [SettingPropertyGroup("{=dictator_law_settings_group_restrictions}Restrictions")]
        public bool BlockDeclareWar { get; set; } = true;

        [SettingPropertyBool("{=dictator_law_settings_block_peace}Block Peace Proposals", Order = 4, RequireRestart = false, HintText = "{=dictator_law_settings_block_peace_hint}If enabled, vassals cannot propose making peace.")]
        [SettingPropertyGroup("{=dictator_law_settings_group_restrictions}Restrictions")]
        public bool BlockMakePeace { get; set; } = true;

        [SettingPropertyBool("{=dictator_law_settings_block_policy}Block Policy Changes", Order = 5, RequireRestart = false, HintText = "{=dictator_law_settings_block_policy_hint}If enabled, vassals cannot propose changing kingdom policies.")]
        [SettingPropertyGroup("{=dictator_law_settings_group_restrictions}Restrictions")]
        public bool BlockKingdomPolicy { get; set; } = true;

        [SettingPropertyBool("{=dictator_law_settings_block_settlement}Block Settlement Claims", Order = 6, RequireRestart = false, HintText = "{=dictator_law_settings_block_settlement_hint}If enabled, vassals cannot propose claiming settlements.")]
        [SettingPropertyGroup("{=dictator_law_settings_group_restrictions}Restrictions")]
        public bool BlockSettlementClaim { get; set; } = true;

        [SettingPropertyBool("{=dictator_law_settings_block_expulsion}Block Clan Expulsion", Order = 7, RequireRestart = false, HintText = "{=dictator_law_settings_block_expulsion_hint}If enabled, vassals cannot propose expelling clans from the kingdom.")]
        [SettingPropertyGroup("{=dictator_law_settings_group_restrictions}Restrictions")]
        public bool BlockExpelClan { get; set; } = true;

        [SettingPropertyBool("{=dictator_law_settings_block_king}Block King Elections", Order = 8, RequireRestart = false, HintText = "{=dictator_law_settings_block_king_hint}If enabled, vassals cannot propose king elections.")]
        [SettingPropertyGroup("{=dictator_law_settings_group_restrictions}Restrictions")]
        public bool BlockKingSelection { get; set; } = true;

        [SettingPropertyBool("{=dictator_law_settings_block_diplomacy}Block Alliances and Trade", Order = 9, RequireRestart = false, HintText = "{=dictator_law_settings_block_diplomacy_hint}If enabled, vassals cannot propose alliances, trade agreements, or call to war agreements.")]
        [SettingPropertyGroup("{=dictator_law_settings_group_restrictions}Restrictions")]
        public bool BlockDiplomacy { get; set; } = true;
    }
}
