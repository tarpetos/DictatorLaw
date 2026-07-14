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
    }
}
