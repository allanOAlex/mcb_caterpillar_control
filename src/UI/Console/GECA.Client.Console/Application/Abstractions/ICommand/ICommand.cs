﻿using GECA.Client.Console.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Application.Abstractions.ICommand
{
    public interface ICommand
    {
        void Execute(CaterpillarState state);
        void Undo(CaterpillarState state);
    }
}
