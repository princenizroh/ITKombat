using UnityEngine;
using Unity.Netcode.Components;
namespace ITKombat
{
    public class OwnerNetworkAnimator : NetworkAnimator {
        protected override bool OnIsServerAuthoritative() {
            return false;
        }
    }
}
