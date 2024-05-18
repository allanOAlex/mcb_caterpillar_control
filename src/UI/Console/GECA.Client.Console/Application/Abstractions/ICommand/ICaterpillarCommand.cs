namespace GECA.Client.Console.Application.Abstractions.ICommand
{
    public interface ICaterpillarCommand
    {
        void Execute();
        void Undo();
    }
}
