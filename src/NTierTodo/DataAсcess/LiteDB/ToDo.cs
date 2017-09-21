﻿using System;
using Microsoft.Rest;

namespace NTierTodo.Dal.Entities
{
    public class ToDo
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool IsComplete { get; set; }
    }
}
