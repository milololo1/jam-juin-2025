using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeItem : MonoBehaviour
{

    public UpgradeAbstract upgrade;
    public Game game;

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            upgrade.upgrade_stat();
            game.complete();
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        int random_value = Random.Range(0, 3);
        switch(random_value)
        {
            case 0:
                upgrade = new UpgradeHealth();
                break;
            case 1:
                upgrade = new UpgradeDamage();
                break;
            case 2:
                upgrade = new UpgradeBulletSpeed();
                break;
            case 3:
                upgrade = new UpgradeMovementSpeed();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
