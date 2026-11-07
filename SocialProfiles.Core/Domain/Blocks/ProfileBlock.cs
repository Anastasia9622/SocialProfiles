using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialProfiles.Core.Domain.Exceptions;

namespace SocialProfiles.Core.Domain.Blocks;

public abstract class ProfileBlock
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public int Position { get; internal set; }
    public bool IsVisible { get; private set; } = true;

    public abstract string Title { get; }
    public virtual bool CanBeHidden => true;

    public void SetVisibility(bool visible)
    {
        if (!visible && !CanBeHidden)
            throw new DomainException($"Блок «{Title}» не можна приховати.");

        IsVisible = visible;
    }
}