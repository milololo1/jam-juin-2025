using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UpgradeAbstract
{
    public abstract void upgrade_stat();
}

public class UpgradeHealth : UpgradeAbstract
{
    public override void upgrade_stat()
    {
        PlayerStats.max_hp += 20f;
    }
}

public class UpgradeDamage : UpgradeAbstract
{
    public override void upgrade_stat()
    {
        PlayerStats.attack_power += 0.1f;
    }
}

public class UpgradeBulletSpeed : UpgradeAbstract
{
    public override void upgrade_stat()
    {
        PlayerStats.attack_speed += 0.1f;
    }
}

public class UpgradeMovementSpeed : UpgradeAbstract
{
    public override void upgrade_stat()
    {
        PlayerStats.movement_speed += 0.1f;
    }
}
