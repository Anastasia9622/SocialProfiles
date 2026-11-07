using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SocialProfiles.Core.Domain.Blocks;

public class CommentsBlock : ProfileBlock
{
    public override string Title => "Стіна коментарів";
    public override bool CanBeHidden => false;
}