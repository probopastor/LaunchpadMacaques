public class DisableToLoad : _ActivateNextLevel
{
    private void OnDisable()
    {
        LoadNextLevel();
    }
}
