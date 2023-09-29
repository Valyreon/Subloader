using System.CommandLine;

namespace SubloaderCLI.Interfaces;
public interface ICommand
{
    public Command BuildCommand();
}
