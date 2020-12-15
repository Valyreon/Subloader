namespace SubloaderWpf.Interfaces
{
    public interface INavigator
    {
        void GoToControl(object control);

        void GoToPreviousControl();
    }
}
