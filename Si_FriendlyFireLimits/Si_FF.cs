/*
 Silica Friendly Fire Limits Mod
 Copyright (C) 2023 by databomb
 
 * Description *
 For Silica listen servers, limits friendly fire in certain situations.

 * License *
 This program is free software: you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation, either version 3 of the License, or
 (at your option) any later version.
 
 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.
 
 You should have received a copy of the GNU General Public License
 along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using UnityEngine;
using MelonLoader;
using Il2Cpp;
using HarmonyLib;

namespace Si_FriendlyFireLimits
{
    public class  FriendlyFireLimits : MelonMod
    {
        [HarmonyPatch(typeof(DamageManager), nameof(DamageManager.ApplyDamage))]
        static class ApplyDamagePatch
        {
            public static bool Prefix(Il2Cpp.DamageManager __instance, ref float __result, UnityEngine.Collider __0, float __1, Il2Cpp.EDamageType __2, UnityEngine.GameObject __3, UnityEngine.Vector3 __4)
            {
                // Victim Team
                Il2Cpp.BaseGameObject victimBase = __instance.Owner;
                Il2Cpp.Team victimTeam = __instance.Team;
                // Attacker Team
                Il2Cpp.BaseGameObject attackerBase = Il2Cpp.GameFuncs.GetBaseGameObject(__3);
                Il2Cpp.Team attackerTeam = attackerBase.Team;

                // if they'rea on the same team but allow fall damage
                if (victimTeam == attackerTeam && victimBase != attackerBase)
                {
                    // Victim Object Type
                    Il2Cpp.ObjectInfoType victimType = victimBase.ObjectInfo.ObjectType;
                    // Attacker Object Type
                    Il2Cpp.ObjectInfoType attackerType = attackerBase.ObjectInfo.ObjectType;

                    // block units attacking friendly units
                    if (victimType == Il2Cpp.ObjectInfoType.Unit && attackerType == Il2Cpp.ObjectInfoType.Unit)
                    {
                        // but don't block AoE
                        if (__2 != Il2Cpp.EDamageType.Explosion)
                        {
                            // find out if attacker was a playable character
                            /*
                            Il2Cpp.NetworkComponent attackerNetComp = attackerBase.NetworkComponent;
                            Il2Cpp.Player attackerPlayer = attackerNetComp.OwnerPlayer;
                            MelonLogger.Msg(attackerPlayer.PlayerName + " was team attacking.");
                            */

                            __result = 0.0f;

                            return false;
                        }
                    }

                    // reduce damage of units attacking friendly structures
                    if (victimType == Il2Cpp.ObjectInfoType.Structure && attackerType == Il2Cpp.ObjectInfoType.Unit)
                    {
                        // AoE goes through with more damage
                        if (__2 == Il2Cpp.EDamageType.Explosion)
                        {
                            __result = __1 * 0.65f;
                        }
                        else
                        {
                            __result = __1 * 0.25f;
                        }

                        return true;
                    }
                }

                return true;
            }
        }
    }
}
