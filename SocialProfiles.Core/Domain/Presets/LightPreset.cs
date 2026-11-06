using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialProfiles.Core.Domain.Profiles;

namespace SocialProfiles.Core.Domain.Presets;

public class LightPreset : ThemePreset
{
    public override string Name => "Світла";

    public override Theme CreateTheme() =>
        new Theme("#ffffff", "#f1f1f6", "#1b1033", "#6a3fd1");
}