namespace Architecture.Components
{
    public class DontDestroyBaseMenu : BaseMenu
    {
        private static DontDestroyBaseMenu _instance = null;

        protected void Init()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                _instance = this;
            }

            DontDestroyOnLoad(this.gameObject);
        }
    }
}