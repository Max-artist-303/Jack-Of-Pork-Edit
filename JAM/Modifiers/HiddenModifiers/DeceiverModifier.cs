using MiraAPI.Modifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAM.Modifiers.HiddenModifiers;

internal class DeceiverModifier : BaseModifier
{
    public override string ModifierName => "Deceiver Modifier";
    public override bool HideOnUi => true;
}
