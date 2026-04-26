namespace SubloaderWpf.Interfaces;

public interface INavigator
{
    public void GoToControl(object control);

    public void GoToPreviousControl();
}
