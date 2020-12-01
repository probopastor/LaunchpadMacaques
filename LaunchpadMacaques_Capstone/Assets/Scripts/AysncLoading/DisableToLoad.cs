/* 
* (Launchpad Macaques - [Trial and Error]) 
* (Levi Schoof) 
* (_ActivateNextLevel.CS) 
* (Implaments the Abstract class _ActivateNextLevel, when this object is disabled will enable level loading) 
*/

public class DisableToLoad : _ActivateNextLevel
{
    private void OnDisable()
    {
        LoadNextLevel();
    }
}
