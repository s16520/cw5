﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cw5.DTOs.Responses
{
    public class PromotionResponse : IResponse
    {
        public string Studies { get; set; }
        public int Semester { get; set; }
    }
}