namespace SubloaderWpf.ViewModels
{
    public interface INavigator
    {
        void GoToControl(object control);

        void GoToPreviousControl();
    }
}
