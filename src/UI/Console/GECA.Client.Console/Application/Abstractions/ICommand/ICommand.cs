﻿namespace GECA.Client.Console.Application.Abstractions.ICommand
{
    public interface ICommand
    {
        Task ExecuteAsync(CaterpillarSimulation simulation);
        Task UndoAsync(CaterpillarSimulation simulation);
    }
}
