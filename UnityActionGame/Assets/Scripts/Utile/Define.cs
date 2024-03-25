using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Define
{
    class Keys
    {
        public const string mouseX = "Mouse X";
        public const string mouseY = "Mouse Y";
        public const string mouseScroll = "Mouse ScrollWheel";
        public const string horizontal = "Horizontal";
        public const string vertical = "Vertical";
    }
    class Animations
    {
        public const string speed = "Speed";
        public const string comboCount = "ComboCount";
        public const string isNextCombo = "isNextCombo";
        public const string isAttacking = "isAttacking";
    }

    class Layers
    {
        public const string player = "Player";
        public const string enemy = "Enemy";
        public const string opstacle = "Opstacle";
    }

    public enum ItemType : int
    {
        None = -1,
        Equipment, //장비
        Countable, //소모
        Etc //기타
    }
}
