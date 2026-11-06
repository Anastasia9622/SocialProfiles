using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialProfiles.Core.Domain.Profiles;

namespace SocialProfiles.Core.Domain.Presets;

public class DarkPreset : ThemePreset
{
    public override string Name => "Нічна";

    public override Theme CreateTheme() =>
        new Theme("#1b1033", "#2a1a4d", "#f3eaff", "#ff5fa2");
}