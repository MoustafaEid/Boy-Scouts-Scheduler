﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boy_Scouts_Scheduler.Models
{
    public class ReleaseInitializer : DontDropDbJustCreateTablesIfModelChanged<SchedulingContext>
    {
        protected override void Seed(SchedulingContext context)
        {
            context.Seed();
        }
    }
}