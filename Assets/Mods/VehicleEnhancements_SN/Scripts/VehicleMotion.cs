using UnityEngine;

namespace DaftAppleGames.VehicleEnhancements_SN
{
    internal enum EnhancedVehicle
    {
        Seamoth,
        PrawnSuit,
        Cyclops
    }

    internal static class VehicleMotion
    {
        internal static bool TryGet(EnhancedVehicle vehicle, out Transform vehicleTransform, out Vector3 velocity)
        {
            vehicleTransform = null;
            velocity = Vector3.zero;

            Player player = Player.main;
            if (!player)
            {
                return false;
            }

            Rigidbody rigidbody;
            switch (vehicle)
            {
                case EnhancedVehicle.PrawnSuit:
                    Exosuit exosuit = player.GetVehicle() as Exosuit;
                    if (!exosuit)
                    {
                        return false;
                    }

                    vehicleTransform = exosuit.transform;
                    rigidbody = exosuit.useRigidbody;
                    break;

                case EnhancedVehicle.Cyclops:
                    SubRoot cyclops = player.currentSub;
                    if (!cyclops || !cyclops.isCyclops || !player.isPiloting || player.GetVehicle())
                    {
                        return false;
                    }

                    vehicleTransform = cyclops.transform;
                    rigidbody = cyclops.rb;
                    break;

                default:
                    SeaMoth seaMoth = player.GetVehicle() as SeaMoth;
                    if (!seaMoth)
                    {
                        return false;
                    }

                    vehicleTransform = seaMoth.transform;
                    rigidbody = seaMoth.useRigidbody;
                    break;
            }

            if (!rigidbody)
            {
                return false;
            }

            velocity = rigidbody.velocity;
            return true;
        }
    }
}
