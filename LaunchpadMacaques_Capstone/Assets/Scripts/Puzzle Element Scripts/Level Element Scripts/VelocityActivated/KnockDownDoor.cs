/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi Schoof) 
* (KnockDownDoor.CS) 
* (Implaments the Velocity Activated Abstract class, when activated will destory an object) 
*/


public class KnockDownDoor : A_VelocityActivated
{
    public override void Activate()
    {
        this.gameObject.SetActive(false);
    }
}
