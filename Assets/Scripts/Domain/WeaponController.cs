using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace Assets.Scripts.Domain
{
    public class WeaponController
    {
        public event Action<int, float, float, float, float, float> MeleeAttackEvent;
        public event Action<int, float, float, float, float, Sprite> RangeAttackEvent;
        public event Action<Vector2, float, int> OnAttackEvent;

        private bool canAttack = true;
        private float timeAttackMelee = 0.6f;
        private float speedMelee = 0.5f;

        private float timeAttackRange = 0.4f;
        private float forceRange = 15f;
        private float speedRange = 0.2f;

        private float frameRate = 0.1f;

        public void Attack(int weaponIndex, Vector2 mousePos, Vector2 currentPos)
        {
            if (WeaponObject.Instance == null || !canAttack) return;

            canAttack = false;
            if (WeaponObject.Instance.isMelee)
            {
                MelleeAttack(weaponIndex, mousePos, currentPos);
            } 
            else
            {
                RangeAttack(weaponIndex, mousePos, currentPos);
            }
        }

        public void MelleeAttack(int weaponIndex, Vector2 mousePos, Vector2 currentPos)
        {
            float curentAngle = GetAngleMouseAndWeapon(mousePos, currentPos);
            Debug.Log("Current angle: " + curentAngle);
            float startAngle;
            float endAngle;

            if (curentAngle < 90 && curentAngle >= -90)
            {
                startAngle = curentAngle + 75;
                endAngle = curentAngle - 75;
            } 
            else
            {
                startAngle = curentAngle - 75;
                endAngle = curentAngle + 75;
            }

            MeleeAttackEvent?.Invoke(weaponIndex, curentAngle, startAngle, endAngle, timeAttackMelee, speedMelee);
        }

        public void RangeAttack(int weaponIndex, Vector2 mousePos, Vector2 currentPos)
        {
            RangeAttackEvent?.Invoke(weaponIndex, GetAngleMouseAndWeapon(mousePos, currentPos), forceRange, timeAttackRange, speedRange, WeaponObject.Instance.hitSprite);
        }

        public void ResetAttack()
        {
            canAttack = true;
        }

        private float GetAngleMouseAndWeapon(Vector2 mousePos, Vector2 currentPos)
        {
            mousePos = Mouse.current.position.ReadValue();
            Vector2 mouseWorldPos = (Vector2)Camera.main.ScreenToWorldPoint(
                new Vector3(mousePos.x, mousePos.y, 0)
            );

            Vector2 lookDir = mouseWorldPos - currentPos;

            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            return angle;
        }

        public void OnHitEnemy(int WeaponIndex, Vector2 position)
        {
            OnAttackEvent?.Invoke(position, frameRate, WeaponIndex);
        }
    }
}
