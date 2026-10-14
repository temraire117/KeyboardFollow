
using System;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KeyboardFollow
{
    [BepInPlugin(
        "com.acsard.sephiria.keyboardfollow",
        "Keyboard Follow",
        "1.0.0"
    )]
    public class KeyboardFollowPlugin : BaseUnityPlugin
    {
        public static Vector2 LastDirection = Vector2.down;

        private Harmony harmony;

        private void Awake()
        {
            harmony = new Harmony(
                "com.acsard.sephiria.keyboardfollow"
            );

            harmony.PatchAll();
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;

            if (keyboard == null)
                return;

            Vector2 direction = Vector2.zero;

            if (keyboard.wKey.isPressed) direction.y += 1f;
            if (keyboard.sKey.isPressed) direction.y -= 1f;
            if (keyboard.aKey.isPressed) direction.x -= 1f;
            if (keyboard.dKey.isPressed) direction.x += 1f;

            if (direction.sqrMagnitude > 0f)
                LastDirection = direction.normalized;
        }

        private void OnDestroy()
        {
            harmony?.UnpatchSelf();
        }
    }

    // 무기 조준 위치 보정
    [HarmonyPatch(typeof(WeaponControllerSimple), "Update")]
    public static class WeaponControllerUpdatePatch
    {
        private static void Prefix(
            WeaponControllerSimple __instance)
        {
            Vector2 direction =
                KeyboardFollowPlugin.LastDirection;

            if (direction.sqrMagnitude < 0.001f)
                return;

            __instance.aimedPositionClientside =
                __instance.transform.position +
                new Vector3(
                    direction.x,
                    direction.y,
                    0f
                );
        }
    }

    // 일반 공격 입력 방향 보정
    [HarmonyPatch(typeof(PlayerAvatar), "AttackButtonDown")]
    public static class AttackButtonDownPatch
    {
        private static void Prefix(
            ref Vector2 attackDirection)
        {
            Vector2 direction =
                KeyboardFollowPlugin.LastDirection;

            if (direction.sqrMagnitude < 0.001f)
                return;

            attackDirection = direction;
        }
    }

    // 특수 공격 입력 방향 보정
    [HarmonyPatch(
        typeof(WeaponControllerSimple),
        nameof(WeaponControllerSimple.SubAttackButtonDown),
        new Type[] { typeof(Vector2) }
    )]
    public static class SubAttackButtonDownPatch
    {
        private static void Prefix(
            ref Vector2 attackDirection)
        {
            Vector2 direction =
                KeyboardFollowPlugin.LastDirection;

            if (direction.sqrMagnitude < 0.001f)
                return;

            attackDirection = direction;
        }
    }

    // 기본 공격 생성 시점의 방향 보정
    [HarmonyPatch(
        typeof(WeaponControllerSimple),
        "CreateBasicAttackSwing",
        new Type[] { typeof(int) }
    )]
    public static class BasicAttackSwingPatch
    {
        private static void Prefix(
            WeaponControllerSimple __instance)
        {
            Vector2 direction =
                KeyboardFollowPlugin.LastDirection;

            if (direction.sqrMagnitude < 0.001f)
                return;

            __instance.attackDirection =
                new Vector3(
                    direction.x,
                    direction.y,
                    0f
                );
        }
    }

    // 단검 공격 방향 보정
    [HarmonyPatch(typeof(WeaponSimple_Dagger), "LateUpdate")]
    public static class DaggerLateUpdatePatch
    {
        private static readonly FieldInfo IsAttackButtonDownField =
            AccessTools.Field(
                typeof(WeaponSimple_Dagger),
                "isAttackButtonDown"
            );

        private static void Postfix(
            WeaponSimple_Dagger __instance)
        {
            if (__instance == null)
                return;

            var weapon = __instance.Networkowner;

            if (weapon == null)
                return;

            if (!__instance.isServer)
                return;

            if (weapon.unitAvatar == null)
                return;

            if (IsAttackButtonDownField == null)
                return;

            bool isAttackButtonDown =
                (bool)IsAttackButtonDownField.GetValue(
                    __instance
                );

            if (!isAttackButtonDown)
                return;

            if (weapon.currentWeaponSwing != -1)
                return;

            if (weapon.allowedInputBufferName.Equals(
                "DASHATTACK",
                StringComparison.OrdinalIgnoreCase))
                return;

            if (!weapon.unitAvatar.CanMove)
                return;

            Vector2 direction =
                KeyboardFollowPlugin.LastDirection;

            weapon.NetworkattackDirection =
                new Vector3(
                    direction.x,
                    direction.y,
                    0f
                );
        }
    }


    // 마법 조준 방향 보정
    [HarmonyPatch(typeof(PlayerInputController), "Update")]
    public static class PlayerInputControllerAimPatch
    {
        private static readonly FieldInfo AvatarField =
            AccessTools.Field(
                typeof(PlayerInputController),
                "avatar"
            );

        private static PlayerInputController cachedController;
        private static PlayerAvatar cachedAvatar;

        private static void Postfix(
            PlayerInputController __instance)
        {
            if (__instance == null ||
                __instance.playerInput == null)
                return;

            if (__instance.playerInput.currentControlScheme !=
                PlayerInputController.KeyboardAndMouseScheme)
                return;

            if (AvatarField == null)
                return;

            // 컨트롤러가 바뀔 때만 아바타를 다시 조회
            if (cachedController != __instance ||
                cachedAvatar == null)
            {
                cachedController = __instance;
                cachedAvatar =
                    AvatarField.GetValue(__instance)
                    as PlayerAvatar;
            }

            PlayerAvatar avatar = cachedAvatar;

            if (avatar == null ||
                avatar.NetworkaimObject == null)
                return;

            Vector2 direction =
                KeyboardFollowPlugin.LastDirection;

            if (direction.sqrMagnitude < 0.001f)
                return;

            Vector2 aimPosition =
                (Vector2)avatar.transform.position +
                direction.normalized *
                PlayerInputController.MaxAimDistance;

            __instance.ForceAimToPosition(aimPosition);
        }
    }

}
